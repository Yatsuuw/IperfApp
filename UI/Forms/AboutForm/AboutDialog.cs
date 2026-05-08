using IperfApp.UI.Constants;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.AboutForm;

/// <summary>
/// Fenêtre modale « Informations » :
/// logo, nom du produit, version, auteur et description rapide.
/// </summary>
public sealed class AboutDialog : Form
{
    private readonly FontTracker _fonts = new();

    public AboutDialog(Icon? ownerIcon)
    {
        // ---------------------------------------------------------------
        // Propriétés de la fenêtre
        // ---------------------------------------------------------------
        Text            = "Informations";
        Size            = new Size(420, 360);
        MinimumSize     = Size;
        MaximumSize     = Size;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition   = FormStartPosition.CenterParent;
        BackColor       = AppColors.Card;
        ShowInTaskbar   = false;
        MaximizeBox     = false;
        MinimizeBox     = false;

        if (ownerIcon is not null)
            Icon = ownerIcon;

        BuildUI();
    }

    // ---------------------------------------------------------------
    // Construction de l'interface
    // ---------------------------------------------------------------

    private void BuildUI()
    {
        // --- Logo ---------------------------------------------------
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "favicon.ico");
        var picLogo = new PictureBox
        {
            Size     = new Size(72, 72),
            Location = new Point((ClientSize.Width - 72) / 2, 28),
            SizeMode = PictureBoxSizeMode.Zoom
        };

        if (File.Exists(iconPath))
        {
            try
            {
                using var ico = new Icon(iconPath, 64, 64);
                picLogo.Image = ico.ToBitmap();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AboutDialog] Chargement logo échoué : {ex.Message}");
            }
        }

        // --- Nom du produit -----------------------------------------
        var lblProduct = new Label
        {
            Text      = Application.ProductName ?? "Speedtest Iperf",
            Font      = _fonts.Track(new Font("Segoe UI Variable Display", 16F, FontStyle.Bold)),
            ForeColor = AppColors.Accent,
            AutoSize  = false,
            Width     = ClientSize.Width,
            Height    = 32,
            Top       = picLogo.Bottom + 14,
            Left      = 0,
            TextAlign = ContentAlignment.MiddleCenter
        };

        // --- Version ------------------------------------------------
        string version = Application.ProductVersion ?? "1.0.0";
        // Tronquer au format X.Y.Z (sans le 4e composant Build auto-généré par .NET)
        var parts = version.Split('.');
        if (parts.Length >= 3)
            version = string.Join('.', parts[0], parts[1], parts[2]);

        var lblVersion = new Label
        {
            Text      = $"Version {version}",
            Font      = _fonts.Track(new Font("Segoe UI", 9F)),
            ForeColor = AppColors.TextMuted,
            AutoSize  = false,
            Width     = ClientSize.Width,
            Height    = 20,
            Top       = lblProduct.Bottom + 2,
            Left      = 0,
            TextAlign = ContentAlignment.MiddleCenter
        };

        // --- Auteur -------------------------------------------------
        var lblAuthor = new Label
        {
            Text      = "Par Lucas PIETERS",
            Font      = _fonts.Track(new Font("Segoe UI", 9F, FontStyle.Italic)),
            ForeColor = AppColors.TextMuted,
            AutoSize  = false,
            Width     = ClientSize.Width,
            Height    = 20,
            Top       = lblVersion.Bottom + 2,
            Left      = 0,
            TextAlign = ContentAlignment.MiddleCenter
        };

        // --- Séparateur --------------------------------------------
        var separator = new Panel
        {
            BackColor = AppColors.CardBorder,
            Left      = 40,
            Width     = ClientSize.Width - 80,
            Height    = 1,
            Top       = lblAuthor.Bottom + 16
        };

        // --- Description --------------------------------------------
        var lblDesc = new Label
        {
            Text = "Outil de mesure de débit réseau basé sur iperf3.\n" +
                   "Permet de réaliser des tests de débit montant et\n" +
                   "descendant avec gestion de profils, export CSV / JSON\n" +
                   "et journalisation en temps réel.",
            Font      = _fonts.Track(new Font("Segoe UI", 9F)),
            ForeColor = AppColors.TextSecondary,
            AutoSize  = false,
            Width     = ClientSize.Width - 60,
            Height    = 72,
            Top       = separator.Bottom + 14,
            Left      = 30,
            TextAlign = ContentAlignment.TopLeft
        };

        // --- Bouton Fermer -----------------------------------------
        var btnClose = new Button
        {
            Text        = "Fermer",
            Font        = _fonts.Track(new Font("Segoe UI Semibold", 9.5F)),
            Size        = new Size(110, 36),
            ForeColor   = AppColors.CardTextWhite,
            BackColor   = AppColors.Accent,
            FlatStyle   = FlatStyle.Flat,
            DialogResult = DialogResult.OK,
            Cursor      = Cursors.Hand
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Location = new Point(
            (ClientSize.Width - btnClose.Width) / 2,
            lblDesc.Bottom + 16);

        btnClose.Click += (_, _) => Close();

        // --- Assemblage --------------------------------------------
        Controls.AddRange([
            picLogo, lblProduct, lblVersion, lblAuthor,
            separator, lblDesc, btnClose
        ]);

        AcceptButton = btnClose;
    }

    // ---------------------------------------------------------------
    // Libération des ressources
    // ---------------------------------------------------------------

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fonts.Dispose();
            // Libérer le bitmap du PictureBox s'il a été chargé
            foreach (Control c in Controls)
                if (c is PictureBox pb)
                    pb.Image?.Dispose();
        }
        base.Dispose(disposing);
    }
}
