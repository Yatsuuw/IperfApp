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
    }
  }

  private async void SaveData()
  {
    Preset? p = lstPresets.SelectedItem as Preset;
    if (lstPresets.SelectedIndex == -1 && !string.IsNullOrWhiteSpace(txtName.Text))
    {
      p = new Preset { Name = txtName.Text, Server = txtServer.Text, Port = txtPort.Text, Channels = txtChannels.Text };
      _data.Presets.Add(p);
      UpdateList(p.Name);
    }
    else if (p != null && p.Name != "Défaut")
    {
      p.Name = txtName.Text; p.Server = txtServer.Text;
      p.Port = txtPort.Text; p.Channels = txtChannels.Text;
      UpdateList(p.Name);
    }

    ConfigService.Save(_data);

    // Feedback visuel
    string oldTxt = btnSave.Text; Color oldCol = btnSave.BackColor;
    btnSave.Text = "✓ ENREGISTRÉ"; btnSave.BackColor = Color.FromArgb(40, 167, 100);
    await Task.Delay(1000);
    btnSave.Text = oldTxt; btnSave.BackColor = oldCol;
  }

  private void CreateNew()
  {
    _data.Presets.Add(new Preset { Name = "Nouveau profil", Server = "0.0.0.0", Port = "5201", Channels = "8" });
    UpdateList("Nouveau profil");
  }

  private void DeleteSelected()
  {
    if (lstPresets.SelectedItem is Preset p && p.Name != "Défaut")
    {
      _data.Presets.Remove(p); UpdateList(); SetLockedState(true);
      lblHeader.Text = "Profil";
    }
  }
}