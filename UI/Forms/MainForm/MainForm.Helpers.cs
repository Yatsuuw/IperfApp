using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>
    /// Ajoute une ligne label + TextBox à un <see cref="Panel"/> et retourne le TextBox créé.
    /// Les <see cref="Font"/> créées sont enregistrées dans <see cref="_trackedFonts"/>.
    /// </summary>
    private TextBox AddModernInput(
        Panel p, ref int top, string lblT, string text, string placeholder,
        int x, int lW, int iW, int g)
    {
        var fLbl = new Font("Segoe UI Semibold", 9F);
        var lbl = new Label
        {
            Text      = lblT,
            Top       = top + 3,
            Left      = x,
            Width     = lW,
            Font      = fLbl,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = Color.DimGray
        };
        _trackedFonts.Add(fLbl);

        var fTxt = new Font("Segoe UI", 10F);
        var txt = new TextBox
        {
            Text            = text,
            PlaceholderText = placeholder,
            Top             = top,
            Left            = x + lW + g,
            Width           = iW,
            Font            = fTxt,
            BorderStyle     = BorderStyle.FixedSingle
        };
        _trackedFonts.Add(fTxt);

        p.Controls.AddRange([lbl, txt]);
        top += 42;
        return txt;
    }

    /// <summary>
    /// Ajoute une ligne label + TextBox numérique (chiffres uniquement).
    /// Délègue à <see cref="AddModernInput"/> puis attache un filtre <c>KeyPress</c>.
    /// </summary>
    private TextBox AddNumericInput(
        Panel p, ref int top, string lblT, string text, string placeholder,
        int x, int lW, int iW, int g)
    {
        var txt = AddModernInput(p, ref top, lblT, text, placeholder, x, lW, iW, g);
        txt.KeyPress += (_, e) =>
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        };
        return txt;
    }

    /// <summary>
    /// Crée un bouton « ghost » (contour, fond blanc, désactivé par défaut).
    /// La <see cref="Font"/> créée est enregistrée dans <see cref="_trackedFonts"/>.
    /// </summary>
    private Button CreateGhostButton(string txt, int t, int x, int w)
    {
        var f = new Font("Segoe UI", 9F);
        var b = new Button
        {
            Text      = txt,
            Top       = t,
            Left      = x,
            Width     = w,
            Height    = 45,
            FlatStyle = FlatStyle.Flat,
            BackColor = AppColors.Card,
            Enabled   = false,
            Font      = f,
            Cursor    = Cursors.Hand
        };
        b.FlatAppearance.BorderColor = Color.FromArgb(210, 220, 230);
        _trackedFonts.Add(f);
        return b;
    }
}
