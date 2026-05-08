using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.SettingsForm;

public partial class SettingsForm
{
  private void LoadPresetIntoFields(Preset p)
  {
    lblHeader.Text = p.Name;
    txtName.Text = p.Name;
    txtServer.Text = p.Server;
    txtPort.Text = p.Port.ToString();
    txtChannels.Text = p.Channels.ToString();
    txtDuration.Text = p.Duration.ToString();
    cbIpVersion.SelectedIndex = p.IpVersion.ToComboIndex();
  }

  private void CreateNew()
  {
    var newP = new Preset
    {
      Name = $"Profil {_data.Presets.Count + 1}",
      Server = "exemple.iperf.fr",
      Port = 5201,
      Channels = 4,
      Duration = 10
    };

    _data.Presets.Add(newP);

    try
    {
      ConfigService.Save(_data);
    }
    catch (Exception ex)
    {
      MessageBox.Show(this,
        $"Profil créé en mémoire mais non sauvegardé sur le disque :\n{ex.Message}",
        "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    UpdateList(newP.Name);
  }

  private void DeleteSelected()
  {
    if (lstPresets.SelectedItem is not Preset p) return;

    if (p.IsDefault)
    {
      MessageBox.Show(this, "Le profil système par défaut ne peut pas être supprimé.",
        "Action impossible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

    var confirm = MessageBox.Show(this,
      $"Supprimer le profil \u00ab {p.Name} \u00bb ? Cette action est irréversible.",
      "Confirmer la suppression",
      MessageBoxButtons.YesNo,
      MessageBoxIcon.Warning);

    if (confirm != DialogResult.Yes) return;

    _data.Presets.Remove(p);

    if (_data.SelectedPresetName == p.Name)
      _data.SelectedPresetName = _data.Presets.FirstOrDefault()?.Name ?? string.Empty;

    try
    {
        ConfigService.Save(_data);
    }
    catch (Exception ex)
    {
      MessageBox.Show(this,
        $"Profil supprimé en mémoire mais la sauvegarde a échoué :\n{ex.Message}",
        "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    UpdateList(_data.Presets.FirstOrDefault()?.Name ?? string.Empty);
  }

  private async Task SaveDataAsync()
  {
    if (lstPresets.SelectedItem is not Preset current) return;

    _ = int.TryParse(txtPort.Text, out int port);
    _ = int.TryParse(txtChannels.Text, out int channels);
    _ = int.TryParse(txtDuration.Text, out int duration);

    var updated = new Preset
    {
      Name = txtName.Text.Trim(),
      Server = txtServer.Text.Trim(),
      Port= port,
      Channels = channels,
      Duration = duration,
      IpVersion = IpVersionExtensions.FromComboIndex(cbIpVersion.SelectedIndex),
      IsDefault = current.IsDefault
    };

    string? validationError = updated.Validate();
    if (validationError is not null)
    {
      MessageBox.Show(this, validationError, "Validation",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

    int index = _data.Presets.IndexOf(current);
    if (index >= 0)
      _data.Presets[index] = updated;

    if (_data.SelectedPresetName == current.Name)
      _data.SelectedPresetName = updated.Name;

    try
    {
      await Task.Run(() => ConfigService.Save(_data));
    }
    catch (Exception ex)
    {
      MessageBox.Show(this, $"Sauvegarde échouée :\n{ex.Message}",
        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
      return;
    }

    if (IsDisposed) return;

    btnSave.Enabled = false;
    var originalColor = btnSave.BackColor;
    var originalText = btnSave.Text;
    btnSave.Text = "\u2713 Enregistré";
    btnSave.BackColor = Constants.AppColors.Success;

    await Task.Delay(1500);

    try
    {
      if (IsDisposed) return;
      UpdateList(updated.Name);
      btnSave.Text = originalText;
      btnSave.BackColor = originalColor;
      btnSave.Enabled = true;
    }
    catch (ObjectDisposedException)
    {
    }
  }
}
