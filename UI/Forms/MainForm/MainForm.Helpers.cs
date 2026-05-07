using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>
    /// Crée un bouton « ghost » (contour fin, fond Card, désactivé par défaut).
    /// La <see cref="Font"/> allouée est enregistrée dans <see cref="_fonts"/>.
    /// </summary>
    private Button CreateGhostButton(string text, int top, int left, int width)
    {
        var btn = new Button
        {
            Text      = text,
            Top       = top,
            Left      = left,
            Width     = width,
            Height    = 45,
            FlatStyle = FlatStyle.Flat,
            BackColor = AppColors.Card,
            Enabled   = false,
            Font      = _fonts.Track(new Font("Segoe UI", 9F)),
            Cursor    = Cursors.Hand
        };
        btn.FlatAppearance.BorderColor = AppColors.ExportBorderDisabled;
        return btn;
    }
}
