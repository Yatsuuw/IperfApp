using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
  private readonly SemaphoreSlim _saveSemaphore = new(1, 1);

  internal void RefreshPresetList(bool warnIfEmpty = false)
  {
    if (_config.Presets.Count == 0)
    {
      cbPresets.DataSource = null;
      cbPresets.Items.Clear();

      if (!_testRunning)
      {
        btnStart.Enabled = false;
        btnStart.Text = "AUCUN PROFIL CONFIGURÉ";
      }

      if (warnIfEmpty)
        MessageBox.Show(this,
          "Aucun profil n'est configuré.\n\nOuvrez le menu \"Profils\" pour en créer un.",
          "Configuration vide", MessageBoxButtons.OK, MessageBoxIcon.Information);

      return;
    }

    if (!_testRunning)
    {
      btnStart.Enabled = true;
      btnStart.Text = "LANCER L'ANALYSE";
    }

    cbPresets.SelectedIndexChanged -= CbPresets_SelectedIndexChanged;
    cbPresets.DataSource = null;
    cbPresets.DataSource = _config.Presets;
    cbPresets.DisplayMember = nameof(Preset.Name);

    var selected =
      _config.Presets.FirstOrDefault(p => p.Name == _config.SelectedPresetName)
      ?? _config.Presets[0];

    cbPresets.SelectedItem = selected;
    ApplyPreset(selected);
    cbPresets.SelectedIndexChanged += CbPresets_SelectedIndexChanged;
  }

  private void CbPresets_SelectedIndexChanged(object? sender, EventArgs e)
  {
    if (cbPresets.SelectedItem is not Preset p) return;

    _config.SelectedPresetName = p.Name;

    _ = Task.Run(async () =>
    {
      await _saveSemaphore.WaitAsync();
      try { ConfigService.Save(_config); }
      catch (Exception ex)
      {
        Debug.WriteLine($"[MainForm] Échec sauvegarde sélection profil : {ex.Message}");
      }
      finally { _saveSemaphore.Release(); }
    });

    ApplyPreset(p);
  }

  private void ApplyPreset(Preset p)
  {
    txtServer.Text = p.Server;
    txtPort.Text = p.Port.ToString();
    txtChannels.Text = p.Channels.ToString();
    cbIpVersion.SelectedIndex = p.IpVersion.ToComboIndex();
  }
}
