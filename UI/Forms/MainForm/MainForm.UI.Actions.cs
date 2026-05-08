using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
  private void BuildActionsArea(int cardLeft)
  {
    const int spacer = 14;
    const int btnMainH = 52;
    const int btnCancelH = 32;
    const int logH = 180;

    int topStart = CardBottom + spacer;
    int topCancel = topStart + btnMainH + 6;
    int topLog = topCancel + btnCancelH + spacer;
    int topExport = topLog + logH + 8;
    int btnHalfW = (CardWidth / 2) - 5;

    btnStart = new Button
    {
      Text = "LANCER L'ANALYSE",
      Top = topStart,
      Left = cardLeft,
      Width = CardWidth,
      Height = btnMainH,
      BackColor = AppColors.Accent,
      ForeColor = AppColors.CardTextWhite,
      FlatStyle = FlatStyle.Flat,
      Font = _fonts.Track(new Font(AppFonts.SemiBoldName, 11.5F, FontStyle.Bold)),
      Cursor = Cursors.Hand
    };
    btnStart.FlatAppearance.BorderSize = 0;
    btnStart.Click += async (_, _) =>
    {
      try   { await RunFullTest(); }
      catch (Exception ex) { Debug.WriteLine($"[btnStart] Exception non gérée : {ex}"); }
    };

    btnCancel = new Button
    {
      Text = "ANNULER",
      Top = topCancel,
      Left = cardLeft,
      Width = CardWidth,
      Height = btnCancelH,
      BackColor = AppColors.Danger,
      ForeColor = AppColors.CardTextWhite,
      FlatStyle = FlatStyle.Flat,
      Font = _fonts.Track(new Font(AppFonts.SemiBoldName, 9.5F)),
      Cursor = Cursors.Hand,
      Enabled = false
    };
    btnCancel.FlatAppearance.BorderSize = 0;
    btnCancel.Click += (_, _) =>
    {
      _testCts?.Cancel();
      btnCancel.Enabled = false;
    };

    txtLog = new TextBox
    {
      Multiline = true,
      ReadOnly = true,
      ScrollBars = ScrollBars.Vertical,
      Top = topLog,
      Left = cardLeft,
      Width = CardWidth,
      Height = logH,
      BackColor = AppColors.Terminal,
      ForeColor = AppColors.LogText,
      Font = _fonts.Track(new Font(AppFonts.MonoName, 9F)),
      BorderStyle = BorderStyle.None
    };

    btnExportNew = CreateGhostButton("Nouveau rapport", topExport, cardLeft, btnHalfW);
    btnExportAppend = CreateGhostButton("Ajouter au fichier", topExport, cardLeft + btnHalfW + 10, btnHalfW);
    btnExportNew.Click += (_, _) => HandleSave(false);
    btnExportAppend.Click += (_, _) => HandleSave(true);
  }
}
