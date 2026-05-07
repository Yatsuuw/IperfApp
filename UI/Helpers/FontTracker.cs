namespace IperfApp.UI.Helpers;

/// <summary>
/// Centralise le suivi et la libération de toutes les instances <see cref="Font"/>
/// créées inline dans les builders de Form.
/// <para>
/// Utilisation :
/// <code>
/// var f = _fonts.Track(new Font("Segoe UI", 9F));
/// control.Font = f;
/// </code>
/// Toutes les fontes enregistrées sont libérées dans <see cref="Dispose"/>.
/// </para>
/// </summary>
internal sealed class FontTracker : IDisposable
{
    private readonly List<Font> _fonts = [];
    private bool _disposed;

    /// <summary>Enregistre <paramref name="font"/> et la retourne pour affectation fluide.</summary>
    /// <exception cref="ObjectDisposedException">Si le tracker a déjà été disposé.</exception>
    public Font Track(Font font)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(font);
        _fonts.Add(font);
        return font;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var f in _fonts)
            f.Dispose();
        _fonts.Clear();
    }
}
