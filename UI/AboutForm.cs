using System.Reflection;

namespace IperfApp.UI;

/// <summary>
/// Boîte de dialogue "Informations" affichant les métadonnées
/// de l'assembly (version, auteur, description, copyright).
/// </summary>
internal sealed class AboutForm : Form
{
  /// <summary>Initialise la fenêtre « Informations ».</summary>
  /// <param name="parentIcon">Icône de la fenêtre parente, reprise pour la boîte.</param>
  public AboutForm(Icon? parentIcon)
  {
    Icon = parentIcon;
    SetupUI();
  }

  // ---------------------------------------------------------------
  // Construction de l'interface
  // ---------------------------------------------------------------

  private void SetupUI()
  {
    Assembly asm = Assembly.GetExecutingAssembly();

    Text            = "Informations";
    Size            = new Size(420, 420);
    MaximizeBox     = false;
    MinimizeBox     = false;
    StartPosition   = FormStartPosition.CenterParent;
    FormBorderStyle = FormBorderStyle.FixedDialog;
    BackColor       = Color.White;

    // --- Icône centrée ---
    if (Icon is not null)
    {
      var pb = new PictureBox
      {
        Image    = Icon.ToBitmap(),
        SizeMode = PictureBoxSizeMode.Zoom,
        Size     = new Size(70, 70),
        Top      = 30
      };
      pb.Left = (ClientSize.Width - pb.Width) / 2;
      Controls.Add(pb);
    }

    // --- Informations texte ---
    var info = new Label
    {
      Top       = 115,
      Left      = 20,
      Width     = ClientSize.Width - 40,
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

    // --- Bouton Fermer ---
    var ok = new Button
    {
      Text      = "Fermer",
      Width     = 120,
      Height    = 40,
      Top       = ClientSize.Height - 65,
      FlatStyle = FlatStyle.Flat,
      BackColor = Color.FromArgb(0, 120, 215),
      ForeColor = Color.White,
      Cursor    = Cursors.Hand,
      Font      = new Font("Segoe UI Semibold", 9)
    };
    ok.FlatAppearance.BorderSize = 0;
    ok.Left   = (ClientSize.Width - ok.Width) / 2;
    ok.Click += (_, _) => Close();

    Controls.AddRange([info, ok]);
  }
}
