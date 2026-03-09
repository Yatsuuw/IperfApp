using IperfApp.Services;

namespace IperfApp.UI;

public partial class SettingsForm : Form
{
  private void LoadSelected()
  {
    if (lstPresets.SelectedItem is Preset p)
    {
      txtName.Text = p.Name; txtServer.Text = p.Server;
      txtPort.Text = p.Port; txtChannels.Text = p.Channels;
      
      bool isDef = p.Name == "Défaut";
      SetLockedState(isDef);
      lblHeader.Text = isDef ? "Profil Système 🔒" : "Modification";
      
      btnRemove.Enabled = !isDef;
      btnSave.Enabled = !isDef;
    }
    else
    {
      SetLockedState(true);
      btnSave.Enabled = false;
    }
  }

  private async void SaveData()
  {

    // Si rien n'est sélectionné ou si c'est "Défaut", stoppe tout
    if (lstPresets.SelectedItem is not Preset p || p.Name == "Défaut") return;

    // Mise à jour des données de l'objet existant
    p.Name = txtName.Text;
    p.Server = txtServer.Text;
    p.Port = txtPort.Text;
    p.Channels = txtChannels.Text;

    // Sauvegarde physique
    ConfigService.Save(_data);

    // Rafraîchissement visuel de la liste pour mettre à jour le nom à gauche
    int currentIndex = lstPresets.SelectedIndex;
    UpdateList();
    lstPresets.SelectedIndex = currentIndex;

    // Feedback visuel
    string oldTxt = btnSave.Text; Color oldCol = btnSave.BackColor;
    btnSave.Text = "✓ ENREGISTRÉ"; btnSave.BackColor = Color.FromArgb(40, 167, 100);
    await Task.Delay(1000);
    btnSave.Text = oldTxt; btnSave.BackColor = oldCol;
  }

  private void CreateNew()
  {
    // Création du profil immédiatement dans la liste
    var newP = new Preset { Name = "Nouveau profil", Server = "0.0.0.0", Port = "5201", Channels = "8" };
    _data.Presets.Add(newP);
    
    // Rafraîchissement de la liste et sélection du nouveau profil
    UpdateList(newP.Name);
    
    // Donne le focus au nom pour que l'utilisateur puisse le renommer
    txtName.Focus();
    txtName.SelectAll();
  }

  private void DeleteSelected()
  {
    if (lstPresets.SelectedItem is Preset p && p.Name != "Défaut")
    {
      _data.Presets.Remove(p); 
      UpdateList(); 
      SetLockedState(true);
      lblHeader.Text = "Profil";
      btnSave.Enabled = false;
    }
  }
}