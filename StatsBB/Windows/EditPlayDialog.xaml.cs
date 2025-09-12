using System.Windows;
using StatsBB.ViewModel;

namespace StatsBB.Windows
{
    /// <summary>
    /// Interaction logic for EditPlayDialog.xaml
    /// </summary>
    public partial class EditPlayDialog : Window
    {
        public PlayActionViewModel? PlayAction { get; private set; }
        public bool WasEdited { get; private set; } = false;

        public EditPlayDialog(PlayActionViewModel playAction)
        {
            InitializeComponent();
            
            // Create a copy for editing to avoid modifying the original until confirmed
            PlayAction = new PlayActionViewModel
            {
                TeamColor = playAction.TeamColor,
                PlayerNumber = playAction.PlayerNumber,
                FirstName = playAction.FirstName,
                LastName = playAction.LastName,
                Action = playAction.Action
            };
            
            DataContext = PlayAction;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            WasEdited = true;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            WasEdited = false;
            DialogResult = false;
            Close();
        }
    }
}