using System.Configuration;
using System.Data;
using System.Windows;
using StatsBB.Services;
using static StatsBB.Services.AppTheme;

namespace StatsBB
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Wait for the application to be fully initialized
            this.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                // Initialize theme manager after UI is ready
                var themeManager = ThemeManager.Instance;
                // Force initial theme application
                themeManager.CurrentTheme = AppTheme.Dark;
            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }
    }

}
