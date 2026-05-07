using System.Reflection;

namespace IperfApp.UI;

public partial class Form1 : Form
{
  private void ShowAboutBox()
  {
    Assembly asm = Assembly.GetExecutingAssembly();

    Form ab = new()
    {
      Text            = "Informations",
      Size            = new Size(420, 420),
      MaximizeBox     = false,
      MinimizeBox     = false,
      StartPosition   = FormStartPosition.CenterParent,
      FormBorderStyle = FormBorderStyle.FixedDialog,
      BackColor       = Color.White,
      Icon            = Icon
    };

    // Icône centrée—le Bitmap produit par ToBitmap() est pris en charge
    // par PictureBox.Image et libéré à la fermeture de la fenêtre.
    if (Icon is not null)
    {
      var pb = new PictureBox
      {
        Image    = Icon.ToBitmap(),   // Bitmap: sera Disposed par pb
        SizeMode = PictureBoxSizeMode.Zoom,
        Size     = new Size(70, 70),
        Top      = 30
      };
      pb.Left = (ab.ClientSize.Width - pb.Width) / 2;
      ab.Controls.Add(pb);
    }

    Label info = new()
    {
      Top       = 115,
      Left      = 20,
      Width     = ab.ClientSize.Width - 40,
      Height    = 200,
      AutoSize  = false,
      TextAlign = ContentAlignment.TopCenter,
      Font      = new Font("Segoe UI", 10),
      Text      =
        $"Speedtest Iperf\n\n" +
        $"Version : {asm.GetName().Version}\n\n" +
        $"Conception : {asm.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company}\n\n" +
        $"{asm.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description}\n\n" +
        $"{asm.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright}"
    };

    Button ok = new()
    {
      Text      = "Fermer",
      Width     = 120,
      Height    = 40,
      Top       = ab.ClientSize.Height - 65,
      FlatStyle = FlatStyle.Flat,
      BackColor = _colorAccent,
      ForeColor = Color.White,
      Cursor    = Cursors.Hand,
      Font      = new Font("Segoe UI Semibold", 9)
    };
    ok.FlatAppearance.BorderSize = 0;
    ok.Left    = (ab.ClientSize.Width - ok.Width) / 2;
    ok.Click  += (_, _) => ab.Close();

    ab.Controls.AddRange([info, ok]);
    ab.ShowDialog();
  }
}
