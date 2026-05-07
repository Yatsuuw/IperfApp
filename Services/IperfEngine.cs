using System.Diagnostics;
using System.Globalization;
using IperfApp.Models;

namespace IperfApp.Services;

public class IperfEngine
{
  public event Action<string>? OnLogReceived;

  public async Task<double> ExecuteAsync(string server, string port, string channels, bool isReverse, IpVersion ipVersion = IpVersion.Auto)
  {
    // En mode Auto on tente IPv4 d'abord, puis IPv6 si échec
    if (ipVersion == IpVersion.Auto)
    {
      double result = await RunAsync(server, port, channels, isReverse, "-4");
      if (result > 0) return result;
      OnLogReceived?.Invoke("[Auto] IPv4 sans résultat, tentative en IPv6...");
      return await RunAsync(server, port, channels, isReverse, "-6");
    }

    string ipFlag = ipVersion == IpVersion.IPv6 ? "-6" : "-4";
    return await RunAsync(server, port, channels, isReverse, ipFlag);
  }

  private async Task<double> RunAsync(string server, string port, string channels, bool isReverse, string ipFlag)
  {
    string args = $"-c {server} -p {port} -P {channels} {ipFlag} {(isReverse ? "-R" : "")} -f m -i 1".Trim();
    double finalBitrate = 0;

    ProcessStartInfo psi = new()
    {
      FileName = Path.Combine(AppContext.BaseDirectory, "Resources", "iperf3.exe"),
      Arguments = args,
      RedirectStandardOutput = true,
      RedirectStandardError  = true,
      UseShellExecute = false,
      CreateNoWindow  = true
    };

    using var proc = new Process { StartInfo = psi };
    proc.Start();

    // Lecture de stderr en arrière-plan pour ne pas bloquer
    var stderrTask = Task.Run(async () =>
    {
      string? line;
      while ((line = await proc.StandardError.ReadLineAsync()) != null)
      {
        if (!string.IsNullOrWhiteSpace(line))
          OnLogReceived?.Invoke($"[ERREUR iperf3] {line}");
      }
    });

    // Lecture de stdout ligne par ligne : toutes les lignes sont traitées
    // avant de continuer, ce qui évite le problème de race condition
    string? outputLine;
    while ((outputLine = await proc.StandardOutput.ReadLineAsync()) != null)
    {
      OnLogReceived?.Invoke(outputLine);
      if (outputLine.Contains("receiver"))
      {
        double parsed = ParseLine(outputLine);
        if (parsed > 0) finalBitrate = parsed;
      }
    }

    await proc.WaitForExitAsync();
    await stderrTask;
    return finalBitrate;
  }

  // Convertit n'importe quelle unité (Kbits, Mbits, Gbits) en Mbps
  private static double ParseLine(string line)
  {
    try
    {
      var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      for (int i = 1; i < parts.Length; i++)
      {
        string unit = parts[i];
        if (!double.TryParse(parts[i - 1], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
          continue;

        if (unit.StartsWith("Gbits")) return value * 1000.0;
        if (unit.StartsWith("Mbits")) return value;
        if (unit.StartsWith("Kbits")) return value / 1000.0;
      }
    }
    catch { }
    return 0;
  }
}
