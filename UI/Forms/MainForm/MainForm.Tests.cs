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

        // Note : si la validation échoue (return ci-dessus), SetTestRunningState(true)
        // n'est jamais appelé. _testRunning reste false et le finally appelle
        // SetTestRunningState(false) de façon inoffensive : btnStart est déjà actif,
        // _testRunning déjà false. Pas de garde supplémentaire nécessaire.
        // Le double-clic ne peut pas déclencher deux tests simultanés car btnStart
        // est désactivé dès l'entrée dans SetTestRunningState(true).

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
            MessageBox.Show(this, $"Erreur inattendue : {ex.Message}", "Erreur",
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
    /// Les valeurs de port et de canaux sont lues depuis les TextBox sans fallback silencieux :
    /// si la valeur saisie est invalide (non numérique, hors plage), <see cref="Preset.Validate"/>
    /// retournera un message d'erreur explicite affiché à l'utilisateur.
    /// </para>
    /// <para>
    /// <b>Design choice — Durée :</b> la carte de configuration n'expose pas de champ
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
            Port      = port,
            Channels  = channels,
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

    /// <summary>
    /// Affiche le récapitulatif des mesures dans la console de logs.
    /// Cadre Unicode à largeur dynamique, avec plancher à 26 caractères internes
    /// pour garantir que le titre "RÉSULTATS DE LA MESURE" (24 car.) ne déborde
    /// jamais et éviter toute <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    private void DisplayResults(TestResult r)
    {
        const string labelUp   = "  Upload   : ";
        const string labelDown = "  Download : ";
        const int    labelW    = 13;
        // Titre = 22 caractères + 2 espaces de marge = 24 caractères minimum.
        // Plancher à 26 pour laisser 1 espace de respiration de chaque côté.
        const int    titleMinW = 26;

        string up   = DisplayHelpers.FormatMbps(r.Upload);
        string down = DisplayHelpers.FormatMbps(r.Download);

        int valueW = Math.Max(up.Length, down.Length);
        int innerW = Math.Max(labelW + valueW + 2, titleMinW);
        string sep = new string('\u2500', innerW);

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($" \u250c{sep}\u2510");
        sb.AppendLine($" \u2502  R\u00c9SULTATS DE LA MESURE{new string(' ', innerW - 24)}\u2502");
        sb.AppendLine($" \u251c{sep}\u2524");
        sb.AppendLine($" \u2502{labelUp}{up.PadLeft(valueW)}  \u2502");
        sb.AppendLine($" \u2502{labelDown}{down.PadLeft(valueW)}  \u2502");
        sb.AppendLine($" \u2514{sep}\u2518");
        txtLog.AppendText(sb.ToString());
    }
}
