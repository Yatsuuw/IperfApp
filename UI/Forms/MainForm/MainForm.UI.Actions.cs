using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    private void BuildActionsArea()
    {
        const int spacer     = 15;
        const int btnMainH   = 50;
        const int btnCancelH = 30;
        const int logH       = 170;

        int topStart  = CardBottom + spacer;
        int topCancel = topStart  + btnMainH   + spacer / 3;
        int topLog    = topCancel + btnCancelH + spacer;
        int topExport = topLog   + logH        + spacer / 3;

        int left     = CardLeft;
        int btnHalfW = (CardWidth / 2) - 5;

        // --- Bouton Lancer ---
        btnStart = new Button
        {
            Text      = "LANCER L'ANALYSE",
            Top       = topStart,
            Left      = left,
            Width     = CardWidth,
            Height    = btnMainH,
            BackColor = AppColors.Accent,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
            Cursor    = Cursors.Hand
        };
        btnStart.FlatAppearance.BorderSize = 0;
        btnStart.Click += async (_, _) => await RunFullTest();

        // --- Bouton Annuler ---
        btnCancel = new Button
        {
            Text      = "ANNULER",
            Top       = topCancel,
            Left      = left,
            Width     = CardWidth,
            Height    = btnCancelH,
            BackColor = AppColors.Danger,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI Semibold", 9F),
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
            Left        = left,
            Width       = CardWidth,
            Height      = logH,
            BackColor   = AppColors.Terminal,
            ForeColor   = Color.FromArgb(220, 220, 220),
            Font        = new Font("Consolas", 9F),
            BorderStyle = BorderStyle.None
        };

        // --- Boutons Export CSV ---
        btnExportNew    = CreateGhostButton("Nouveau rapport",    topExport, left,                 btnHalfW);
        btnExportAppend = CreateGhostButton("Ajouter au fichier", topExport, left + btnHalfW + 10, btnHalfW);
        btnExportNew.Click    += (_, _) => HandleSave(false);
        btnExportAppend.Click += (_, _) => HandleSave(true);
    }
}
