namespace IperfApp.UI;

public partial class Form1 : Form
{
  /// <summary>Ouvre la boîte de dialogue "Informations".</summary>
  private void ShowAboutBox()
  {
    using var about = new AboutForm(Icon);
    about.ShowDialog(this);
  }
}
