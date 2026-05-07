using System.Drawing.Drawing2D;
using IperfApp.Models;
using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.SettingsForm;

public partial class SettingsForm
{
    // ---------------------------------------------------------------
    // Rendu personnalisé de la ListBox
    // ---------------------------------------------------------------

    private void DrawListItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0) return;

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

        using (var bgBrush = new SolidBrush(isSelected ? AppColors.Accent : lstPresets.BackColor))
            e.Graphics.FillRectangle(bgBrush, e.Bounds);

        if (!isSelected)
        {
            using var pen = new Pen(Color.FromArgb(225, 228, 232), 1);
            e.Graphics.DrawLine(pen,
                e.Bounds.Left  + 10, e.Bounds.Bottom - 1,
                e.Bounds.Right - 10, e.Bounds.Bottom - 1);
        }

        // Accès O(1) direct à l'item
        string name = (lstPresets.Items[e.Index] as Preset)?.Name ?? "Inconnu";

        if (isSelected)
        {
            var pastille = new Rectangle(e.Bounds.X + 8, e.Bounds.Y + 12, 4, e.Bounds.Height - 24);
            e.Graphics.FillRectangle(Brushes.White, pastille);
        }

        TextRenderer.DrawText(
            e.Graphics, name, lstPresets.Font,
            new Rectangle(e.Bounds.X + 22, e.Bounds.Y, e.Bounds.Width - 22, e.Bounds.Height),
            isSelected ? Color.White : Color.FromArgb(80, 80, 80),
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
    }

    // ---------------------------------------------------------------
    // État verrouillé / déverrouillé des champs
    // ---------------------------------------------------------------

    private void SetLockedState(bool locked)
    {
        txtName.ReadOnly = txtServer.ReadOnly =
            txtPort.ReadOnly = txtChannels.ReadOnly = locked;
        cbIpVersion.Enabled = !locked;

        Color bg = locked ? Color.FromArgb(248, 248, 248) : Color.White;
        Color fg = locked ? Color.FromArgb(160, 160, 160) : Color.Black;

        foreach (var field in (TextBox[])[txtName, txtServer, txtPort, txtChannels])
        {
            field.BackColor = bg;
            field.ForeColor = fg;
        }
    }

    // ---------------------------------------------------------------
    // Abonnement sélection ListBox
    // ---------------------------------------------------------------

    private void OnPresetSelectionChanged(object? sender, EventArgs e)
    {
        if (lstPresets.SelectedItem is Preset p)
            LoadPresetIntoFields(p);
    }

    /// <summary>
    /// Repeuple la ListBox.
    /// Désabonne / ré-abonne <see cref="OnPresetSelectionChanged"/> pour éviter
    /// les déclenchements multiples pendant le rechargement de la DataSource.
    /// </summary>
    private void UpdateList(string toSelect = "")
    {
        lstPresets.SelectedIndexChanged -= OnPresetSelectionChanged;

        lstPresets.DataSource    = null;
        lstPresets.DataSource    = _data.Presets;
        lstPresets.DisplayMember = nameof(Preset.Name);

        if (!string.IsNullOrEmpty(toSelect))
        {
            var item = _data.Presets.FirstOrDefault(x => x.Name == toSelect);
            if (item is not null)
            {
                lstPresets.SelectedItem = item;
                LoadPresetIntoFields(item);
            }
        }
        else
        {
            lstPresets.SelectedIndex = -1;
        }

        lstPresets.SelectedIndexChanged += OnPresetSelectionChanged;
    }
}
