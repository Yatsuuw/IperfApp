using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    // ---------------------------------------------------------------
    // Constantes de mise en page de la carte
    // ---------------------------------------------------------------
    private const int RowLabelW   = 90;   // largeur du label
    private const int RowInputW   = 270;  // largeur du champ / combo
    private const int RowGap      = 12;   // espace label → champ
    private const int RowPaddingL = 35;   // marge gauche
    // Total occupé : 35 + 90 + 12 + 270 = 407 px  →  marge droite = 480 - 407 = 73 px

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
                AppColors.CardBorder, ButtonBorderStyle.Solid);

        int top = 20;

        AddCardComboRow(pnlCard, "Profil",    out cbPresets,   ref top, isAccent: true);
        AddCardTextRow (pnlCard, "Serveur",   out txtServer,   isNumeric: false, ref top);
        AddCardTextRow (pnlCard, "Port",      out txtPort,     isNumeric: true,  ref top);
        AddCardTextRow (pnlCard, "Canaux",    out txtChannels, isNumeric: true,  ref top);
        AddCardComboRow(pnlCard, "Protocole", out cbIpVersion, ref top, isAccent: true);

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
        Panel card, string label,
        out TextBox field,
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
            ForeColor   = AppColors.FieldText
        };

        var line = new Panel
        {
            Left      = field.Left,
            Top       = field.Bottom + lineGap,
            Width     = RowInputW,
            Height    = 1,
            BackColor = AppColors.FieldBorder
        };

        field.Enter += (_, _) => line.BackColor = AppColors.FieldBorderFocus;
        field.Leave += (_, _) => line.BackColor = AppColors.FieldBorder;

        if (isNumeric)
            field.KeyPress += (_, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            };

        card.Controls.AddRange([lbl, field, line]);
        top += rowStep;
    }

    // ---------------------------------------------------------------
    // Helper : ligne label + ComboBox encadrée
    // ---------------------------------------------------------------

    private void AddCardComboRow(
        Panel card, string label,
        out ComboBox combo,
        ref int top,
        bool isAccent)
    {
        const int rowH    = 24;
        const int borderW = 1;
        const int rowStep = 52;

        var lbl = new Label
        {
            Text      = label + " :",
            Left      = RowPaddingL,
            Top       = top,
            Width     = RowLabelW,
            Height    = rowH,
            Font      = _fonts.Track(new Font("Segoe UI Semibold", 9.5F)),
            ForeColor = isAccent ? AppColors.Accent : AppColors.TextMuted,
            TextAlign = ContentAlignment.MiddleRight
        };

        // Créer le ComboBox en premier pour lire sa hauteur réelle imposée par WinForms.
        // Dimensionner le wrapper AVANT reviendrait à un cadre trop grand ou trop petit
        // selon le DPI et la fonte.
        combo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle     = FlatStyle.Flat,
            Font          = _fonts.Track(new Font("Segoe UI", 10F)),
            Width         = RowInputW,
            Left          = borderW,
            Top           = borderW,
            // Neutralise la marge WinForms par défaut (3 px) qui décalerait
            // le rendu du ComboBox à l'intérieur du wrapper selon le DPI.
            Margin        = Padding.Empty
        };

        // Hauteur réelle du ComboBox (imposée par WinForms selon la fonte).
        int comboH = combo.Height;

        // Wrapper calibré exactement sur le ComboBox + bordure 1 px tout autour.
        Color borderColor = AppColors.FieldBorder;
        var wrapper = new Panel
        {
            Left      = RowPaddingL + RowLabelW + RowGap,
            Top       = top + (rowH - comboH) / 2,
            Width     = RowInputW + borderW * 2,
            Height    = comboH    + borderW * 2,
            BackColor = AppColors.Card
        };

        wrapper.Paint += (_, e) =>
        {
            using var pen = new System.Drawing.Pen(borderColor, borderW);
            e.Graphics.DrawRectangle(pen,
                0, 0,
                wrapper.Width  - borderW,
                wrapper.Height - borderW);
        };

        combo.Enter += (_, _) => { borderColor = AppColors.FieldBorderFocus; wrapper.Invalidate(); };
        combo.Leave += (_, _) => { borderColor = AppColors.FieldBorder;      wrapper.Invalidate(); };

        wrapper.Controls.Add(combo);
        card.Controls.AddRange([lbl, wrapper]);
        top += rowStep;
    }
}
