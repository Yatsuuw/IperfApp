using IperfApp.UI.Constants;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>
    /// Construit la carte de configuration centrale.
    /// <paramref name="cardLeft"/> est fourni par <c>SetupModernUI</c> pour garantir
    /// la cohérence de position entre la carte et la zone d'actions.
    /// </summary>
    private Panel BuildConfigCard(int cardLeft)
    {
        var pnlCard = new Panel
        {
            BackColor = AppColors.Card,
            Size      = new Size(CardWidth, CardHeight),
            Location  = new Point(cardLeft, CardTop)
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
        cbPresets = new ComboBox
        {
            Top           = top,
            Left          = rowX + labelW + gap,
            Width         = inputW - 45,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = _fonts.Track(new Font("Segoe UI", 10F))
        };
        var lblPreset = new Label
        {
            Text      = "Profil :",
            Top       = top + 3,
            Left      = rowX,
            Width     = labelW,
            Font      = _fonts.Track(new Font("Segoe UI Semibold", 9F)),
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = AppColors.Accent
        };
        pnlCard.Controls.AddRange([lblPreset, cbPresets]);
        top += 45;

        // Champs texte / numérique — helpers partagés
        txtServer   = FormBuilderHelpers.AddInputField  (pnlCard, "Serveur :",   new TextBox(), ref top, _fonts);
        txtPort     = FormBuilderHelpers.AddNumericField(pnlCard, "Port :",      new TextBox(), ref top, _fonts);
        txtChannels = FormBuilderHelpers.AddNumericField(pnlCard, "Canaux :",    new TextBox(), ref top, _fonts);

        // Ligne : Protocole IP
        cbIpVersion = new ComboBox
        {
            Top           = top,
            Left          = rowX + labelW + gap,
            Width         = inputW,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = _fonts.Track(new Font("Segoe UI", 10F))
        };
        var lblIpVersion = new Label
        {
            Text      = "Protocole :",
            Top       = top + 3,
            Left      = rowX,
            Width     = labelW,
            Font      = _fonts.Track(new Font("Segoe UI Semibold", 9F)),
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = Color.DimGray
        };
        cbIpVersion.Items.AddRange(["Auto (défaut)", "IPv4 (-4)", "IPv6 (-6)"]);
        cbIpVersion.SelectedIndex = 0;
        _mainToolTip.SetToolTip(cbIpVersion, "Force le protocole IP utilisé par iperf3 (-4, -6, ou auto)");
        pnlCard.Controls.AddRange([lblIpVersion, cbIpVersion]);

        return pnlCard;
    }
}
