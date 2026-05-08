using System.Text.RegularExpressions;
using IperfApp.Models;

namespace IperfApp.Services;

public sealed class IperfEngine : IDisposable
{
  private static readonly string IperfExePath =
    Path.Combine(AppContext.BaseDirectory, "Resources", "iperf3.exe");

  private static readonly Regex BitrateRegex = new(
    @"([\d.]+)\s*(G|M|K)bits/sec",
    RegexOptions.Compiled | RegexOptions.IgnoreCase,
    TimeSpan.FromSeconds(1));

  private bool _disposed;

  public event Action<string>? OnLogReceived;

  public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(90);

  public async Task<double> ExecuteAsync(Preset preset, bool isReverse, CancellationToken ct = default)
  {
    ObjectDisposedException.ThrowIf(_disposed, this);
    ArgumentNullException.ThrowIfNull(preset);

    using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
    timeoutCts.CancelAfter(Timeout);
    var linkedCt = timeoutCts.Token;

    if (preset.IpVersion == IpVersion.Auto)
    {
      double result = await RunAsync(preset, isReverse, "-4", linkedCt);
      if (result > 0) return result;
      if (linkedCt.IsCancellationRequested) return 0;
      OnLogReceived?.Invoke("[Auto] IPv4 sans résultat, tentative en IPv6...");
      return await RunAsync(preset, isReverse, "-6", linkedCt);
    }

    string ipFlag = preset.IpVersion == IpVersion.IPv6 ? "-6" : "-4";
    return await RunAsync(preset, isReverse, ipFlag, linkedCt);
  }

  public void Dispose()
  {
    if (_disposed) return;
    OnLogReceived = null;
    _disposed = true;
  }

  private async Task<double> RunAsync(Preset preset, bool isReverse, string ipFlag, CancellationToken ct)
  {
    double finalBitrate = 0;

    var psi = new ProcessStartInfo
    {
      FileName = IperfExePath,
      Arguments = BuildArgs(preset, isReverse, ipFlag),
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true
    };

    using var proc = new Process { StartInfo = psi };

    try
    {
      proc.Start();
    }
    catch (Exception ex)
    {
      OnLogReceived?.Invoke($"[ERREUR] Impossible de lancer iperf3.exe : {ex.Message}");
      return 0;
    }

    var stderrTask = Task.Run(async () =>
    {
      try
      {
        string? line;
        while ((line = await proc.StandardError.ReadLineAsync(ct)) != null)
          if (!string.IsNullOrWhiteSpace(line))
            OnLogReceived?.Invoke($"[ERREUR iperf3] {line}");
      }
      catch (OperationCanceledException)
      {
        Debug.WriteLine("[IperfEngine] Lecture stderr annulée (test annulé, processus tué par KillProcess).");
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"[IperfEngine] Lecture stderr échouée : {ex.Message}");
      }
    }, CancellationToken.None);

    try
    {
      string? outputLine;
      while ((outputLine = await proc.StandardOutput.ReadLineAsync(ct)) != null)
      {
        OnLogReceived?.Invoke(outputLine);
        if (outputLine.Contains("receiver", StringComparison.Ordinal))
        {
          double parsed = ParseBitrate(outputLine);
          if (parsed > 0) finalBitrate = parsed;
        }
      }
    }
    catch (OperationCanceledException)
    {
      OnLogReceived?.Invoke("[AVERTISSEMENT] Test annulé (timeout ou annulation manuelle).");
      KillProcess(proc);
    }

    try
    {
      await proc.WaitForExitAsync(CancellationToken.None);
      await stderrTask;
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[IperfEngine] WaitForExitAsync exception inattendue : {ex.Message}");
    }

    return finalBitrate;
  }

  private static void KillProcess(Process proc)
  {
    if (proc.HasExited) return;
    try { proc.Kill(entireProcessTree: true); }
    catch (Exception ex)
    {
      Debug.WriteLine($"[IperfEngine] Impossible de tuer iperf3 : {ex.Message}");
    }
  }

  private static string BuildArgs(Preset preset, bool isReverse, string ipFlag)
  {
    string reverse = isReverse ? " -R" : string.Empty;
    return $"-c {preset.Server} -p {preset.Port} -P {preset.Channels} -t {preset.Duration} {ipFlag}{reverse} -f m";
  }

  private static double ParseBitrate(string line)
  {
    var match = BitrateRegex.Match(line);
    if (!match.Success) return 0;

    if (!double.TryParse(match.Groups[1].Value, NumberStyles.Float,
      CultureInfo.InvariantCulture, out double value)) return 0;

    return match.Groups[2].Value.ToUpperInvariant() switch
    {
      "G" => value * 1000.0,
      "K" => value / 1000.0,
      _ => value
    };
  }
}
