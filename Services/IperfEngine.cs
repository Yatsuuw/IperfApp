using System.Diagnostics;
using System.Globalization;

namespace IperfApp.Services;

public class IperfEngine
{
  public event Action<string>? OnLogReceived;

  public async Task<double> ExecuteAsync(string server, string port, string channels, bool isReverse)
  {
    string args = $"-c {server} -p {port} -P {channels} {(isReverse ? "-R" : "")} -f m -i 1";
    double finalBitrate = 0;

    ProcessStartInfo psi = new()
    {
      FileName = Path.Combine(AppContext.BaseDirectory, "Resources", "iperf3.exe"),
      Arguments = args, RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true
    };

    using var proc = new Process { StartInfo = psi };
    proc.OutputDataReceived += (s, e) => {
      if (e.Data != null) 
      {
        OnLogReceived?.Invoke(e.Data);
        if (e.Data.Contains("receiver")) finalBitrate = ParseLine(e.Data);
      }
    };

    proc.Start(); proc.BeginOutputReadLine();
    await proc.WaitForExitAsync();
    return finalBitrate;
  }

  private static double ParseLine(string line) {
    try 
    {
      var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < parts.Length; i++) 
      {
        if (parts[i].Contains("Mbits/sec") && i > 0)
          return double.Parse(parts[i - 1], CultureInfo.InvariantCulture);
      }
    } catch
    {

    }
    return 0;
  }
}