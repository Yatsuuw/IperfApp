using System.Text;
using IperfApp.Models;
using IperfApp.UI.Constants;
using IperfApp.UI.Helpers;

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

        _lastPreset = BuildCurrentPreset();

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
            MessageBox.Show($"Erreur inattendue : {ex.Message}", "Erreur",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
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

    /// <summary>
    /// Formate un débit en Mbps vers la bonne unité lisible.
    /// Réutilisable depuis d'autres méthodes (ex : export, clipboard).
    /// </summary>
    internal static string FormatMbps(double mbps) => mbps switch
    {
        >= 1000 => $"{mbps / 1000.0:F2} Gbps",
        >= 1    => $"{mbps:F2} Mbps",
        _       => $"{mbps * 1000.0:F1} Kbps"
    };

    /// <summary>
    /// Affiche le récapitulatif des mesures dans la console de logs.
    /// Cadre ASCII à largeur dynamique : robuste à toutes les magnitudes.
    /// </summary>
    private void DisplayResults(TestResult r)
    {
        string up   = FormatMbps(r.Upload);
        string down = FormatMbps(r.Download);

        int valueWidth = Math.Max(up.Length, down.Length);
        string sep     = new string('─', 24 + valueWidth);

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($" ┌{sep}┐");
        sb.AppendLine($" │  RÉSULTATS DE LA MESURE{new string(' ', valueWidth)}  │");
        sb.AppendLine($" ├{sep}┤");
        sb.AppendLine($" │  Upload   : {up.PadLeft(valueWidth)}          │");
        sb.AppendLine($" │  Download : {down.PadLeft(valueWidth)}          │");
        sb.AppendLine($" └{sep}┘");
        txtLog.AppendText(sb.ToString());
    }
}
