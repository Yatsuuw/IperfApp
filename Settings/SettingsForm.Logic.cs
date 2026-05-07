using IperfApp.Models;
using IperfApp.Services;

namespace IperfApp.UI;

public partial class SettingsForm : Form
{
  private void LoadSelected()
  {
    if (lstPresets.SelectedItem is Preset p)
    {
      txtName.Text     = p.Name;
      txtServer.Text   = p.Server;
      txtPort.Text     = p.Port;
      txtChannels.Text = p.Channels;
      cbIpVersion.SelectedIndex = p.IpVersion switch
      {
        IpVersion.IPv4 => 1,
        IpVersion.IPv6 => 2,
        _              => 0
      };

      bool isDef = p.Name == "Défaut";
      SetLockedState(isDef);
      lblHeader.Text = isDef ? "Profil Système 🔒" : "Modification";
      btnRemove.Enabled = !isDef;
      btnSave.Enabled   = !isDef;
    }
    else
    {
      SetLockedState(true);
      btnSave.Enabled = false;
    }
  }

  private async void SaveData()
  {
    if (lstPresets.SelectedItem is not Preset p || p.Name == "Défaut") return;

    p.Name     = txtName.Text;
    p.Server   = txtServer.Text;
    p.Port     = txtPort.Text;
    p.Channels = txtChannels.Text;
    p.IpVersion = cbIpVersion.SelectedIndex switch
    {
      1 => IpVersion.IPv4,
      2 => IpVersion.IPv6,
      _ => IpVersion.Auto
    };

    ConfigService.Save(_data);

    int currentIndex = lstPresets.SelectedIndex;
    UpdateList();
    lstPresets.SelectedIndex = currentIndex;

    string oldTxt = btnSave.Text; Color oldCol = btnSave.BackColor;
    btnSave.Text = "✓ ENREGISTRÉ"; btnSave.BackColor = Color.FromArgb(40, 167, 100);
    await Task.Delay(1000);
    btnSave.Text = oldTxt; btnSave.BackColor = oldCol;
  }

  private void CreateNew()
  {
    var newP = new Preset { Name = "Nouveau profil", Server = "0.0.0.0", Port = "5201", Channels = "8", IpVersion = IpVersion.Auto };
    _data.Presets.Add(newP);
    UpdateList(newP.Name);
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
      lblHeader.Text  = "Profil";
      btnSave.Enabled = false;
    }
  }
}
