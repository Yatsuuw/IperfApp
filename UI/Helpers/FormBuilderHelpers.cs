using IperfApp.UI.Constants;

namespace IperfApp.UI.Helpers;

/// <summary>
/// Helpers de construction de formulaires WinForms partagés entre
/// <c>MainForm</c> et <c>SettingsForm</c>.
/// </summary>
internal static class FormBuilderHelpers
{
    public static TextBox AddInputField(
        Panel panel, string label, TextBox tb,
        ref int top, FontTracker fonts)
    {
        var lbl = new Label
        {
            Text      = label,
            Top       = top,
            Left      = 25,
            Font      = fonts.Track(new Font("Segoe UI", 7F, FontStyle.Bold)),
            ForeColor = AppColors.Accent,
            AutoSize  = true
        };

        tb.Top         = top + 18;
        tb.Left        = 25;
        tb.Width       = 260;
        tb.Font        = fonts.Track(new Font("Segoe UI Semibold", 9.5F));
        tb.BorderStyle = BorderStyle.None;

        var line = new Panel
        {
            Top       = tb.Bottom + 4,
            Left      = 25,
            Width     = 260,
            Height    = 1,
            BackColor = AppColors.FieldBorder
        };

        tb.Enter += (_, _) => line.BackColor = AppColors.FieldBorderFocus;
        tb.Leave += (_, _) => line.BackColor = AppColors.FieldBorder;

        panel.Controls.AddRange([lbl, tb, line]);
        top += 55;
        return tb;
    }

    public static TextBox AddNumericField(
        Panel panel, string label, TextBox tb,
        ref int top, FontTracker fonts)
    {
        AddInputField(panel, label, tb, ref top, fonts);
        tb.KeyPress += (_, e) =>
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        };
        return tb;
    }

    /// <summary>
    /// Ajoute une ligne label + ComboBox pré-rempli avec les choix de version IP.
    /// <para><b>Usage exclusif :</b> champ cbIpVersion uniquement.</para>
    /// </summary>
    public static ComboBox AddIpVersionField(
        Panel panel, string label, ComboBox cb,
        ref int top, FontTracker fonts)
    {
        var lbl = new Label
        {
            Text      = label,
            Top       = top,
            Left      = 25,
            Font      = fonts.Track(new Font("Segoe UI", 7F, FontStyle.Bold)),
            ForeColor = AppColors.Accent,
            AutoSize  = true
        };

        cb.Top           = top + 18;
        cb.Left          = 25;
        cb.Width         = 260;
        cb.Font          = fonts.Track(new Font("Segoe UI Semibold", 9.5F));
        cb.DropDownStyle = ComboBoxStyle.DropDownList;
        cb.FlatStyle     = FlatStyle.Standard;
        cb.Items.Clear();
        cb.Items.AddRange(["Auto (défaut)", "IPv4 (-4)", "IPv6 (-6)"]);
        cb.SelectedIndex = 0;

        var line = new Panel
        {
            Top       = cb.Bottom + 4,
            Left      = 25,
            Width     = 260,
            Height    = 1,
            BackColor = AppColors.FieldBorder
        };

        cb.Enter += (_, _) => line.BackColor = AppColors.FieldBorderFocus;
        cb.Leave += (_, _) => line.BackColor = AppColors.FieldBorder;

        panel.Controls.AddRange([lbl, cb, line]);
        top += 55;
        return cb;
    }
}
