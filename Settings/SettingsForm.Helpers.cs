using System.Drawing.Drawing2D;

namespace IperfApp.UI;

public partial class SettingsForm : Form
{
  private static void AddInputField(Panel p, string label, TextBox tb, ref int top)
  {
    Label lbl = new() { Text = label, Top = top, Left = 25, Font = new Font("Segoe UI", 7F, FontStyle.Bold), ForeColor = Color.FromArgb(0, 103, 192), AutoSize = true };
    tb.Top = top + 18; tb.Left = 25; tb.Width = 260; 
    tb.Font = new Font("Segoe UI Semibold", 9.5F); tb.BorderStyle = BorderStyle.None;
    Panel line = new() { Top = tb.Bottom + 4, Left = 25, Width = 260, Height = 1, BackColor = Color.FromArgb(210, 212, 215) };
    tb.Enter += (s, e) => line.BackColor = Color.FromArgb(0, 120, 215);
    tb.Leave += (s, e) => line.BackColor = Color.FromArgb(210, 212, 215);
    p.Controls.AddRange([lbl, tb, line]);
    top += 55;
  }

  private static void AddNumericField(Panel p, string label, TextBox tb, ref int top)
  {
    AddInputField(p, label, tb, ref top);

    // Validation : accepter seulement les chiffres
    tb.KeyPress += (s, e) =>
    {
      // Accepter les chiffres et les touches de contrôle (backspace, suppression, etc)
      if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
      {
        e.Handled = true;
      }
    };
  }

  private void DrawListItem(object? sender, DrawItemEventArgs e)
  {
    if (e.Index < 0) return;
    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
    bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
    e.Graphics.FillRectangle(new SolidBrush(isSelected ? Color.FromArgb(0, 120, 215) : lstPresets.BackColor), e.Bounds);

    if (!isSelected) {
      using Pen p = new(Color.FromArgb(225, 228, 232), 1);
      e.Graphics.DrawLine(p, e.Bounds.Left + 10, e.Bounds.Bottom - 1, e.Bounds.Right - 10, e.Bounds.Bottom - 1);
    }

    string profileName = _data.Presets[e.Index]?.Name ?? "Inconnu";
    if (isSelected) {
      Rectangle pastille = new(e.Bounds.X + 8, e.Bounds.Y + 12, 4, e.Bounds.Height - 24);
      e.Graphics.FillRectangle(Brushes.White, pastille);
    }

    Rectangle textRect = new(e.Bounds.X + 22, e.Bounds.Y, e.Bounds.Width - 22, e.Bounds.Height);
    TextRenderer.DrawText(e.Graphics, profileName, lstPresets.Font, textRect, isSelected ? Color.White : Color.FromArgb(80, 80, 80), TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
  }

  private void SetLockedState(bool locked)
  {
    // ReadOnly à tous les champs
    txtName.ReadOnly = txtServer.ReadOnly = txtPort.ReadOnly = txtChannels.ReadOnly = locked;

    // Définition des couleurs (Gris si verrouillé, Blanc/Noir si modifiable)
    Color bg = locked ? Color.FromArgb(248, 248, 248) : Color.White;
    Color fg = locked ? Color.FromArgb(160, 160, 160) : Color.Black;

    // Application groupée aux 4 champs
    var fields = new[] { txtName, txtServer, txtPort, txtChannels };
    foreach (var field in fields)
    {
      field.BackColor = bg;
      field.ForeColor = fg;
    }
  }

  private void UpdateList(string toSelect = "")
  {
    lstPresets.SelectedIndexChanged -= (s, e) => LoadSelected();
    lstPresets.DataSource = null;
    lstPresets.DataSource = _data.Presets;
    lstPresets.DisplayMember = "Name";
    if (!string.IsNullOrEmpty(toSelect)) {
      var item = _data.Presets.FirstOrDefault(x => x.Name == toSelect);
      if (item != null) lstPresets.SelectedItem = item;
    } else { lstPresets.SelectedIndex = -1; }
    lstPresets.SelectedIndexChanged += (s, e) => LoadSelected();
  }
}