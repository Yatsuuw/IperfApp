using System.Text;
using IperfApp.Models;
using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
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

        // Snapshot du profil au moment du lancement — l'export CSV utilisera
        // toujours CE profil, même si l'utilisateur modifie les champs après.
        _lastPreset = BuildCurrentPreset();

        // Dispose du CTS précédent avant d'en créer un nouveau
        _testCts?.Dispose();
        _testCts = new CancellationTokenSource();
        var ct = _testCts.Token;

        SetTestRunningState(true);
        txtLog.Clear();

        try
        {
            txtLog.AppendText(" [SYSTÈME] Démarrage des flux..." + Environment.NewLine);
            txtLog.AppendText(" >>> FLUX MONTANT (UPLOAD)" + Environment.NewLine);
            double up = await _engine.ExecuteAsync(_lastPreset, isReverse: false, ct);

            if (ct.IsCancellationRequested) return;

            txtLog.AppendText(Environment.NewLine + " <<< FLUX DESCENDANT (DOWNLOAD)" + Environment.NewLine);
            double down = await _engine.ExecuteAsync(_lastPreset, isReverse: true, ct);

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
            MessageBox.Show($"Erreur inattendue : {ex.Message}", "Erreur",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            // Toujours disposer le CTS et réactiver l'UI, même en cas d'exception
            _testCts?.Dispose();
            _testCts = null;
            SetTestRunningState(false);
        }
    }

    /// <summary>Construit un <see cref="Preset"/> depuis les champs de l'UI.</summary>
    private Preset BuildCurrentPreset()
    {
        _ = int.TryParse(txtPort.Text,     out int port);
        _ = int.TryParse(txtChannels.Text, out int channels);

        return new Preset
        {
            Name      = cbPresets.SelectedItem is Preset p ? p.Name : "Temporaire",
            Server    = txtServer.Text.Trim(),
            Port      = port     > 0 ? port     : 5201,
            Channels  = channels > 0 ? channels : 8,
            IpVersion = IpVersionExtensions.FromComboIndex(cbIpVersion.SelectedIndex)
        };
    }

    /// <summary>Active ou désactive les contrôles selon l'état du test.</summary>
    private void SetTestRunningState(bool running)
    {
        btnStart.Enabled   = !running;
        btnCancel.Enabled  =  running;
        btnStart.Text      = running ? "ANALYSE EN COURS..." : "LANCER L'ANALYSE";
        btnStart.BackColor = running ? AppColors.AccentDisabled : AppColors.Accent;

        if (!running)
        {
            bool hasResult = _lastResult is not null;
            btnExportNew.Enabled    = hasResult;
            btnExportAppend.Enabled = hasResult;
            btnExportNew.FlatAppearance.BorderColor    = hasResult ? AppColors.Accent : AppColors.ExportBorderDisabled;
            btnExportAppend.FlatAppearance.BorderColor = hasResult ? AppColors.Accent : AppColors.ExportBorderDisabled;
        }
    }

    /// <summary>Affiche le récapitulatif des mesures dans la console de logs.</summary>
    private void DisplayResults(TestResult r)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine(" ╔══════════════════════════════════════╗");
        sb.AppendLine($" ║  RÉSULTATS DE LA MESURE              ║");
        sb.AppendLine($" ║  Upload   : {r.Upload,10:F2} Mbps          ║");
        sb.AppendLine($" ║  Download : {r.Download,10:F2} Mbps          ║");
        sb.AppendLine(" ╚══════════════════════════════════════╝");
        txtLog.AppendText(sb.ToString());
    }
}
