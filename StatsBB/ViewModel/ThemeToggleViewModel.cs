using System.ComponentModel;
using System.Runtime.CompilerServices;
using StatsBB.MVVM;
using StatsBB.Services;

namespace StatsBB.ViewModel;

public class ThemeToggleViewModel : INotifyPropertyChanged
{
    private readonly ThemeManager _themeManager;
    
    public ThemeToggleViewModel()
    {
        _themeManager = ThemeManager.Instance;
        
        // Subscribe to theme changes
        _themeManager.PropertyChanged += (s, e) =>
        {
            OnPropertyChanged(e.PropertyName);
        };
        
        ToggleThemeCommand = new RelayCommand(_ => _themeManager.ToggleTheme());
    }

    public AppTheme CurrentTheme
    {
        get => _themeManager.CurrentTheme;
        set => _themeManager.CurrentTheme = value;
    }

    public bool IsDarkTheme => _themeManager.IsDarkTheme;
    public bool IsLightTheme => _themeManager.IsLightTheme;

    public RelayCommand ToggleThemeCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}