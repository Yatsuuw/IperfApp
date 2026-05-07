using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>
    /// Construit la zone d'actions (boutons Lancer / Annuler, console logs, exports CSV).
    /// </summary>
    private void BuildActionsArea(int cardLeft)
    {
        const int spacer     = 14;
        const int btnMainH   = 52;
        const int btnCancelH = 32;
        const int logH       = 180;

        int topStart  = CardBottom + spacer;
        int topCancel = topStart  + btnMainH   + 6;
        int topLog    = topCancel + btnCancelH + spacer;
        int topExport = topLog    + logH       + 8;
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
            Font      = _fonts.Track(new Font("Segoe UI Semibold", 11.5F, FontStyle.Bold)),
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
            Font      = _fonts.Track(new Font("Segoe UI Semibold", 9.5F)),
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
        btnExportNew    = CreateGhostButton("Nouveau rapport",    topExport, cardLeft,                 btnHalfW);
        btnExportAppend = CreateGhostButton("Ajouter au fichier", topExport, cardLeft + btnHalfW + 10, btnHalfW);
        btnExportNew.Click    += (_, _) => HandleSave(false);
        btnExportAppend.Click += (_, _) => HandleSave(true);
    }

    /// <summary>
    /// Crée un bouton "ghost" (fond transparent, bordure légère) pour les actions secondaires.
    /// </summary>
    private Button CreateGhostButton(string text, int top, int left, int width)
    {
        var btn = new Button
        {
            Text      = text,
            Top       = top,
            Left      = left,
            Width     = width,
            Height    = 34,
            BackColor = Color.Transparent,
            ForeColor = AppColors.Accent,
            FlatStyle = FlatStyle.Flat,
            Font      = _fonts.Track(new Font("Segoe UI", 9F)),
            Cursor    = Cursors.Hand,
            Enabled   = false
        };
        btn.FlatAppearance.BorderColor = AppColors.ExportBorderDisabled;
        btn.FlatAppearance.BorderSize  = 1;
        return btn;
    }
}
