using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>
    /// Construit la zone d'actions (boutons Lancer / Annuler, console logs, exports CSV).
    /// <paramref name="cardLeft"/> est fourni par <c>SetupModernUI</c> — même valeur
    /// que celle passée à <see cref="BuildConfigCard"/> pour garantir l'alignement.
    /// </summary>
    private void BuildActionsArea(int cardLeft)
    {
        const int spacer     = 15;
        const int btnMainH   = 50;
        const int btnCancelH = 30;
        const int logH       = 170;

        int topStart  = CardBottom + spacer;
        int topCancel = topStart  + btnMainH   + (spacer / 3);
        int topLog    = topCancel + btnCancelH + spacer;
        int topExport = topLog   + logH        + (spacer / 3);
        int btnHalfW  = (CardWidth / 2) - 5;

        // --- Bouton Lancer ---
        btnStart = new Button
        {
            Text      = "LANCER L'ANALYSE",
            Top       = topStart,
            Left      = cardLeft,
            Width     = CardWidth,
            Height    = btnMainH,
            BackColor = AppColors.Accent,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = _fonts.Track(new Font("Segoe UI Semibold", 11F, FontStyle.Bold)),
            Cursor    = Cursors.Hand
        };
        btnStart.FlatAppearance.BorderSize = 0;
        btnStart.Click += async (_, _) => await RunFullTest();

        // --- Bouton Annuler ---
        btnCancel = new Button
        {
            Text      = "ANNULER",
            Top       = topCancel,
            Left      = cardLeft,
            Width     = CardWidth,
            Height    = btnCancelH,
            BackColor = AppColors.Danger,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = _fonts.Track(new Font("Segoe UI Semibold", 9F)),
            Cursor    = Cursors.Hand,
            Enabled   = false
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += (_, _) =>
        {
            _testCts?.Cancel();
            btnCancel.Enabled = false;
        };

        // --- Console de logs ---
        txtLog = new TextBox
        {
            Multiline   = true,
            ReadOnly    = true,
            ScrollBars  = ScrollBars.Vertical,
            Top         = topLog,
            Left        = cardLeft,
            Width       = CardWidth,
            Height      = logH,
            BackColor   = AppColors.Terminal,
            ForeColor   = Color.FromArgb(220, 220, 220),
            Font        = _fonts.Track(new Font("Consolas", 9F)),
            BorderStyle = BorderStyle.None
        };

        // --- Boutons Export CSV ---
        btnExportNew    = CreateGhostButton("Nouveau rapport",    topExport, cardLeft,                  btnHalfW);
        btnExportAppend = CreateGhostButton("Ajouter au fichier", topExport, cardLeft + btnHalfW + 10,  btnHalfW);
        btnExportNew.Click    += (_, _) => HandleSave(false);
        btnExportAppend.Click += (_, _) => HandleSave(true);
    }
}
