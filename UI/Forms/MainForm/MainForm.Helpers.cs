using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
  private Button CreateGhostButton(string text, int top, int left, int width)
  {
    var btn = new Button
    {
      Text = text,
      Top = top,
      Left = left,
      Width = width,
      Height = 45,
      FlatStyle = FlatStyle.Flat,
      BackColor = AppColors.Card,
      Enabled = false,
      Font = _fonts.Track(new Font(AppFonts.Name, AppFonts.SizeBase)),
      Cursor = Cursors.Hand
    };
    btn.FlatAppearance.BorderColor = AppColors.ExportBorderDisabled;
    return btn;
  }
}
