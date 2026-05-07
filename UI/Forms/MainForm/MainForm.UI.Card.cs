using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    // ---------------------------------------------------------------
    // Constantes de mise en page de la carte
    // ---------------------------------------------------------------
    private const int RowLabelW   = 90;   // largeur du label
    private const int RowInputW   = 270;  // largeur du champ / combo (marge droite = 30px minimum)
    private const int RowGap      = 12;   // espace label → champ
    private const int RowPaddingL = 35;   // marge gauche
    // Total occupé : 35 + 90 + 12 + 270 = 407 px  →  marge droite = 480 - 407 = 73 px

    /// <summary>
    /// Construit la carte de configuration centrale.
    /// Chaque ligne est alignée : label (droite) | champ (gauche) sur la même baseline.
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
                Color.FromArgb(220, 225, 232), ButtonBorderStyle.Solid);

        int top = 20;

        // ---- Ligne : Profil ----
        AddCardComboRow(pnlCard, "Profil",    ref cbPresets,   ref top, isAccent: true);

        // ---- Lignes texte / numérique ----
        AddCardTextRow(pnlCard, "Serveur",    ref txtServer,   isNumeric: false, ref top);
        AddCardTextRow(pnlCard, "Port",       ref txtPort,     isNumeric: true,  ref top);
        AddCardTextRow(pnlCard, "Canaux",     ref txtChannels, isNumeric: true,  ref top);

        // ---- Ligne : Protocole IP ----
        AddCardComboRow(pnlCard, "Protocole", ref cbIpVersion, ref top, isAccent: false);
        cbIpVersion.Items.AddRange(["Auto (défaut)", "IPv4 (-4)", "IPv6 (-6)"]);
        cbIpVersion.SelectedIndex = 0;
        _mainToolTip.SetToolTip(cbIpVersion,
            "Force le protocole IP utilisé par iperf3 (-4, -6, ou auto)");

        return pnlCard;
    }

    // ---------------------------------------------------------------
    // Helper : ligne label + TextBox + soulignement
    // ---------------------------------------------------------------

    private void AddCardTextRow(
        Panel card,
        string label,
        ref TextBox field,
        bool isNumeric,
        ref int top)
    {
        const int rowH    = 24;
        const int lineGap = 3;
        const int rowStep = 52;

        var lbl = new Label
        {
            Text      = label + " :",
            Left      = RowPaddingL,
            Top       = top,
            Width     = RowLabelW,
            Height    = rowH,
            Font      = _fonts.Track(new Font("Segoe UI Semibold", 9.5F)),
            ForeColor = AppColors.Accent,
            TextAlign = ContentAlignment.MiddleRight
        };

        field = new TextBox
        {
            Left        = RowPaddingL + RowLabelW + RowGap,
            Top         = top + (rowH - 18) / 2,
            Width       = RowInputW,
            Height      = 20,
            Font        = _fonts.Track(new Font("Segoe UI", 10.5F)),
            BorderStyle = BorderStyle.None,
            ForeColor   = Color.FromArgb(30, 30, 30)
        };

        var line = new Panel
        {
            Left      = field.Left,
            Top       = field.Bottom + lineGap,
            Width     = RowInputW,
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
        top += rowStep;
    }

    // ---------------------------------------------------------------
    // Helper : ligne label + ComboBox
    // ---------------------------------------------------------------

    private void AddCardComboRow(
        Panel card,
        string label,
        ref ComboBox combo,
        ref int top,
        bool isAccent)
    {
        const int rowH    = 24;
        const int rowStep = 50;

        var lbl = new Label
        {
            Text      = label + " :",
            Left      = RowPaddingL,
            Top       = top,
            Width     = RowLabelW,
            Height    = rowH,
            Font      = _fonts.Track(new Font("Segoe UI Semibold", 9.5F)),
            ForeColor = isAccent ? AppColors.Accent : Color.DimGray,
            TextAlign = ContentAlignment.MiddleRight
        };

        combo = new ComboBox
        {
            Left          = RowPaddingL + RowLabelW + RowGap,
            Top           = top + (rowH - 22) / 2,
            Width         = RowInputW,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = _fonts.Track(new Font("Segoe UI", 10F)),
            FlatStyle     = FlatStyle.Flat
        };

        card.Controls.AddRange([lbl, combo]);
        top += rowStep;
    }
}
