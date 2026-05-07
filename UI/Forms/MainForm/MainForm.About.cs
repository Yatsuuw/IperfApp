using IperfApp.UI.Forms.AboutForm;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>Ouvre la boîte de dialogue « Informations ».</summary>
    private void ShowAboutBox()
    {
        // L'icône peut être null si le chargement a échoué au démarrage — AboutForm gère ce cas.
        using var about = new AboutForm(Icon);
        about.ShowDialog(this);
    }
}
