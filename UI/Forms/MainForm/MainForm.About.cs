namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>Ouvre la boîte de dialogue « Informations ».</summary>
    private void ShowAboutBox()
    {
        // Nom qualifié complet pour éviter l'ambiguïté entre le namespace et la classe AboutForm.
        using var about = new IperfApp.UI.Forms.AboutForm.AboutForm(Icon);
        about.ShowDialog(this);
    }
}
