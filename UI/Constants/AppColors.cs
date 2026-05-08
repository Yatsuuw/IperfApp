namespace IperfApp.UI.Constants;

/// <summary>
/// Palette de couleurs centralisée de l'application.
/// Source unique de vérité — ne jamais écrire de valeurs ARGB en dur ailleurs.
/// </summary>
internal static class AppColors
{
    // ── Surfaces ──────────────────────────────────────────────────────────────

    /// <summary>Fond général de la fenêtre principale.</summary>
    public static readonly Color Background           = Color.FromArgb(240, 243, 247);

    /// <summary>Fond des cartes / panneaux blancs.</summary>
    public static readonly Color Card                 = Color.White;

    /// <summary>Fond sombre de la console de logs (style terminal).</summary>
    public static readonly Color Terminal             = Color.FromArgb( 28,  28,  30);

    /// <summary>Fond du panneau latéral de la fenêtre Profils.</summary>
    public static readonly Color SidePanel            = Color.FromArgb(242, 245, 248);

    /// <summary>Fond d'un champ verrouillé (ReadOnly).</summary>
    public static readonly Color FieldLocked          = Color.FromArgb(248, 248, 248);

    // ── Texte ─────────────────────────────────────────────────────────────────

    /// <summary>Texte principal des champs de saisie.</summary>
    public static readonly Color FieldText            = Color.FromArgb( 30,  30,  30);

    /// <summary>Texte secondaire atténué (labels, menus non-actifs).</summary>
    public static readonly Color TextMuted            = Color.DimGray;

    /// <summary>Texte d'un item non sélectionné dans la ListBox des profils.</summary>
    public static readonly Color TextSecondary        = Color.FromArgb( 80,  80,  80);

    /// <summary>Texte d'un champ verrouillé (ReadOnly).</summary>
    public static readonly Color TextLocked           = Color.FromArgb(160, 160, 160);

    /// <summary>Texte de la console de logs (fond sombre).</summary>
    public static readonly Color LogText              = Color.FromArgb(220, 220, 220);

    // ── Accents ───────────────────────────────────────────────────────────────

    /// <summary>Couleur d'accent principale (bleu Windows).</summary>
    public static readonly Color Accent               = Color.FromArgb(  0, 120, 215);

    /// <summary>Accent grisé affiché pendant un test en cours.</summary>
    public static readonly Color AccentDisabled       = Color.FromArgb(160, 174, 192);

    /// <summary>Rouge danger (bouton Annuler, erreurs).</summary>
    public static readonly Color Danger               = Color.FromArgb(196,  43,  28);

    /// <summary>Vert succès (feedback bouton Enregistrer).</summary>
    public static readonly Color Success              = Color.FromArgb( 40, 167, 100);

    // ── Bordures ──────────────────────────────────────────────────────────────

    /// <summary>Bordure de la carte de configuration principale.</summary>
    public static readonly Color CardBorder           = Color.FromArgb(220, 225, 232);

    /// <summary>Bordure inactive des champs Settings.</summary>
    public static readonly Color FieldBorder          = Color.FromArgb(210, 212, 215);

    /// <summary>Bordure active (focus) des champs Settings.</summary>
    public static readonly Color FieldBorderFocus     = Color.FromArgb(  0, 120, 215);

    /// <summary>Bordure des boutons ＋ / － du panneau latéral Settings.</summary>
    public static readonly Color SideBtnBorder        = Color.FromArgb(210, 210, 210);

    /// <summary>Séparateur entre items de la ListBox des profils.</summary>
    public static readonly Color ListDivider          = Color.FromArgb(225, 228, 232);

    /// <summary>Bordure des boutons d'export CSV quand désactivés.</summary>
    public static readonly Color ExportBorderDisabled = Color.FromArgb(210, 220, 230);
}
