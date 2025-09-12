using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace StatsBB.Services;

/// <summary>
/// Service for managing application settings and user preferences
/// </summary>
public class SettingsService
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "StatsBB");
    
    private static readonly string SettingsFile = Path.Combine(SettingsDirectory, "settings.json");
    
    private static AppSettings? _currentSettings;
    
    /// <summary>
    /// Gets the current application settings
    /// </summary>
    public static AppSettings Current => _currentSettings ??= LoadSettings();
    
    /// <summary>
    /// Loads settings from file or creates default settings
    /// </summary>
    /// <returns>Application settings</returns>
    public static AppSettings LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsFile))
            {
                var json = File.ReadAllText(SettingsFile);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                return settings ?? new AppSettings();
            }
        }
        catch (Exception)
        {
            // If loading fails, return default settings
        }
        
        return new AppSettings();
    }
    
    /// <summary>
    /// Saves current settings to file
    /// </summary>
    /// <returns>Task representing the save operation</returns>
    public static async Task SaveSettingsAsync()
    {
        try
        {
            Directory.CreateDirectory(SettingsDirectory);
            
            var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            await File.WriteAllTextAsync(SettingsFile, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Saves settings synchronously
    /// </summary>
    public static void SaveSettings()
    {
        try
        {
            Directory.CreateDirectory(SettingsDirectory);
            
            var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            File.WriteAllText(SettingsFile, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Resets settings to default values
    /// </summary>
    public static void ResetToDefaults()
    {
        _currentSettings = new AppSettings();
        SaveSettings();
    }
}

/// <summary>
/// Application settings model
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Default game duration in minutes
    /// </summary>
    public int DefaultGameDurationMinutes { get; set; } = 48;
    
    /// <summary>
    /// Number of periods in a game
    /// </summary>
    public int NumberOfPeriods { get; set; } = 4;
    
    /// <summary>
    /// Auto-save interval in minutes
    /// </summary>
    public int AutoSaveIntervalMinutes { get; set; } = 5;
    
    /// <summary>
    /// Enable auto-save functionality
    /// </summary>
    public bool AutoSaveEnabled { get; set; } = true;
    
    /// <summary>
    /// Show confirmation dialogs for critical actions
    /// </summary>
    public bool ShowConfirmationDialogs { get; set; } = true;
    
    /// <summary>
    /// Default team A color (hex format)
    /// </summary>
    public string DefaultTeamAColor { get; set; } = "#FF6B35";
    
    /// <summary>
    /// Default team B color (hex format)
    /// </summary>
    public string DefaultTeamBColor { get; set; } = "#2E8B57";
    
    /// <summary>
    /// Enable sound effects
    /// </summary>
    public bool SoundEffectsEnabled { get; set; } = true;
    
    /// <summary>
    /// Sound volume (0.0 to 1.0)
    /// </summary>
    public double SoundVolume { get; set; } = 0.5;
    
    /// <summary>
    /// Enable keyboard shortcuts
    /// </summary>
    public bool KeyboardShortcutsEnabled { get; set; } = true;
    
    /// <summary>
    /// Last used directory for file operations
    /// </summary>
    public string LastUsedDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    
    /// <summary>
    /// Export format preference (PDF, Excel, CSV)
    /// </summary>
    public string PreferredExportFormat { get; set; } = "PDF";
    
    /// <summary>
    /// Include detailed statistics in exports
    /// </summary>
    public bool IncludeDetailedStats { get; set; } = true;
    
    /// <summary>
    /// Maximum number of recent files to remember
    /// </summary>
    public int MaxRecentFiles { get; set; } = 10;
    
    /// <summary>
    /// Enable advanced statistics calculations
    /// </summary>
    public bool EnableAdvancedStats { get; set; } = true;
    
    /// <summary>
    /// Show tooltips and help text
    /// </summary>
    public bool ShowHelpTooltips { get; set; } = true;
    
    /// <summary>
    /// Application theme preference (Light, Dark, Auto)
    /// </summary>
    public string ThemePreference { get; set; } = "Light";
    
    /// <summary>
    /// Check for updates on startup
    /// </summary>
    public bool CheckForUpdatesOnStartup { get; set; } = true;
    
    /// <summary>
    /// Language/locale preference
    /// </summary>
    public string Language { get; set; } = "en-US";
}