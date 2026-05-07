using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    private Panel BuildConfigCard()
    {
        var pnlCard = new Panel
        {
            BackColor = AppColors.Card,
            Size      = new Size(CardWidth, CardHeight),
            Location  = new Point(CardLeft, CardTop)
        };

        pnlCard.Paint += (_, e) =>
            ControlPaint.DrawBorder(e.Graphics, pnlCard.ClientRectangle,
                Color.FromArgb(230, 235, 240), ButtonBorderStyle.Solid);

        int top          = 20;
        const int labelW = 100;
        const int inputW = 280;
        const int gap    = 15;
        int rowX         = (CardWidth - (labelW + gap + inputW)) / 2;

        // Ligne : Profil
        var lblPreset = new Label
        {
            Text      = "Profil :",
            Top       = top + 3,
            Left      = rowX,
            Width     = labelW,
            Font      = new Font("Segoe UI Semibold", 9F),
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = AppColors.Accent
        };
        cbPresets = new ComboBox
        {
            Top           = top,
            Left          = rowX + labelW + gap,
            Width         = inputW - 45,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = new Font("Segoe UI", 10F)
        };
        pnlCard.Controls.AddRange([lblPreset, cbPresets]);
        top += 45;

        txtServer   = AddModernInput (pnlCard, ref top, "Serveur :",  "", "Adresse du serveur", rowX, labelW, inputW, gap);
        txtPort     = AddNumericInput(pnlCard, ref top, "Port :",     "", "5201",               rowX, labelW, inputW, gap);
        txtChannels = AddNumericInput(pnlCard, ref top, "Canaux :",   "", "8",                  rowX, labelW, inputW, gap);

        // Ligne : Protocole IP
        var lblIpVersion = new Label
        {
            Text      = "Protocole :",
            Top       = top + 3,
            Left      = rowX,
            Width     = labelW,
            Font      = new Font("Segoe UI Semibold", 9F),
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = Color.DimGray
        };
        cbIpVersion = new ComboBox
        {
            Top           = top,
            Left          = rowX + labelW + gap,
            Width         = inputW,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = new Font("Segoe UI", 10F)
        };
        cbIpVersion.Items.AddRange(["Auto (défaut)", "IPv4 (-4)", "IPv6 (-6)"]);
        cbIpVersion.SelectedIndex = 0;
        _mainToolTip.SetToolTip(cbIpVersion, "Force le protocole IP utilisé par iperf3 (-4, -6, ou auto)");
        pnlCard.Controls.AddRange([lblIpVersion, cbIpVersion]);

        return pnlCard;
    }
}
