using IperfApp.UI.Constants;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.SettingsForm;

public partial class SettingsForm
{
    private void SetupUI(Form parent)
    {
        Text            = " Profils";
        Icon            = parent.Icon;
        Size            = new Size(520, 480);
        BackColor       = Color.White;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition   = FormStartPosition.CenterParent;
        MaximizeBox     = false;

        // --- Panneau gauche : liste des profils ---
        var pnlLeft = new Panel
        {
            Dock      = DockStyle.Left,
            Width     = 165,
            BackColor = AppColors.SidePanel,
            Padding   = new Padding(5)
        };
        var pnlBtns = new Panel { Dock = DockStyle.Top, Height = 40 };

        ConfigureSideButton(btnAdd,    "＋", new Point(5,  5));
        ConfigureSideButton(btnRemove, "－", new Point(38, 5));
        btnAdd.Click    += (_, _) => CreateNew();
        btnRemove.Click += (_, _) => DeleteSelected();
        pnlBtns.Controls.AddRange([btnAdd, btnRemove]);

        lstPresets.Dock        = DockStyle.Fill;
        lstPresets.BorderStyle = BorderStyle.None;
        lstPresets.BackColor   = AppColors.SidePanel;
        lstPresets.ItemHeight  = 40;
        lstPresets.DrawMode    = DrawMode.OwnerDrawFixed;
        lstPresets.Cursor      = Cursors.Hand;
        lstPresets.Font        = _fonts.Track(new Font("Segoe UI Semibold", 9F));
        lstPresets.DrawItem   += DrawListItem;
        lstPresets.SelectedIndexChanged += OnPresetSelectionChanged;

        pnlLeft.Controls.AddRange([lstPresets, pnlBtns]);

        // --- Panneau droit : édition ---
        var pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25, 20, 25, 20) };

        lblHeader.Font     = _fonts.Track(new Font("Segoe UI Variable Display", 14F, FontStyle.Bold));
        lblHeader.Location = new Point(25, 15);
        lblHeader.AutoSize = true;

        int top = 65;
        FormBuilderHelpers.AddInputField  (pnlRight, "NOM DU SCÉNARIO", txtName,     ref top, _fonts);
        FormBuilderHelpers.AddInputField  (pnlRight, "ADRESSE SERVEUR", txtServer,   ref top, _fonts);
        FormBuilderHelpers.AddNumericField(pnlRight, "PORT",            txtPort,     ref top, _fonts);
        FormBuilderHelpers.AddNumericField(pnlRight, "CANAUX",          txtChannels, ref top, _fonts);
        FormBuilderHelpers.AddComboField  (pnlRight, "PROTOCOLE IP",    cbIpVersion, ref top, _fonts);

        btnSave.Text      = "ENREGISTRER";
        btnSave.Dock      = DockStyle.Bottom;
        btnSave.Height    = 40;
        btnSave.BackColor = AppColors.Accent;
        btnSave.ForeColor = Color.White;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Cursor    = Cursors.Hand;
        btnSave.Font      = _fonts.Track(new Font("Segoe UI Bold", 9F));
        btnSave.Click    += async (_, _) => await SaveDataAsync();

        pnlRight.Controls.AddRange([lblHeader, btnSave]);
        Controls.AddRange([pnlRight, pnlLeft]);
    }

    private void ConfigureSideButton(Button b, string text, Point location)
    {
        b.Text      = text;
        b.Size      = new Size(28, 28);
        b.Location  = location;
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize  = 1;
        b.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210);
        b.BackColor = Color.White;
        b.Cursor    = Cursors.Hand;
        b.Font      = _fonts.Track(new Font("Segoe UI", 9F, FontStyle.Bold));
    }
}
