using System.Reflection;

namespace IperfApp.UI;

public partial class Form1 : Form
{
  // Boîte de dialogue "À propos"
  private void ShowAboutBox()
  {
    // Fenêtre Informations
    Assembly asm = Assembly.GetExecutingAssembly();
    Form ab = new()
    { 
      Text = "Informations", 
      Size = new Size(420, 420), 
      MaximizeBox = false, 
      StartPosition = FormStartPosition.CenterParent, 
      FormBorderStyle = FormBorderStyle.FixedDialog, 
      BackColor = Color.White 
    };

    // Label
    Label info = new()
    { 
      Top = 120, 
      Left = 20, 
      Width = 380, 
      Height = 180, 
      TextAlign = ContentAlignment.TopCenter, 
      Font = new Font("Segoe UI", 10),
      Text = $"Speedtest Iperf\n\nVersion : {asm.GetName().Version}\n\nConception : {asm.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company}\n\n{asm.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description}\n\n{asm.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright}" 
    };

    // Bouton fermer
    Button ok = new()
    { 
      Text = "Fermer", 
      Width = 110, 
      Height = 40, 
      Top = 320, 
      FlatStyle = FlatStyle.Flat, 
      BackColor = _colorAccent, 
      ForeColor = Color.White 
    };
    ok.Left = (ab.ClientSize.Width - ok.Width) / 2;
    ok.Click += (s, e) => ab.Close();

    // Icône
    if (Icon != null) {
      PictureBox pb = new()
      { 
        Image = this.Icon.ToBitmap(), 
        SizeMode = PictureBoxSizeMode.Zoom, 
        Size = new Size(64, 64), 
        Top = 30 
      };
      pb.Left = (ab.ClientSize.Width - pb.Width) / 2;
      ab.Controls.Add(pb);
    }

    // Contrôles
    ab.Controls.AddRange([info, ok]);

    // Afficher la boîte de dialogue
    ab.ShowDialog();
  }
}
