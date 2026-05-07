using IperfApp.Models;

namespace IperfApp.UI;

public partial class Form1
{
  /// <summary>Lance le test Upload puis Download sur le profil actif.</summary>
  private async Task RunFullTest()
  {
    if (string.IsNullOrWhiteSpace(txtServer.Text))
    {
      MessageBox.Show("Veuillez entrer l'adresse du serveur.", "Champ requis",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

    // Construction du profil courant depuis l'UI
    var preset = BuildCurrentPreset();

    // Token d'annulation
    _testCts?.Dispose();
    _testCts = new CancellationTokenSource();
    var ct = _testCts.Token;

    // État UI : en cours
    SetTestRunningState(true);
    txtLog.Clear();

    try
    {
      txtLog.AppendText(" [SYSTÈME] Démarrage des flux..." + Environment.NewLine);
      txtLog.AppendText(" >>> FLUX MONTANT (UPLOAD)" + Environment.NewLine);
      double up = await _engine.ExecuteAsync(preset, isReverse: false, ct);

      if (ct.IsCancellationRequested) return;

      txtLog.AppendText(Environment.NewLine + " <<< FLUX DESCENDANT (DOWNLOAD)" + Environment.NewLine);
      double down = await _engine.ExecuteAsync(preset, isReverse: true, ct);

      if (!ct.IsCancellationRequested)
      {
        _lastResult = new TestResult(up, down, DateTime.Now);
        DisplayResults(_lastResult);
      }
    }
    catch (OperationCanceledException)
    {
      txtLog.AppendText(" [SYSTÈME] Test annulé par l'utilisateur." + Environment.NewLine);
    }
    catch (Exception ex)
    {
      MessageBox.Show($"Erreur inattendue : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
      SetTestRunningState(false);
    }
  }

  /// <summary>Construit un <see cref="Preset"/> depuis les champs de l'UI.</summary>
  private Preset BuildCurrentPreset()
  {
    _ = int.TryParse(txtPort.Text, out int port);
    _ = int.TryParse(txtChannels.Text, out int channels);

    return new Preset
    {
      Name     = cbPresets.SelectedItem is Preset p ? p.Name : "Temporaire",
      Server   = txtServer.Text.Trim(),
      Port     = port > 0 ? port : 5201,
      Channels = channels > 0 ? channels : 8,
      IpVersion = cbIpVersion.SelectedIndex switch
      {
        1 => IpVersion.IPv4,
        2 => IpVersion.IPv6,
        _ => IpVersion.Auto
      }
    };
  }

  /// <summary>Active ou désactive les contrôles selon l'état du test.</summary>
  private void SetTestRunningState(bool running)
  {
    btnStart.Enabled  = !running;
    btnCancel.Enabled =  running;
    btnStart.Text     = running ? "ANALYSE EN COURS..." : "LANCER L'ANALYSE";
    btnStart.BackColor = running
      ? Color.FromArgb(160, 174, 192)
      : _colorAccent;

    if (!running)
    {
      bool hasResult = _lastResult is not null;
      btnExportNew.Enabled    = hasResult;
      btnExportAppend.Enabled = hasResult;
      btnExportNew.FlatAppearance.BorderColor    = hasResult ? _colorAccent : Color.FromArgb(210, 220, 230);
      btnExportAppend.FlatAppearance.BorderColor = hasResult ? _colorAccent : Color.FromArgb(210, 220, 230);
    }
  }

  private void DisplayResults(TestResult r)
  {
    txtLog.AppendText(Environment.NewLine +
      " ╔══════════════════════════════════════╗" + Environment.NewLine +
      $" ║  RÉSULTATS DE LA MESURE              ║" + Environment.NewLine +
      $" ║  Upload   : {r.Upload,10:F2} Mbps          ║" + Environment.NewLine +
      $" ║  Download : {r.Download,10:F2} Mbps          ║" + Environment.NewLine +
      " ╚══════════════════════════════════════╝" + Environment.NewLine);
  }
}
