using IperfApp.UI.Forms.MainForm;

namespace IperfApp;

static class Program
{
  [STAThread]
  static void Main()
  {
    Application.ThreadException += (_, e) =>
      ShowFatalError("Exception non gérée sur le thread UI", e.Exception);

    AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    {
      if (e.ExceptionObject is Exception ex)
        ShowFatalError("Exception non gérée (thread background)", ex);
    };

    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

    try
    {
      ApplicationConfiguration.Initialize();
      Application.Run(new MainForm());
    }
    catch (Exception ex)
    {
      ShowFatalError("Erreur critique au démarrage", ex);
      Environment.Exit(1);
    }
  }

  private static void ShowFatalError(string context, Exception ex) =>
    MessageBox.Show(
      $"{context} :\n\n{ex.GetType().Name}\n{ex.Message}",
      "Erreur fatale",
      MessageBoxButtons.OK,
      MessageBoxIcon.Error);
}
