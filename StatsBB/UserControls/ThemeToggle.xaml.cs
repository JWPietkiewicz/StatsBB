using System.Windows.Controls;
using StatsBB.ViewModel;

namespace StatsBB.UserControls
{
    public partial class ThemeToggle : UserControl
    {
        public ThemeToggle()
        {
            InitializeComponent();
            DataContext = new ThemeToggleViewModel();
        }
    }
}