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
      UseShellExecute  = false,
      CreateNoWindow   = true
    };

    using var proc = new Process { StartInfo = psi };

    proc.OutputDataReceived += (s, e) =>
    {
      if (e.Data is null) return;
      OnLogReceived?.Invoke(e.Data);
      // On cible la ligne SUM receiver (multi-flux) ou receiver (flux unique)
      if (e.Data.Contains("receiver"))
      {
        double parsed = ParseLine(e.Data);
        if (parsed > 0) finalBitrate = parsed;
      }
    };

    proc.ErrorDataReceived += (s, e) =>
    {
      if (!string.IsNullOrWhiteSpace(e.Data))
        OnLogReceived?.Invoke($"[ERREUR iperf3] {e.Data}");
    };

    proc.Start();
    proc.BeginOutputReadLine();
    proc.BeginErrorReadLine();
    await proc.WaitForExitAsync();
    return finalBitrate;
  }

  // Convertit n'importe quelle unité (Kbits, Mbits, Gbits) en Mbps
  private static double ParseLine(string line)
  {
    try
    {
      var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < parts.Length; i++)
      {
        if (i == 0) continue;
        string unit = parts[i];
        double value = double.Parse(parts[i - 1], CultureInfo.InvariantCulture);

        if (unit.StartsWith("Gbits")) return value * 1000.0;
        if (unit.StartsWith("Mbits")) return value;
        if (unit.StartsWith("Kbits")) return value / 1000.0;
      }
    }
    catch { }
    return 0;
  }
}
