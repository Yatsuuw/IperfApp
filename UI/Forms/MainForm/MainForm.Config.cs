using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>Charge la configuration depuis le disque et met à jour l'UI.</summary>
    private void LoadConfigIntoUI() => RefreshPresetList();

    /// <summary>
    /// Repeuple le <see cref="ComboBox"/> des profils et sélectionne le dernier utilisé.
    /// Désactive le bouton de lancement si aucun profil n'existe.
    /// </summary>
    internal void RefreshPresetList()
    {
        if (_config.Presets.Count == 0)
        {
            cbPresets.DataSource = null;
            cbPresets.Items.Clear();
            btnStart.Enabled = false;
            btnStart.Text    = "AUCUN PROFIL CONFIGURÉ";
            MessageBox.Show(
                "Aucun profil n'est configuré.\n\nOuvrez le menu \"Profils\" pour en créer un.",
                "Configuration vide", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        btnStart.Enabled = true;
        btnStart.Text    = "LANCER L'ANALYSE";

        cbPresets.SelectedIndexChanged -= CbPresets_SelectedIndexChanged;
        cbPresets.DataSource    = null;
        cbPresets.DataSource    = _config.Presets;
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
        try   { ConfigService.Save(_config); }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[MainForm] Échec sauvegarde sélection profil : {ex.Message}");
        }
        ApplyPreset(p);
    }

    /// <summary>Remplit les champs UI depuis un profil.</summary>
    private void ApplyPreset(Preset p)
    {
        txtServer.Text    = p.Server;
        txtPort.Text      = p.Port.ToString();
        txtChannels.Text  = p.Channels.ToString();
        cbIpVersion.SelectedIndex = p.IpVersion.ToComboIndex();
    }

    /// <summary>Affiche la boîte À propos.</summary>
    private static void ShowAboutBox()
    {
        MessageBox.Show(
            "Speedtest Iperf\n" +
            "Version 1.0\n\n" +
            "Application de mesure de débit réseau\nbasée sur iperf3.\n\n" +
            "© 2025 — Yatsuuw",
            "Informations",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
