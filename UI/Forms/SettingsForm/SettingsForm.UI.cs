using IperfApp.UI.Constants;

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

        // Panneau gauche : liste des profils
        Panel pnlLeft = new()
        {
            Dock      = DockStyle.Left,
            Width     = 165,
            BackColor = AppColors.SidePanel,
            Padding   = new Padding(5)
        };
        Panel pnlBtns = new() { Dock = DockStyle.Top, Height = 40 };

        ConfigureSideButton(btnAdd,    "＋", new Point(5,  5));
        ConfigureSideButton(btnRemove, "－", new Point(38, 5));
        btnAdd.Click    += (_, _) => CreateNew();
        btnRemove.Click += (_, _) => DeleteSelected();

        pnlBtns.Controls.AddRange([btnAdd, btnRemove]);

        lstPresets.Dock        = DockStyle.Fill;
        lstPresets.BorderStyle = BorderStyle.None;
        lstPresets.BackColor   = AppColors.SidePanel;
        lstPresets.Font        = new Font("Segoe UI Semibold", 9F);
        lstPresets.ItemHeight  = 40;
        lstPresets.DrawMode    = DrawMode.OwnerDrawFixed;
        lstPresets.Cursor      = Cursors.Hand;
        lstPresets.DrawItem   += DrawListItem;
        lstPresets.SelectedIndexChanged += OnPresetSelectionChanged;

        pnlLeft.Controls.AddRange([lstPresets, pnlBtns]);

        // Panneau droit : édition du profil sélectionné
        Panel pnlRight = new() { Dock = DockStyle.Fill, Padding = new Padding(25, 20, 25, 20) };
        lblHeader.Font     = new Font("Segoe UI Variable Display", 14F, FontStyle.Bold);
        lblHeader.Location = new Point(25, 15);
        lblHeader.AutoSize = true;

        int top = 65;
        AddInputField  (pnlRight, "NOM DU SCÉNARIO", txtName,     ref top);
        AddInputField  (pnlRight, "ADRESSE SERVEUR", txtServer,   ref top);
        AddNumericField(pnlRight, "PORT",             txtPort,     ref top);
        AddNumericField(pnlRight, "CANAUX",           txtChannels, ref top);
        AddComboField  (pnlRight, "PROTOCOLE IP",     cbIpVersion, ref top);

        btnSave.Text      = "ENREGISTRER";
        btnSave.Dock      = DockStyle.Bottom;
        btnSave.Height    = 40;
        btnSave.BackColor = AppColors.Accent;
        btnSave.ForeColor = Color.White;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Cursor    = Cursors.Hand;
        btnSave.Font      = new Font("Segoe UI Bold", 9F);
        btnSave.Click    += async (_, _) => await SaveDataAsync();

        pnlRight.Controls.AddRange([lblHeader, btnSave]);
        Controls.AddRange([pnlRight, pnlLeft]);
    }

    private static void ConfigureSideButton(Button b, string txt, Point loc)
    {
        b.Text      = txt;
        b.Size      = new Size(28, 28);
        b.Location  = loc;
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize  = 1;
        b.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210);
        b.BackColor = Color.White;
        b.Cursor    = Cursors.Hand;
        b.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
    }
}
