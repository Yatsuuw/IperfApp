using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.AboutForm;

/// <summary>Boîte de dialogue « Informations » de l'application.</summary>
public class AboutDialog : Form
{
    // Toutes les fonts allouées ici sont libérées dans Dispose(bool).
    private readonly Font _fontTitle;
    private readonly Font _fontBody;
    private readonly Font _fontBtn;

    public AboutDialog(Icon? parentIcon)
    {
        Text            = " Informations";
        Size            = new Size(360, 220);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition   = FormStartPosition.CenterParent;
        MaximizeBox     = false;
        MinimizeBox     = false;
        BackColor       = AppColors.Card;
        if (parentIcon is not null) Icon = parentIcon;

        _fontTitle = new Font("Segoe UI Variable Display", 14F, FontStyle.Bold);
        _fontBody  = new Font("Segoe UI", 9F);
        _fontBtn   = new Font("Segoe UI", 9F);

        var lblTitle = new Label
        {
            Text      = "IperfApp",
            Font      = _fontTitle,
            ForeColor = AppColors.Accent,
            Location  = new Point(20, 20),
            AutoSize  = true
        };

        var lblVersion = new Label
        {
            Text      = $"Version {Application.ProductVersion}",
            Font      = _fontBody,
            ForeColor = AppColors.TextMuted,
            Location  = new Point(20, 55),
            AutoSize  = true
        };

        var lblDesc = new Label
        {
            Text      = "Outil de mesure de débit réseau basé sur iperf3.",
            Font      = _fontBody,
            ForeColor = AppColors.TextMuted,
            Location  = new Point(20, 80),
            AutoSize  = true
        };

        var btnClose = new Button
        {
            Text         = "Fermer",
            Font         = _fontBtn,
            DialogResult = DialogResult.OK,
            Location     = new Point(240, 145),
            Size         = new Size(90, 30),
            FlatStyle    = FlatStyle.Flat,
            BackColor    = AppColors.Accent,
            ForeColor    = AppColors.Card,
            Cursor       = Cursors.Hand
        };
        btnClose.FlatAppearance.BorderSize = 0;

        Controls.AddRange([lblTitle, lblVersion, lblDesc, btnClose]);
        AcceptButton = btnClose;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fontTitle.Dispose();
            _fontBody.Dispose();
            _fontBtn.Dispose();
        }
        base.Dispose(disposing);
    }
}
