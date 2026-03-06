namespace IperfApp.UI;

public partial class Form1 : Form
{
  private static TextBox AddModernInput(Panel p, ref int top, string lblT, string text, string placeholder, int x, int lW, int iW, int g)
  {
    // Créer le label
    var lbl = new Label
    { 
      Text = lblT, 
      Top = top + 3, 
      Left = x, 
      Width = lW, 
      Font = new Font("Segoe UI Semibold", 10F), 
      TextAlign = ContentAlignment.MiddleRight, 
      ForeColor = Color.Gray 
    };

    // Créer la textbox
    var txt = new TextBox
    { 
      Text = text, 
      PlaceholderText = placeholder, 
      Top = top, 
      Left = x + lW + g, 
      Width = iW, 
      Font = new Font("Segoe UI", 11F), 
      BorderStyle = BorderStyle.FixedSingle 
    };

    // Ajouter les contrôles au panel
    p.Controls.AddRange([lbl, txt]);

    // Incrémenter la position pour le prochain champ
    top += 50;

    return txt;
  }

  // Bouton discret
  private Button CreateGhostButton(string txt, int t, int x, int w)
  {
    var b = new Button
    { 
      Text = txt, 
      Top = t, 
      Left = x, 
      Width = w, 
      Height = 45, 
      FlatStyle = FlatStyle.Flat, 
      BackColor = _colorCard, 
      Enabled = false, 
      Font = new Font("Segoe UI", 9F) 
    };

    b.FlatAppearance.BorderColor = Color.FromArgb(210, 220, 230);

    return b;
  }
}
