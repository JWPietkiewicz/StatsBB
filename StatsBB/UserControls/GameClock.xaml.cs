using System.Windows;
using System.Windows.Controls;
using StatsBB.ViewModel;

namespace StatsBB.UserControls;

public partial class GameClock : UserControl
{
    public GameClock()
    {
        InitializeComponent();
        DataContext = new GameClockViewModel();
    }
}
