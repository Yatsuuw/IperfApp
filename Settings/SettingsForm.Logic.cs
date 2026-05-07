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
      txtPort.Text     = p.Port.ToString();
      txtChannels.Text = p.Channels.ToString();
      txtDuration.Text = p.Duration > 0 ? p.Duration.ToString() : "10";
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

  /// <summary>Valide et enregistre le profil sélectionné.</summary>
  private async Task SaveDataAsync()
  {
    if (lstPresets.SelectedItem is not Preset p || p.Name == "Défaut") return;

    string name = txtName.Text.Trim();
    if (string.IsNullOrWhiteSpace(name))
    {
      MessageBox.Show("Le nom du scénario ne peut pas être vide.", "Validation",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
      txtName.Focus(); return;
    }

    string server = txtServer.Text.Trim();
    if (string.IsNullOrWhiteSpace(server))
    {
      MessageBox.Show("L'adresse du serveur ne peut pas être vide.", "Validation",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
      txtServer.Focus(); return;
    }

    if (!int.TryParse(txtPort.Text, out int port) || port < 1 || port > 65535)
    {
      MessageBox.Show("Le port doit être un entier compris entre 1 et 65535.", "Validation",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
      txtPort.Focus(); return;
    }

    if (!int.TryParse(txtChannels.Text, out int channels) || channels < 1)
    {
      MessageBox.Show("Le nombre de canaux doit être un entier supérieur ou égal à 1.", "Validation",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
      txtChannels.Focus(); return;
    }

    if (!int.TryParse(txtDuration.Text, out int duration) || duration < 1 || duration > 120)
    {
      MessageBox.Show("La durée doit être un entier compris entre 1 et 120 secondes.", "Validation",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
      txtDuration.Focus(); return;
    }

    p.Name      = name;
    p.Server    = server;
    p.Port      = port;
    p.Channels  = channels;
    p.Duration  = duration;
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

    string oldTxt = btnSave.Text;
    Color  oldCol = btnSave.BackColor;
    btnSave.Text      = "✓ ENREGISTRÉ";
    btnSave.BackColor = Color.FromArgb(40, 167, 100);
    await Task.Delay(1000);
    btnSave.Text      = oldTxt;
    btnSave.BackColor = oldCol;
  }

  private void CreateNew()
  {
    var newP = new Preset
    {
      Name      = "Nouveau profil",
      Server    = "",
      Port      = 5201,
      Channels  = 8,
      Duration  = 10,
      IpVersion = IpVersion.Auto
    };
    _data.Presets.Add(newP);
    UpdateList(newP.Name);
    SetLockedState(false);
    lblHeader.Text    = "Modification";
    btnRemove.Enabled = true;
    btnSave.Enabled   = true;
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
