using IperfApp.Models;
using IperfApp.UI.Constants;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
  private async Task RunFullTest()
  {
    if (string.IsNullOrWhiteSpace(txtServer.Text))
    {
      MessageBox.Show(this, "Veuillez entrer l'adresse du serveur.", "Champ requis",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

    _lastPreset = BuildCurrentPreset();

    string? validationError = _lastPreset.Validate();
    if (validationError is not null)
    {
      MessageBox.Show(this, validationError, "Valeurs invalides",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

    _testCts?.Dispose();
    _testCts = new CancellationTokenSource();
    var ct = _testCts.Token;

    SetTestRunningState(true);
    txtLog.Clear();

    try
    {
      AppendLog("[SYSTÈME] Démarrage des flux...");
      AppendLog(">>> FLUX MONTANT (UPLOAD)");
      double up = await _engine.ExecuteAsync(_lastPreset, isReverse: false, ct);

      if (ct.IsCancellationRequested) return;

      AppendLog(string.Empty);
      AppendLog("<<< FLUX DESCENDANT (DOWNLOAD)");
      double down = await _engine.ExecuteAsync(_lastPreset, isReverse: true, ct);

      if (!ct.IsCancellationRequested)
      {
        _lastResult = new TestResult(up, down, DateTime.Now);
        DisplayResults(_lastResult);
      }
    }
    catch (OperationCanceledException)
    {
      AppendLog("[SYSTÈME] Test annulé par l'utilisateur.");
    }
    catch (Exception ex)
    {
      MessageBox.Show(this, $"Erreur inattendue : {ex.Message}", "Erreur",
        MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
      _testCts?.Dispose();
      _testCts = null;
      SetTestRunningState(false);
    }
  }

  private Preset BuildCurrentPreset()
  {
    var selectedPreset = cbPresets.SelectedItem as Preset;

    _ = int.TryParse(txtPort.Text, out int port);
    _ = int.TryParse(txtChannels.Text, out int channels);

    return new Preset
    {
      Name = selectedPreset?.Name ?? "Temporaire",
      Server = txtServer.Text.Trim(),
      Port = port,
      Channels = channels,
      Duration = selectedPreset is { Duration: > 0 } p ? p.Duration : 10,
      IpVersion = IpVersionExtensions.FromComboIndex(cbIpVersion.SelectedIndex)
    };
  }

  private void SetTestRunningState(bool running)
  {
    _testRunning = running;

    btnStart.Enabled = !running;
    btnCancel.Enabled =  running;
    btnStart.Text = running ? "ANALYSE EN COURS..." : "LANCER L'ANALYSE";
    btnStart.BackColor = running ? AppColors.AccentDisabled : AppColors.Accent;

    if (!running)
    {
      bool hasResult = _lastResult is not null;
      btnExportNew.Enabled = hasResult;
      btnExportAppend.Enabled = hasResult;
      btnExportNew.FlatAppearance.BorderColor = hasResult ? AppColors.Accent : AppColors.ExportBorderDisabled;
      btnExportAppend.FlatAppearance.BorderColor = hasResult ? AppColors.Accent : AppColors.ExportBorderDisabled;
    }
  }

  private void DisplayResults(TestResult r)
  {
    const string labelUp = "Upload   : ";
    const string labelDown = "Download : ";
    const int labelW = 13;
    const int titleMinW = 26;

    string up = DisplayHelpers.FormatMbps(r.Upload);
    string down = DisplayHelpers.FormatMbps(r.Download);

    int valueW = Math.Max(up.Length, down.Length);
    int innerW = Math.Max(labelW + valueW + 2, titleMinW);
    string sep = new('\u2500', innerW);

    var sb = new StringBuilder();
    sb.AppendLine();
    sb.AppendLine($"\u250c{sep}\u2510");
    sb.AppendLine($"\u2502R\u00c9SULTATS DE LA MESURE{new string(' ', innerW - 24)}\u2502");
    sb.AppendLine($"\u251c{sep}\u2524");
    sb.AppendLine($"\u2502{labelUp}{up.PadLeft(valueW)}\u2502");
    sb.AppendLine($"\u2502{labelDown}{down.PadLeft(valueW)}\u2502");
    sb.AppendLine($"\u2514{sep}\u2518");
    txtLog.AppendText(sb.ToString());
  }
}
