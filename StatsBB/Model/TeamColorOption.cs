using System.Windows.Media;

namespace StatsBB.Model;

public class TeamColorOption
{
    public string Name { get; }
    public Brush ColorBrush { get; }
    public Brush TextBrush { get; }

    public TeamColorOption(string name, string colorHex, string textHex)
    {
        Name = name;
        ColorBrush = (Brush)new BrushConverter().ConvertFromString(colorHex)!;
        TextBrush = (Brush)new BrushConverter().ConvertFromString(textHex)!;
    }
}
