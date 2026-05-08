namespace IperfApp.UI.Constants;

/// <summary>
/// Noms de familles de fontes centralisés.
/// Source unique de vérité — ne jamais écrire les noms de fonte en dur ailleurs.
/// <para>
/// Les <see cref="Font"/> elles-mêmes ne sont pas des <c>static readonly</c> ici
/// car leur durée de vie doit être gérée par <c>FontTracker</c> pour éviter les fuites GDI.
/// Ce fichier centralise uniquement les <em>noms</em> et les <em>tailles de référence</em>.
/// </para>
/// </summary>
internal static class AppFonts
{
    // ── Familles ──────────────────────────────────────────────────────────────

    /// <summary>Police principale de l'interface (labels, boutons, champs).</summary>
    public const string Name         = "Segoe UI";

    /// <summary>Variante semi-grasse (titres de sections, labels d'accent).</summary>
    public const string SemiBoldName = "Segoe UI Semibold";

    /// <summary>Variante display grand titre (fenêtres principales).</summary>
    public const string DisplayName  = "Segoe UI Variable Display";

    /// <summary>Police à espacement fixe (console de logs).</summary>
    public const string MonoName     = "Consolas";

    /// <summary>Variante grasse (boutons d'action principaux).</summary>
    public const string BoldName     = "Segoe UI Bold";

    // ── Tailles de référence ──────────────────────────────────────────────────

    /// <summary>Taille des petits labels (catégories, en-têtes de champ).</summary>
    public const float SizeSmall     = 7F;

    /// <summary>Taille standard des champs, boutons et éléments UI courants.</summary>
    public const float SizeBase      = 9F;

    /// <summary>Taille intermédiaire (nav, menus, boutons secondaires).</summary>
    public const float SizeMedium    = 9.5F;

    /// <summary>Taille des valeurs de champs de saisie.</summary>
    public const float SizeInput     = 10F;

    /// <summary>Taille des valeurs de champs de saisie principale (MainForm).</summary>
    public const float SizeInputLg   = 10.5F;

    /// <summary>Taille des titres de section (SettingsForm header).</summary>
    public const float SizeTitle     = 14F;

    /// <summary>Taille du titre principal de la fenêtre.</summary>
    public const float SizeHero      = 18F;

    /// <summary>Taille du bouton Lancer l'analyse.</summary>
    public const float SizeAction    = 11.5F;
}
