namespace IperfApp;

static class Program
{
  /// <summary>Point d'entrée principal de l'application.</summary>
  [STAThread]
  static void Main()
  {
    try
    {
      ApplicationConfiguration.Initialize();
      Application.Run(new IperfApp.UI.Form1());
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
