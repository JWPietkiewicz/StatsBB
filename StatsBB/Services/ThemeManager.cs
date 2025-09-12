using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ModernWpf;

namespace StatsBB.Services;

public enum AppTheme
{
    Light,
    Dark,
    System
}

public class ThemeManager : INotifyPropertyChanged
{
    private static ThemeManager? _instance;
    public static ThemeManager Instance => _instance ??= new ThemeManager();

    private AppTheme _currentTheme = AppTheme.Light;
    
    public AppTheme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                ApplyTheme(value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsDarkTheme));
                OnPropertyChanged(nameof(IsLightTheme));
            }
        }
    }

    public bool IsDarkTheme => CurrentTheme == AppTheme.Dark || 
                               (CurrentTheme == AppTheme.System && IsSystemDarkTheme());
    
    public bool IsLightTheme => !IsDarkTheme;

    private ThemeManager()
    {
        // Initialize with light theme by default
        ApplyTheme(_currentTheme);
    }

    private void ApplyTheme(AppTheme theme)
    {
        try
        {
            // Update ModernWpf theme first
            var themeResources = Application.Current.Resources.MergedDictionaries
                .OfType<ModernWpf.ThemeResources>()
                .FirstOrDefault();
                
            if (themeResources != null)
            {
                themeResources.RequestedTheme = theme switch
                {
                    AppTheme.Light => ApplicationTheme.Light,
                    AppTheme.Dark => ApplicationTheme.Dark,
                    AppTheme.System => null, // Use system default
                    _ => ApplicationTheme.Dark
                };
            }
            
            // Update our custom theme resources
            UpdateCustomThemeResources();
            
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme, IsDarkTheme));
        }
        catch (Exception ex)
        {
            // Fallback: just update our custom resources
            UpdateCustomThemeResources();
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme, IsDarkTheme));
        }
    }

    private void UpdateCustomThemeResources()
    {
        var resources = Application.Current.Resources;
        
        if (IsDarkTheme)
        {
            // Dark theme colors
            resources["SystemBackgroundColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 30, 30));
            resources["SystemSurfaceColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48));
            resources["SystemControlBackgroundColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(63, 63, 70));
            resources["SystemControlHoverColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(72, 72, 72));
            resources["SystemControlPressedColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(82, 82, 82));
            resources["SystemTextColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            resources["SystemTextSecondaryColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 200, 200));
            resources["SystemBorderColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(90, 90, 90));
            resources["DisabledColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(90, 90, 90));
        }
        else
        {
            // Light theme colors
            resources["SystemBackgroundColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            resources["SystemSurfaceColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(249, 249, 249));
            resources["SystemControlBackgroundColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(245, 245, 245));
            resources["SystemControlHoverColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(230, 230, 230));
            resources["SystemControlPressedColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(210, 210, 210));
            resources["SystemTextColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            resources["SystemTextSecondaryColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(96, 96, 96));
            resources["SystemBorderColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 200, 200));
            resources["DisabledColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 200, 200));
        }
        
        // Semantic colors remain the same for both themes
        resources["SuccessColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 179, 115));
        resources["ErrorColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(216, 59, 1));
        resources["WarningColor"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(247, 184, 1));
    }

    private static bool IsSystemDarkTheme()
    {
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            return value is int intValue && intValue == 0;
        }
        catch
        {
            return false; // Default to light if we can't determine
        }
    }

    public void ToggleTheme()
    {
        CurrentTheme = CurrentTheme switch
        {
            AppTheme.Light => AppTheme.Dark,
            AppTheme.Dark => AppTheme.Light,
            AppTheme.System => AppTheme.Light,
            _ => AppTheme.Dark
        };
    }

    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ThemeChangedEventArgs : EventArgs
{
    public AppTheme Theme { get; }
    public bool IsDarkTheme { get; }

    public ThemeChangedEventArgs(AppTheme theme, bool isDarkTheme)
    {
        Theme = theme;
        IsDarkTheme = isDarkTheme;
    }
}