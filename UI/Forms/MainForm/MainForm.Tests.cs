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

    /// <summary>
    /// Construit un <see cref="Preset"/> depuis les champs de l'UI.
    /// <para>
    /// <b>Design choice — Durée :</b> la carte de configuration n'expose pas de champ
    /// <c>txtDuration</c> modifiable en temps réel. La durée est toujours lue depuis
    /// <c>selectedPreset.Duration</c> (valeur persistée dans le profil). Pour modifier
    /// la durée, l'utilisateur doit éditer le profil via la fenêtre Profils.
    /// </para>
    /// </summary>
    private Preset BuildCurrentPreset()
    {
        var selectedPreset = cbPresets.SelectedItem as Preset;

        _ = int.TryParse(txtPort.Text,     out int port);
        _ = int.TryParse(txtChannels.Text, out int channels);

        return new Preset
        {
            Name      = selectedPreset?.Name ?? "Temporaire",
            Server    = txtServer.Text.Trim(),
            Port      = port     > 0 ? port     : 5201,
            Channels  = channels > 0 ? channels : 8,
            Duration  = selectedPreset is { Duration: > 0 } p ? p.Duration : 10,
            IpVersion = IpVersionExtensions.FromComboIndex(cbIpVersion.SelectedIndex)
        };
    }

    /// <summary>
    /// Active ou désactive les contrôles selon l'état du test.
    /// Met à jour <see cref="_testRunning"/> comme source-of-truth.
    /// </summary>
    private void SetTestRunningState(bool running)
    {
        _testRunning = running;

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

    /// <summary>Formate un débit en Mbps vers la bonne unité lisible (Kbps / Mbps / Gbps).</summary>
    internal static string FormatMbps(double mbps) => mbps switch
    {
        >= 1000 => $"{mbps / 1000.0:F2} Gbps",
        >= 1    => $"{mbps:F2} Mbps",
        _       => $"{mbps * 1000.0:F1} Kbps"
    };

    /// <summary>
    /// Affiche le récapitulatif des mesures dans la console de logs.
    /// Cadre ASCII à largeur dynamique : robuste à toutes les magnitudes de débit.
    /// </summary>
    private void DisplayResults(TestResult r)
    {
        const string labelUp   = "  Upload   : ";
        const string labelDown = "  Download : ";
        const int    labelW    = 13;

        string up   = FormatMbps(r.Upload);
        string down = FormatMbps(r.Download);

        int valueW = Math.Max(up.Length, down.Length);
        int innerW = labelW + valueW + 2;
        string sep = new string('─', innerW);

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($" ┌{sep}┐");
        sb.AppendLine($" │  RÉSULTATS DE LA MESURE{new string(' ', innerW - 24)}│");
        sb.AppendLine($" ├{sep}┤");
        sb.AppendLine($" │{labelUp}{up.PadLeft(valueW)}  │");
        sb.AppendLine($" │{labelDown}{down.PadLeft(valueW)}  │");
        sb.AppendLine($" └{sep}┘");
        txtLog.AppendText(sb.ToString());
    }
}
