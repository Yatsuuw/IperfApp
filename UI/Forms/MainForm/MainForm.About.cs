using IperfApp.UI.Forms.AboutForm;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
  private void ShowAboutBox()
  {
    using var about = new AboutDialog(Icon);
    about.ShowDialog(this);
  }
}
