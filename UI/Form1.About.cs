using System.Reflection;

namespace IperfApp.UI;

public partial class Form1 : Form
{
  // Boîte de dialogue "À propos"
  private void ShowAboutBox()
  {
    Assembly asm = Assembly.GetExecutingAssembly();
    
    // 1. Fenêtre Informations
    Form ab = new()
    { 
      Text = "Informations", 
      Size = new Size(420, 420), 
      MaximizeBox = false,
      MinimizeBox = false, // Désactive le bouton réduire
      StartPosition = FormStartPosition.CenterParent, 
      FormBorderStyle = FormBorderStyle.FixedDialog, 
      BackColor = Color.White,
      Icon = Icon // <-- Place l'icône de l'app dans l'en-tête
    };

    // 2. Icône principale (PictureBox) au centre
    if (Icon != null) {
      PictureBox pb = new()
      { 
        Image = this.Icon.ToBitmap(), 
        SizeMode = PictureBoxSizeMode.Zoom, 
        Size = new Size(70, 70), // Taille légèrement augmentée
        Top = 35 
      };
      // Centrage horizontal dynamique
      pb.Left = (ab.ClientSize.Width - pb.Width) / 2;
      ab.Controls.Add(pb);
    }

    // 3. Label de texte centré
    Label info = new()
    { 
      Top = 130, // Positionné sous l'icône
      Width = 380, 
      Height = 180, 
      TextAlign = ContentAlignment.TopCenter, // Centre le texte à l'intérieur du label
      Font = new Font("Segoe UI", 10),
      Text = $"Speedtest Iperf\n\nVersion : {asm.GetName().Version}\n\nConception : {asm.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company}\n\n{asm.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description}\n\n{asm.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright}" 
    };
    // Centrage du label lui-même dans la fenêtre
    info.Left = (ab.ClientSize.Width - info.Width) / 2;

    // 4. Bouton fermer centré
    Button ok = new()
    { 
      Text = "Fermer", 
      Width = 120, 
      Height = 40, 
      Top = 325, 
      FlatStyle = FlatStyle.Flat, 
      BackColor = _colorAccent, 
      ForeColor = Color.White,
      Cursor = Cursors.Hand,
      Font = new Font("Segoe UI Semibold", 9)
    };
    ok.FlatAppearance.BorderSize = 0; // Retire la bordure pour le style moderne
    // Centrage du bouton dans la fenêtre
    ok.Left = (ab.ClientSize.Width - ok.Width) / 2;
    ok.Click += (s, e) => ab.Close();

    // 5. Ajout final des contrôles
    ab.Controls.AddRange([info, ok]);

    // Afficher la boîte de dialogue
    ab.ShowDialog();
  }
}