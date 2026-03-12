namespace IperfApp.UI;

public partial class SettingsForm : Form
{
  private void SetupUI(Form parent)
  {
    Text = " Profils";
    Icon = parent.Icon;
    Size = new Size(520, 420);
    BackColor = Color.White;
    FormBorderStyle = FormBorderStyle.FixedDialog;
    StartPosition = FormStartPosition.CenterParent;
    MaximizeBox = false;

    // Sélection des profils
    Panel pnlLeft = new() { Dock = DockStyle.Left, Width = 165, BackColor = Color.FromArgb(242, 245, 248), Padding = new Padding(5) };
    Panel pnlBtns = new() { Dock = DockStyle.Top, Height = 40 };

    ConfigureSideButton(btnAdd, "＋", new Point(5, 5));
    ConfigureSideButton(btnRemove, "－", new Point(38, 5));
    btnAdd.Click += (s, e) => CreateNew();
    btnRemove.Click += (s, e) => DeleteSelected();

    pnlBtns.Controls.AddRange([btnAdd, btnRemove]);

    lstPresets.Dock = DockStyle.Fill;
    lstPresets.BorderStyle = BorderStyle.None;
    lstPresets.BackColor = Color.FromArgb(242, 245, 248);
    lstPresets.Font = new Font("Segoe UI Semibold", 9F);
    lstPresets.ItemHeight = 40;
    lstPresets.DrawMode = DrawMode.OwnerDrawFixed;
    lstPresets.Cursor = Cursors.Hand;
    lstPresets.DrawItem += DrawListItem; // Déporté dans Helpers
    lstPresets.SelectedIndexChanged += (s, e) => LoadSelected();

    pnlLeft.Controls.AddRange([lstPresets, pnlBtns]);

    // Édition des profils
    Panel pnlRight = new() { Dock = DockStyle.Fill, Padding = new Padding(25, 20, 25, 20) };
    lblHeader.Font = new Font("Segoe UI Variable Display", 14F, FontStyle.Bold);
    lblHeader.Location = new Point(25, 15); lblHeader.AutoSize = true;

    int top = 65;
    AddInputField(pnlRight, "NOM DU SCÉNARIO", txtName, ref top);
    AddInputField(pnlRight, "ADRESSE SERVEUR", txtServer, ref top);
    AddNumericField(pnlRight, "PORT", txtPort, ref top);
    AddNumericField(pnlRight, "CANAUX", txtChannels, ref top);

    btnSave.Text = "ENREGISTRER";
    btnSave.Dock = DockStyle.Bottom; btnSave.Height = 40;
    btnSave.BackColor = Color.FromArgb(0, 120, 215); btnSave.ForeColor = Color.White;
    btnSave.FlatStyle = FlatStyle.Flat; btnSave.Cursor = Cursors.Hand;
    btnSave.Font = new Font("Segoe UI Bold", 9F);
    btnSave.Click += (s, e) => SaveData();

    pnlRight.Controls.AddRange([lblHeader, btnSave]);
    Controls.AddRange([pnlRight, pnlLeft]);
  }

  private static void ConfigureSideButton(Button b, string txt, Point loc)
  {
    b.Text = txt; b.Size = new Size(28, 28); b.Location = loc;
    b.FlatStyle = FlatStyle.Flat; b.FlatAppearance.BorderSize = 1;
    b.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210);
    b.BackColor = Color.White; b.Cursor = Cursors.Hand;
    b.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
  }
}