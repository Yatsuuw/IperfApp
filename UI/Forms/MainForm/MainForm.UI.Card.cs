using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    // ---------------------------------------------------------------
    // Constantes de mise en page de la carte
    // ---------------------------------------------------------------
    private const int RowLabelW  = 90;   // largeur du label à gauche
    private const int RowInputW  = 290;  // largeur du champ / combo
    private const int RowGap     = 10;   // espace label → champ
    private const int RowPaddingL = 30;  // marge gauche de la carte

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

        int top = 18;

        // ---- Ligne : Profil ----
        AddCardComboRow(pnlCard, "Profil", ref cbPresets, ref top, isPreset: true);

        // ---- Lignes texte / numérique ----
        AddCardTextRow(pnlCard, "Serveur",  ref txtServer,   isNumeric: false, ref top);
        AddCardTextRow(pnlCard, "Port",     ref txtPort,     isNumeric: true,  ref top);
        AddCardTextRow(pnlCard, "Canaux",   ref txtChannels, isNumeric: true,  ref top);

        // ---- Ligne : Protocole IP ----
        AddCardComboRow(pnlCard, "Protocole", ref cbIpVersion, ref top, isPreset: false);
        cbIpVersion.Items.AddRange(["Auto (défaut)", "IPv4 (-4)", "IPv6 (-6)"]);
        cbIpVersion.SelectedIndex = 0;
        _mainToolTip.SetToolTip(cbIpVersion,
            "Force le protocole IP utilisé par iperf3 (-4, -6, ou auto)");

        return pnlCard;
    }

    // ---------------------------------------------------------------
    // Helpers privés
    // ---------------------------------------------------------------

    /// <summary>
    /// Ligne : label de texte + TextBox + soulignement.
    /// Label et champ sont sur la même ligne, verticalement centrés.
    /// </summary>
    private void AddCardTextRow(
        Panel card, string label,
        ref TextBox field,
        bool isNumeric,
        ref int top)
    {
        const int rowH    = 26;   // hauteur utile de la ligne
        const int lineGap = 3;    // espace entre le bas du champ et la ligne
        const int rowStep = 54;   // pas vertical total entre deux lignes

        // Label
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

        // Champ
        field = new TextBox
        {
            Left        = RowPaddingL + RowLabelW + RowGap,
            Top         = top + (rowH - 18) / 2,   // centré verticallement avec le label
            Width       = RowInputW,
            Height      = 20,
            Font        = _fonts.Track(new Font("Segoe UI", 10.5F)),
            BorderStyle = BorderStyle.None,
            ForeColor   = Color.FromArgb(30, 30, 30)
        };

        // Soulignement
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

    /// <summary>
    /// Ligne : label + ComboBox.
    /// <paramref name="isPreset"/> = true pour le ComboBox des profils
    /// (plus large, sans items pré-remplis).
    /// </summary>
    private void AddCardComboRow(
        Panel card, string label,
        ref ComboBox combo,
        ref int top,
        bool isPreset)
    {
        const int rowH    = 26;
        const int rowStep = 50;

        var lbl = new Label
        {
            Text      = label + " :",
            Left      = RowPaddingL,
            Top       = top,
            Width     = RowLabelW,
            Height    = rowH,
            Font      = _fonts.Track(new Font("Segoe UI Semibold", 9.5F)),
            ForeColor = isPreset ? AppColors.Accent : Color.DimGray,
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
