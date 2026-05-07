using IperfApp.UI.Constants;

namespace IperfApp.UI.Helpers;

/// <summary>
/// Helpers de construction de formulaires WinForms partagés entre
/// <c>MainForm</c> et <c>SettingsForm</c>.
/// <para>
/// Chaque méthode crée les contrôles et enregistre les <see cref="Font"/>
/// allouées dans le <see cref="FontTracker"/> fourni.
/// </para>
/// </summary>
internal static class FormBuilderHelpers
{
    /// <summary>
    /// Ajoute un champ texte avec son label et une ligne de soulignement colorée
    /// dans le panneau <paramref name="panel"/>.
    /// </summary>
    public static TextBox AddInputField(
        Panel panel, string label, TextBox tb,
        ref int top, FontTracker fonts)
    {
        var fLbl = fonts.Track(new Font("Segoe UI", 7F, FontStyle.Bold));
        var lbl  = new Label
        {
            Text      = label,
            Top       = top,
            Left      = 25,
            Font      = fLbl,
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

    /// <summary>
    /// Identique à <see cref="AddInputField"/> avec un filtre numérique
    /// sur <c>KeyPress</c> (chiffres uniquement).
    /// </summary>
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
    /// Ajoute un <see cref="ComboBox"/> avec son label et une ligne de soulignement
    /// dans le panneau <paramref name="panel"/>.
    /// Les items « Auto / IPv4 / IPv6 » sont ajoutés automatiquement.
    /// </summary>
    public static ComboBox AddComboField(
        Panel panel, string label, ComboBox cb,
        ref int top, FontTracker fonts)
    {
        var fLbl = fonts.Track(new Font("Segoe UI", 7F, FontStyle.Bold));
        var lbl  = new Label
        {
            Text      = label,
            Top       = top,
            Left      = 25,
            Font      = fLbl,
            ForeColor = AppColors.Accent,
            AutoSize  = true
        };

        cb.Top           = top + 18;
        cb.Left          = 25;
        cb.Width         = 260;
        cb.Font          = fonts.Track(new Font("Segoe UI Semibold", 9.5F));
        cb.DropDownStyle = ComboBoxStyle.DropDownList;
        cb.FlatStyle     = FlatStyle.Flat;
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
