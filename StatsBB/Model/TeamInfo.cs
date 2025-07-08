using System.Collections.ObjectModel;
using StatsBB.MVVM;

namespace StatsBB.Model;

public class TeamInfo : ViewModelBase
{
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    private string _shortName = string.Empty;
    public string ShortName
    {
        get => _shortName;
        set { _shortName = value; OnPropertyChanged(); }
    }

    private TeamColorOption? _color;
    public TeamColorOption? Color
    {
        get => _color;
        set { _color = value; OnPropertyChanged(); }
    }

    public ObservableCollection<Player> Players { get; } = new();
}
