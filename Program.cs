using IperfApp.UI.Forms.MainForm;

namespace IperfApp;

static class Program
{
    /// <summary>Point d'entrée principal de l'application.</summary>
    [STAThread]
    static void Main()
    {
        // Capture des exceptions non gérées sur le thread UI
        Application.ThreadException += (_, e) =>
            ShowFatalError("Exception non gérée sur le thread UI", e.Exception);

        // Capture des exceptions non gérées sur les threads d'arrière-plan
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

    private static void ShowFatalError(string context, Exception ex)
    {
        MessageBox.Show(
            $"{context} :\n\n{ex.GetType().Name}\n{ex.Message}\n\n{ex.StackTrace}",
            "Erreur fatale",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
        );
    }
}
