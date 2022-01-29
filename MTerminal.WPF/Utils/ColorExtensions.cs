using System.Windows.Media;

namespace MTerminal.WPF;

/// <summary>
/// Contains extension methods with relation to color.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Sets the color opacity to the specified level.
    /// </summary>
    /// <param name="color">Color to modify.</param>
    /// <param name="multiplier">How opaque color will be: 0 - transparent, 1 - fully opaque.</param>
    public static Color WithOpacity(this Color color, double multiplier)
    {
        var clampedMultiplier = Math.Clamp(multiplier, 0, 1);

        color.A = (byte)(255 * clampedMultiplier);
        return color;
    } 
}
