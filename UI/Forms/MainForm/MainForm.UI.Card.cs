using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>
    /// Construit la carte de configuration centrale.
    /// Utilise un système de positionnement centré propre à la MainForm,
    /// indépendant de <c>FormBuilderHelpers</c> (prévu pour les panneaux full-width).
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

        // --- Constantes de layout ---
        const int labelW  = 100;
        const int inputW  = 260;
        const int gap     = 15;
        int       rowX    = (CardWidth - (labelW + gap + inputW)) / 2;
        int       top     = 18;

        // ---- Ligne : Profil ----
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
        cbPresets = new ComboBox
        {
            Top           = top,
            Left          = rowX + labelW + gap,
            Width         = inputW,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = _fonts.Track(new Font("Segoe UI", 10F))
        };
        pnlCard.Controls.AddRange([lblPreset, cbPresets]);
        top += 44;

        // ---- Ligne : Serveur ----
        AddCardRow(pnlCard, "Serveur :",  ref txtServer,   isNumeric: false, ref top, rowX, labelW, inputW, gap);
        // ---- Ligne : Port ----
        AddCardRow(pnlCard, "Port :",     ref txtPort,     isNumeric: true,  ref top, rowX, labelW, inputW, gap);
        // ---- Ligne : Canaux ----
        AddCardRow(pnlCard, "Canaux :",   ref txtChannels, isNumeric: true,  ref top, rowX, labelW, inputW, gap);

        // ---- Ligne : Protocole IP ----
        var lblIpVersion = new Label
        {
            Text      = "Protocole :",
            Top       = top + 5,
            Left      = rowX,
            Width     = labelW,
            Font      = _fonts.Track(new Font("Segoe UI Semibold", 9F)),
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = Color.DimGray
        };
        cbIpVersion = new ComboBox
        {
            Top           = top + 2,
            Left          = rowX + labelW + gap,
            Width         = inputW,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = _fonts.Track(new Font("Segoe UI", 10F))
        };
        cbIpVersion.Items.AddRange(["Auto (défaut)", "IPv4 (-4)", "IPv6 (-6)"]);
        cbIpVersion.SelectedIndex = 0;
        _mainToolTip.SetToolTip(cbIpVersion, "Force le protocole IP utilisé par iperf3 (-4, -6, ou auto)");
        pnlCard.Controls.AddRange([lblIpVersion, cbIpVersion]);

        return pnlCard;
    }

    // ---------------------------------------------------------------
    // Helper privé : une ligne label + champ texte + soulignement
    // ---------------------------------------------------------------

    /// <summary>
    /// Ajoute une ligne (label + TextBox + soulignement coloré) dans la carte.
    /// Utilise le même positionnement centré que les autres lignes de la carte.
    /// </summary>
    private void AddCardRow(
        Panel card,
        string label,
        ref TextBox field,
        bool   isNumeric,
        ref int top,
        int rowX, int labelW, int inputW, int gap)
    {
        var lbl = new Label
        {
            Text      = label,
            Top       = top,
            Left      = rowX,
            Width     = labelW,
            Height    = 18,
            Font      = _fonts.Track(new Font("Segoe UI", 7.5F, FontStyle.Bold)),
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = AppColors.Accent
        };

        field = new TextBox
        {
            Top         = top + 20,
            Left        = rowX + labelW + gap,
            Width       = inputW,
            Font        = _fonts.Track(new Font("Segoe UI Semibold", 9.5F)),
            BorderStyle = BorderStyle.None
        };

        var line = new Panel
        {
            Top       = field.Bottom + 3,
            Left      = rowX + labelW + gap,
            Width     = inputW,
            Height    = 1,
            BackColor = AppColors.FieldBorder
        };

        if (isNumeric)
        {
            field.KeyPress += (_, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            };
        }

        field.Enter += (_, _) => line.BackColor = AppColors.FieldBorderFocus;
        field.Leave += (_, _) => line.BackColor = AppColors.FieldBorder;

        card.Controls.AddRange([lbl, field, line]);
        top += 48;
    }
}
