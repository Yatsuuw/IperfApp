using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.AboutForm;

/// <summary>Boîte de dialogue "Informations" de l'application.</summary>
public class AboutForm : Form
{
    public AboutForm(Icon? parentIcon)
    {
        Text            = " Informations";
        Size            = new Size(360, 220);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition   = FormStartPosition.CenterParent;
        MaximizeBox     = false;
        MinimizeBox     = false;
        BackColor       = AppColors.Card;
        if (parentIcon is not null) Icon = parentIcon;

        var lblTitle = new Label
        {
            Text      = "IperfApp",
            Font      = new Font("Segoe UI Variable Display", 14F, FontStyle.Bold),
            ForeColor = AppColors.Accent,
            Location  = new Point(20, 20),
            AutoSize  = true
        };

        var lblVersion = new Label
        {
            Text      = $"Version {Application.ProductVersion}",
            Font      = new Font("Segoe UI", 9F),
            ForeColor = AppColors.TextMuted,
            Location  = new Point(20, 55),
            AutoSize  = true
        };

        var lblDesc = new Label
        {
            Text      = "Outil de mesure de débit réseau basé sur iperf3.",
            Font      = new Font("Segoe UI", 9F),
            ForeColor = AppColors.TextMuted,
            Location  = new Point(20, 80),
            AutoSize  = true
        };

        var btnClose = new Button
        {
            Text         = "Fermer",
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
}
