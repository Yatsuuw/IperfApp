using System.Diagnostics;
using System.Globalization;
using IperfApp.Models;

namespace IperfApp.Services;

/// <summary>Orchestre les exécutions d'iperf3.exe et retourne le débit mesuré en Mbps.</summary>
public class IperfEngine
{
  /// <summary>Délégué invoqué pour chaque ligne de sortie d'iperf3 (stdout + stderr).</summary>
  public event Action<string>? OnLogReceived;

  /// <summary>Durée maximale avant annulation automatique du test (défaut : 60 s).</summary>
  public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(60);

  /// <summary>
  /// Exécute un test iperf3 (upload OU download) et retourne le débit en Mbps.
  /// Retourne 0 si aucun résultat n'est obtenu ou si le test est annulé.
  /// </summary>
  /// <param name="preset">Profil contenant serveur, port, canaux et version IP.</param>
  /// <param name="isReverse">Si <c>true</c>, ajoute <c>-R</c> pour mesurer le download.</param>
  /// <param name="ct">Token d'annulation externe optionnel.</param>
  public async Task<double> ExecuteAsync(Preset preset, bool isReverse, CancellationToken ct = default)
  {
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

  // --- Privé ---

  private async Task<double> RunAsync(Preset preset, bool isReverse, string ipFlag, CancellationToken ct)
  {
    string args = BuildArgs(preset, isReverse, ipFlag);
    double finalBitrate = 0;

    var psi = new ProcessStartInfo
    {
      FileName              = Path.Combine(AppContext.BaseDirectory, "Resources", "iperf3.exe"),
      Arguments             = args,
      RedirectStandardOutput = true,
      RedirectStandardError  = true,
      UseShellExecute        = false,
      CreateNoWindow         = true
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

    // Lecture stderr en arrière-plan
    var stderrTask = Task.Run(async () =>
    {
      string? line;
      while ((line = await proc.StandardError.ReadLineAsync(ct)) != null)
        if (!string.IsNullOrWhiteSpace(line))
          OnLogReceived?.Invoke($"[ERREUR iperf3] {line}");
    }, ct);

    // Lecture stdout : garantit que finalBitrate est rempli avant return
    try
    {
      string? outputLine;
      while ((outputLine = await proc.StandardOutput.ReadLineAsync(ct)) != null)
      {
        OnLogReceived?.Invoke(outputLine);
        if (outputLine.Contains("receiver"))
        {
          double parsed = ParseLine(outputLine);
          if (parsed > 0) finalBitrate = parsed;
        }
      }
    }
    catch (OperationCanceledException)
    {
      OnLogReceived?.Invoke("[AVERTISSEMENT] Test annulé (timeout ou annulation manuelle).");
      if (!proc.HasExited)
      {
        try { proc.Kill(); } catch { /* ignore */ }
      }
    }

    try
    {
      await proc.WaitForExitAsync(ct);
      await stderrTask;
    }
    catch (OperationCanceledException) { /* déjà géré ci-dessus */ }

    return finalBitrate;
  }

  private static string BuildArgs(Preset preset, bool isReverse, string ipFlag)
  {
    var sb = new System.Text.StringBuilder();
    sb.Append($"-c {preset.Server} -p {preset.Port} -P {preset.Channels} {ipFlag}");
    if (isReverse) sb.Append(" -R");
    sb.Append(" -f m -i 1");
    return sb.ToString();
  }

  /// <summary>Convertit n'importe quelle unité iperf3 (Kbits, Mbits, Gbits) en Mbps.</summary>
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

        if (unit.StartsWith("Gbits", StringComparison.Ordinal)) return value * 1000.0;
        if (unit.StartsWith("Mbits", StringComparison.Ordinal)) return value;
        if (unit.StartsWith("Kbits", StringComparison.Ordinal)) return value / 1000.0;
      }
    }
    catch { /* ligne malformée, on ignore */ }
    return 0;
  }
}
