namespace IperfApp.UI;

public partial class Form1
{
  // ---------------------------------------------------------------
  // Builder : zone d'actions (boutons + console + export)
  // Toutes les positions Y sont calculées à partir de CardBottom
  // pour éviter les valeurs absolues fragiles.
  // ---------------------------------------------------------------

  /// <summary>
  /// Initialise <see cref="btnStart"/>, <see cref="btnCancel"/>,
  /// <see cref="txtLog"/>, <see cref="btnExportNew"/> et
  /// <see cref="btnExportAppend"/>.
  /// Toutes les positions Y sont calculées relativement à
  /// <see cref="CardBottom"/> pour faciliter les ajustements futurs.
  /// </summary>
  private void BuildActionsArea()
  {
    const int spacer   = 15;   // espace entre éléments
    const int btnMainH = 50;   // hauteur bouton Lancer
    const int btnCancelH = 30; // hauteur bouton Annuler
    const int logH     = 170;  // hauteur console
    const int exportH  = 45;   // hauteur boutons export

    int topStart  = CardBottom + spacer;                          // 365
    int topCancel = topStart  + btnMainH  + spacer / 3;           // ~418
    int topLog    = topCancel + btnCancelH + spacer;              // ~463
    int topExport = topLog    + logH      + spacer / 3;           // ~638

    int left  = CardLeft;
    int btnHalfW = (CardWidth / 2) - 5;

    // --- Bouton Lancer ---
    btnStart = new Button
    {
      Text      = "LANCER L'ANALYSE",
      Top       = topStart,
      Left      = left,
      Width     = CardWidth,
      Height    = btnMainH,
      BackColor = _colorAccent,
      ForeColor = Color.White,
      FlatStyle = FlatStyle.Flat,
      Font      = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
      Cursor    = Cursors.Hand
    };
    btnStart.FlatAppearance.BorderSize = 0;
    btnStart.Click += async (_, _) => await RunFullTest();

    // --- Bouton Annuler ---
    btnCancel = new Button
    {
      Text      = "ANNULER",
      Top       = topCancel,
      Left      = left,
      Width     = CardWidth,
      Height    = btnCancelH,
      BackColor = _colorDanger,
      ForeColor = Color.White,
      FlatStyle = FlatStyle.Flat,
      Font      = new Font("Segoe UI Semibold", 9F),
      Cursor    = Cursors.Hand,
      Enabled   = false
    };
    btnCancel.FlatAppearance.BorderSize = 0;
    btnCancel.Click += (_, _) =>
    {
      _testCts?.Cancel();
      btnCancel.Enabled = false;
    };

    // --- Console de logs ---
    txtLog = new TextBox
    {
      Multiline   = true,
      ReadOnly    = true,
      ScrollBars  = ScrollBars.Vertical,
      Top         = topLog,
      Left        = left,
      Width       = CardWidth,
      Height      = logH,
      BackColor   = _colorTerminal,
      ForeColor   = Color.FromArgb(220, 220, 220),
      Font        = new Font("Consolas", 9F),
      BorderStyle = BorderStyle.None
    };

    // --- Boutons Export CSV ---
    btnExportNew    = CreateGhostButton("Nouveau rapport",    topExport, left,              btnHalfW);
    btnExportAppend = CreateGhostButton("Ajouter au fichier", topExport, left + btnHalfW + 10, btnHalfW);
    btnExportNew.Click    += (_, _) => HandleSave(false);
    btnExportAppend.Click += (_, _) => HandleSave(true);
  }
}
