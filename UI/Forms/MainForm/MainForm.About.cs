using IperfApp.UI.Forms.AboutForm;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>Ouvre la boîte de dialogue « Informations ».</summary>
    private void ShowAboutBox()
    {
        // AboutDialog : classe dans le namespace IperfApp.UI.Forms.AboutForm
        // Le using ci-dessus lève l'ambiguïté avec le dossier AboutForm.
        using var about = new AboutDialog(Icon);
        about.ShowDialog(this);
    }
}
