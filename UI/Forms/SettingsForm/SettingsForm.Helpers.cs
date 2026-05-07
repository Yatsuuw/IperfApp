using System.Drawing.Drawing2D;
using IperfApp.Models;
using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.SettingsForm;

public partial class SettingsForm
{
    // ---------------------------------------------------------------
    // Helpers : construction des champs de formulaire
    // ---------------------------------------------------------------

    private static void AddInputField(Panel p, string label, TextBox tb, ref int top)
    {
        Label lbl = new()
        {
            Text      = label,
            Top       = top,
            Left      = 25,
            Font      = new Font("Segoe UI", 7F, FontStyle.Bold),
            ForeColor = AppColors.Accent,
            AutoSize  = true
        };

        tb.Top         = top + 18;
        tb.Left        = 25;
        tb.Width       = 260;
        tb.Font        = new Font("Segoe UI Semibold", 9.5F);
        tb.BorderStyle = BorderStyle.None;

        Panel line = new()
        {
            Top       = tb.Bottom + 4,
            Left      = 25,
            Width     = 260,
            Height    = 1,
            BackColor = AppColors.FieldBorder
        };

        tb.Enter += (_, _) => line.BackColor = AppColors.FieldBorderFocus;
        tb.Leave += (_, _) => line.BackColor = AppColors.FieldBorder;

        p.Controls.AddRange([lbl, tb, line]);
        top += 55;
    }

    private static void AddNumericField(Panel p, string label, TextBox tb, ref int top)
    {
        AddInputField(p, label, tb, ref top);
        tb.KeyPress += (_, e) =>
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        };
    }

    private static void AddComboField(Panel p, string label, ComboBox cb, ref int top)
    {
        Label lbl = new()
        {
            Text      = label,
            Top       = top,
            Left      = 25,
            Font      = new Font("Segoe UI", 7F, FontStyle.Bold),
            ForeColor = AppColors.Accent,
            AutoSize  = true
        };

        cb.Top           = top + 18;
        cb.Left          = 25;
        cb.Width         = 260;
        cb.Font          = new Font("Segoe UI Semibold", 9.5F);
        cb.DropDownStyle = ComboBoxStyle.DropDownList;
        cb.FlatStyle     = FlatStyle.Flat;
        cb.Items.AddRange(["Auto (défaut)", "IPv4 (-4)", "IPv6 (-6)"]);
        cb.SelectedIndex = 0;

        Panel line = new()
        {
            Top       = cb.Bottom + 4,
            Left      = 25,
            Width     = 260,
            Height    = 1,
            BackColor = AppColors.FieldBorder
        };

        cb.Enter += (_, _) => line.BackColor = AppColors.FieldBorderFocus;
        cb.Leave += (_, _) => line.BackColor = AppColors.FieldBorder;

        p.Controls.AddRange([lbl, cb, line]);
        top += 55;
    }

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
            using Pen pen = new(Color.FromArgb(225, 228, 232), 1);
            e.Graphics.DrawLine(pen,
                e.Bounds.Left  + 10, e.Bounds.Bottom - 1,
                e.Bounds.Right - 10, e.Bounds.Bottom - 1);
        }

        string profileName = _data.Presets.ElementAtOrDefault(e.Index)?.Name ?? "Inconnu";

        if (isSelected)
        {
            var pastille = new Rectangle(e.Bounds.X + 8, e.Bounds.Y + 12, 4, e.Bounds.Height - 24);
            e.Graphics.FillRectangle(Brushes.White, pastille);
        }

        var textRect = new Rectangle(e.Bounds.X + 22, e.Bounds.Y, e.Bounds.Width - 22, e.Bounds.Height);
        TextRenderer.DrawText(
            e.Graphics, profileName, lstPresets.Font, textRect,
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

        foreach (var field in new[] { txtName, txtServer, txtPort, txtChannels })
        {
            field.BackColor = bg;
            field.ForeColor = fg;
        }
    }

    // ---------------------------------------------------------------
    // Mise à jour de la liste des profils
    // ---------------------------------------------------------------

    private void OnPresetSelectionChanged(object? sender, EventArgs e) => LoadSelected();

    /// <summary>
    /// Repeuple la <see cref="ListBox"/> des profils.
    /// Désabonne / ré-abonne <see cref="OnPresetSelectionChanged"/> pour éviter
    /// les déclenchements multiples. Appelle <see cref="LoadSelected"/> explicitement
    /// car l'événement est muet pendant le rechargement de la DataSource.
    /// </summary>
    private void UpdateList(string toSelect = "")
    {
        lstPresets.SelectedIndexChanged -= OnPresetSelectionChanged;

        lstPresets.DataSource    = null;
        lstPresets.DataSource    = _data.Presets;
        lstPresets.DisplayMember = "Name";

        if (!string.IsNullOrEmpty(toSelect))
        {
            var item = _data.Presets.FirstOrDefault(x => x.Name == toSelect);
            if (item is not null)
            {
                lstPresets.SelectedItem = item;
                LoadSelected();
            }
        }
        else
        {
            lstPresets.SelectedIndex = -1;
        }

        lstPresets.SelectedIndexChanged += OnPresetSelectionChanged;
    }
}
