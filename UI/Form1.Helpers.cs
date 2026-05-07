namespace IperfApp.UI;

public partial class Form1 : Form
{
  /// <summary>
  /// Ajoute une ligne label + TextBox à un <see cref="Panel"/> et retourne le TextBox créé.
  /// </summary>
  /// <param name="p">Panel conteneur.</param>
  /// <param name="top">Position verticale courante (incrémentée de 42 après l'appel).</param>
  /// <param name="lblT">Texte du label.</param>
  /// <param name="text">Valeur initiale du TextBox.</param>
  /// <param name="placeholder">Texte indicatif affiché quand le champ est vide.</param>
  /// <param name="x">Position X de départ.</param>
  /// <param name="lW">Largeur du label.</param>
  /// <param name="iW">Largeur du TextBox.</param>
  /// <param name="g">Espace entre le label et le TextBox.</param>
  private static TextBox AddModernInput(
    Panel p, ref int top, string lblT, string text, string placeholder,
    int x, int lW, int iW, int g)
  {
    var lbl = new Label
    {
      Text      = lblT,
      Top       = top + 3,
      Left      = x,
      Width     = lW,
      Font      = new Font("Segoe UI Semibold", 9F),
      TextAlign = ContentAlignment.MiddleRight,
      ForeColor = Color.DimGray
    };

    var txt = new TextBox
    {
      Text            = text,
      PlaceholderText = placeholder,
      Top             = top,
      Left            = x + lW + g,
      Width           = iW,
      Font            = new Font("Segoe UI", 10F),
      BorderStyle     = BorderStyle.FixedSingle
    };

    p.Controls.AddRange([lbl, txt]);
    top += 42;
    return txt;
  }

  /// <summary>
  /// Ajoute une ligne label + TextBox numérique (chiffres uniquement).
  /// Délègue à <see cref="AddModernInput"/> puis attache un filtre <c>KeyPress</c>.
  /// </summary>
  private static TextBox AddNumericInput(
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
  /// Crée un bouton "ghost" (contour seulement, fond blanc, désactivé par défaut).
  /// Utilisé pour les boutons d'export CSV.
  /// </summary>
  private Button CreateGhostButton(string txt, int t, int x, int w)
  {
    var b = new Button
    {
      Text      = txt,
      Top       = t,
      Left      = x,
      Width     = w,
      Height    = 45,
      FlatStyle = FlatStyle.Flat,
      BackColor = _colorCard,
      Enabled   = false,
      Font      = new Font("Segoe UI", 9F),
      Cursor    = Cursors.Hand
    };
    b.FlatAppearance.BorderColor = Color.FromArgb(210, 220, 230);
    return b;
  }
}
