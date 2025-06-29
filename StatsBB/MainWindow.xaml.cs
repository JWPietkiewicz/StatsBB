// Cleaned and organized version of MainWindow.xaml.cs

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StatsBB.Model;
using StatsBB.UserControls;
using StatsBB.ViewModel;

namespace StatsBB
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel vm = new MainWindowViewModel(Resources);
            DataContext = vm;
            CourtControl.CourtClick += OnCourtClick;
        }

        private void OnCourtClick(object sender, CourtPointData p)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.SelectedPoint = p;
            }
        }
    }
}
