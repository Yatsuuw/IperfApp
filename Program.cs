namespace IperfApp.UI;

static class Program
{
  /// <summary>
  ///  The main entry point for the application.
  /// </summary>
  [STAThread]
  static void Main()
  {
    // To customize application configuration such as set high DPI settings or default font,
    // see https://aka.ms/applicationconfiguration.

    try
    {
      ApplicationConfiguration.Initialize();
      Application.Run(new Form1());
    }
    catch (Exception ex)
    {
      MessageBox.Show(
        $"Erreur critique au démarrage :\n\n{ex.GetType().Name}\n{ex.Message}\n\n{ex.StackTrace}",
        "Erreur",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error
      );
      Environment.Exit(1);
    }
  }    
}