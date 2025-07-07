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

            var vm = new MainWindowViewModel(Resources);

            // 🔗 Connect view model to view
            vm.MarkerRequested += (pos, color, filled) => CourtControl.SetMarker(pos, color, filled);
            vm.TempMarkerRemoved += () => CourtControl.RemoveTemporaryMarker();

            // 📍 On canvas click: set point + show temp white marker
            CourtControl.CourtClick += (s, data) =>
            {
                vm.HandleCourtClick(data);
                CourtControl.ShowTemporaryMarker(data.Point);
            };

            DataContext = vm;
        }

        // Optional if you're using this handler elsewhere
        private void OnCourtClick(object sender, CourtPointData p)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.HandleCourtClick(p);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && DataContext is MainWindowViewModel vm)
            {
                vm.CancelCurrentAction();
                CourtControl.RemoveTemporaryMarker();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Space)
            {
                GameClockControl.Toggle();
                e.Handled = true;
            }
        }
    }

}
