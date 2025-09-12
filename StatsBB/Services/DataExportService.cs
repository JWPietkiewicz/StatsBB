using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StatsBB.Domain;
using Microsoft.Win32;

namespace StatsBB.Services;

/// <summary>
/// Service for exporting game data and statistics to various formats
/// </summary>
public class DataExportService
{
    /// <summary>
    /// Export formats supported by the application
    /// </summary>
    public enum ExportFormat
    {
        CSV,
        JSON,
        TXT
    }

    /// <summary>
    /// Export game statistics to the specified format
    /// </summary>
    /// <param name="game">Game data to export</param>
    /// <param name="format">Export format</param>
    /// <param name="includeDetailedStats">Include detailed player statistics</param>
    /// <returns>Success status and file path if successful</returns>
    public static async Task<(bool Success, string FilePath, string ErrorMessage)> ExportGameAsync(
        Game game, ExportFormat format, bool includeDetailedStats = true)
    {
        try
        {
            var saveDialog = new SaveFileDialog
            {
                Title = "Export Game Statistics",
                Filter = GetFileFilter(format),
                DefaultExt = GetFileExtension(format),
                FileName = GenerateDefaultFileName(game, format)
            };

            if (SettingsService.Current.LastUsedDirectory != null && 
                Directory.Exists(SettingsService.Current.LastUsedDirectory))
            {
                saveDialog.InitialDirectory = SettingsService.Current.LastUsedDirectory;
            }

            if (saveDialog.ShowDialog() == true)
            {
                var filePath = saveDialog.FileName;
                SettingsService.Current.LastUsedDirectory = Path.GetDirectoryName(filePath);
                await SettingsService.SaveSettingsAsync();

                string content = format switch
                {
                    ExportFormat.CSV => GenerateCSVContent(game, includeDetailedStats),
                    ExportFormat.JSON => GenerateJSONContent(game, includeDetailedStats),
                    ExportFormat.TXT => GenerateTextContent(game, includeDetailedStats),
                    _ => throw new ArgumentException($"Unsupported export format: {format}")
                };

                await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
                return (true, filePath, string.Empty);
            }

            return (false, string.Empty, "Export cancelled by user.");
        }
        catch (Exception ex)
        {
            return (false, string.Empty, $"Export failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Generate CSV content for game statistics
    /// </summary>
    private static string GenerateCSVContent(Game game, bool includeDetailedStats)
    {
        var csv = new StringBuilder();
        
        // Game header information
        csv.AppendLine("Basketball Game Statistics Export");
        csv.AppendLine($"Export Date:,{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        csv.AppendLine($"Home Team:,{game.HomeTeam?.TeamName ?? "Unknown"}");
        csv.AppendLine($"Away Team:,{game.AwayTeam?.TeamName ?? "Unknown"}");
        csv.AppendLine($"Final Score:,{game.HomeTeam?.Points ?? 0} - {game.AwayTeam?.Points ?? 0}");
        csv.AppendLine();

        // Period scores
        csv.AppendLine("Period Scores");
        csv.AppendLine("Period,Home,Away");
        foreach (var period in game.Periods)
        {
            csv.AppendLine($"{period.Name},{period.HomePeriodScore},{period.AwayPeriodScore}");
        }
        csv.AppendLine();

        if (includeDetailedStats)
        {
            // Home team player statistics
            ExportTeamStats(csv, game.HomeTeam, "Home Team");
            
            // Away team player statistics
            ExportTeamStats(csv, game.AwayTeam, "Away Team");
        }

        return csv.ToString();
    }

    /// <summary>
    /// Export team statistics to CSV format
    /// </summary>
    private static void ExportTeamStats(StringBuilder csv, Team? team, string teamLabel)
    {
        if (team == null) return;

        csv.AppendLine($"{teamLabel} Player Statistics");
        csv.AppendLine("Number,First Name,Last Name,Points,Field Goals Made,Field Goals Attempted,3PT Made,3PT Attempted,Free Throws Made,Free Throws Attempted,Rebounds,Offensive Rebounds,Defensive Rebounds,Assists,Steals,Blocks,Turnovers,Personal Fouls,Captain");
        
        var sortedPlayers = team.Players.Where(p => !string.IsNullOrEmpty(p.FirstName) || !string.IsNullOrEmpty(p.LastName)).OrderBy(p => p.Number);
        
        foreach (var player in sortedPlayers)
        {
            csv.AppendLine($"{player.Number},\"{player.FirstName}\",\"{player.LastName}\",{player.Points},{player.FieldGoalsMade},{player.FieldGoalsAttempted},{player.ShotsMade3pt},{player.ShotAttempts3pt},{player.FreeThrowsMade},{player.FreeThrowsAttempted},{player.Rebounds},{player.OffensiveRebounds},{player.DefensiveRebounds},{player.Assists},{player.Steals},{player.Blocks},{player.Turnovers},{player.FoulsCommitted},{(player.IsCaptain ? "Yes" : "No")}");
        }
        
        // Team totals
        var activePlayers = team.Players.Where(p => p.Points > 0 || p.FieldGoalsAttempted > 0).ToList();
        csv.AppendLine($"TOTALS,,,{activePlayers.Sum(p => p.Points)},{activePlayers.Sum(p => p.FieldGoalsMade)},{activePlayers.Sum(p => p.FieldGoalsAttempted)},{activePlayers.Sum(p => p.ShotsMade3pt)},{activePlayers.Sum(p => p.ShotAttempts3pt)},{activePlayers.Sum(p => p.FreeThrowsMade)},{activePlayers.Sum(p => p.FreeThrowsAttempted)},{activePlayers.Sum(p => p.Rebounds)},{activePlayers.Sum(p => p.OffensiveRebounds)},{activePlayers.Sum(p => p.DefensiveRebounds)},{activePlayers.Sum(p => p.Assists)},{activePlayers.Sum(p => p.Steals)},{activePlayers.Sum(p => p.Blocks)},{activePlayers.Sum(p => p.Turnovers)},{activePlayers.Sum(p => p.FoulsCommitted)},");
        csv.AppendLine();
    }

    /// <summary>
    /// Generate JSON content for game statistics
    /// </summary>
    private static string GenerateJSONContent(Game game, bool includeDetailedStats)
    {
        var exportData = new
        {
            ExportInfo = new
            {
                ExportDate = DateTime.Now,
                Format = "JSON",
                IncludeDetailedStats = includeDetailedStats
            },
            GameInfo = new
            {
                HomeTeam = game.HomeTeam?.TeamName ?? "Unknown",
                AwayTeam = game.AwayTeam?.TeamName ?? "Unknown",
                FinalScore = new
                {
                    Home = game.HomeTeam?.Points ?? 0,
                    Away = game.AwayTeam?.Points ?? 0
                }
            },
            PeriodScores = game.Periods.Select(p => new
            {
                PeriodName = p.Name,
                p.HomePeriodScore,
                p.AwayPeriodScore
            }).ToList(),
            PlayerStatistics = includeDetailedStats ? new
            {
                HomeTeam = ExportTeamStatsToJson(game.HomeTeam),
                AwayTeam = ExportTeamStatsToJson(game.AwayTeam)
            } : null
        };

        return JsonSerializer.Serialize(exportData, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// Export team statistics to JSON format
    /// </summary>
    private static object ExportTeamStatsToJson(Team? team)
    {
        if (team == null) return new { };

        var players = team.Players
            .Where(p => !string.IsNullOrEmpty(p.FirstName) || !string.IsNullOrEmpty(p.LastName))
            .OrderBy(p => p.Number)
            .Select(p => new
            {
                p.Number,
                p.FirstName,
                p.LastName,
                p.Points,
                FieldGoals = new { p.FieldGoalsMade, p.FieldGoalsAttempted },
                ThreePointers = new { Made = p.ShotsMade3pt, Attempted = p.ShotAttempts3pt },
                FreeThrows = new { p.FreeThrowsMade, p.FreeThrowsAttempted },
                Rebounds = new { Total = p.Rebounds, p.OffensiveRebounds, p.DefensiveRebounds },
                p.Assists,
                p.Steals,
                p.Blocks,
                p.Turnovers,
                p.FoulsCommitted,
                p.IsCaptain
            });

        var activePlayers = team.Players.Where(p => p.Points > 0 || p.FieldGoalsAttempted > 0).ToList();
        var totals = new
        {
            Points = activePlayers.Sum(p => p.Points),
            FieldGoals = new
            {
                Made = activePlayers.Sum(p => p.FieldGoalsMade),
                Attempted = activePlayers.Sum(p => p.FieldGoalsAttempted)
            },
            ThreePointers = new
            {
                Made = activePlayers.Sum(p => p.ShotsMade3pt),
                Attempted = activePlayers.Sum(p => p.ShotAttempts3pt)
            },
            FreeThrows = new
            {
                Made = activePlayers.Sum(p => p.FreeThrowsMade),
                Attempted = activePlayers.Sum(p => p.FreeThrowsAttempted)
            },
            Rebounds = new
            {
                Total = activePlayers.Sum(p => p.Rebounds),
                Offensive = activePlayers.Sum(p => p.OffensiveRebounds),
                Defensive = activePlayers.Sum(p => p.DefensiveRebounds)
            },
            Assists = activePlayers.Sum(p => p.Assists),
            Steals = activePlayers.Sum(p => p.Steals),
            Blocks = activePlayers.Sum(p => p.Blocks),
            Turnovers = activePlayers.Sum(p => p.Turnovers),
            Fouls = activePlayers.Sum(p => p.FoulsCommitted)
        };

        return new
        {
            TeamName = team.TeamName,
            Players = players,
            Totals = totals
        };
    }

    /// <summary>
    /// Generate formatted text content for game statistics
    /// </summary>
    private static string GenerateTextContent(Game game, bool includeDetailedStats)
    {
        var text = new StringBuilder();

        // Header
        text.AppendLine("BASKETBALL GAME STATISTICS");
        text.AppendLine("=".PadRight(50, '='));
        text.AppendLine();
        text.AppendLine($"Export Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        text.AppendLine($"Home Team: {game.HomeTeam?.TeamName ?? "Unknown"}");
        text.AppendLine($"Away Team: {game.AwayTeam?.TeamName ?? "Unknown"}");
        text.AppendLine();

        // Final Score
        text.AppendLine("FINAL SCORE");
        text.AppendLine("-".PadRight(20, '-'));
        text.AppendLine($"{game.HomeTeam?.TeamName ?? "Home"}: {game.HomeTeam?.Points ?? 0}");
        text.AppendLine($"{game.AwayTeam?.TeamName ?? "Away"}: {game.AwayTeam?.Points ?? 0}");
        text.AppendLine();

        // Period Scores
        text.AppendLine("PERIOD SCORES");
        text.AppendLine("-".PadRight(20, '-'));
        foreach (var period in game.Periods)
        {
            text.AppendLine($"{period.Name}: {period.HomePeriodScore} - {period.AwayPeriodScore}");
        }
        text.AppendLine();

        if (includeDetailedStats)
        {
            ExportTeamStatsToText(text, game.HomeTeam, "HOME TEAM");
            ExportTeamStatsToText(text, game.AwayTeam, "AWAY TEAM");
        }

        return text.ToString();
    }

    /// <summary>
    /// Export team statistics to formatted text
    /// </summary>
    private static void ExportTeamStatsToText(StringBuilder text, Team? team, string teamLabel)
    {
        if (team == null) return;

        text.AppendLine($"{teamLabel} STATISTICS");
        text.AppendLine("-".PadRight(50, '-'));
        text.AppendLine();

        var players = team.Players
            .Where(p => !string.IsNullOrEmpty(p.FirstName) || !string.IsNullOrEmpty(p.LastName))
            .OrderBy(p => p.Number);

        foreach (var player in players)
        {
            text.AppendLine($"#{player.Number} {player.FirstName} {player.LastName}".Trim() + (player.IsCaptain ? " (C)" : ""));
            text.AppendLine($"  Points: {player.Points}");
            text.AppendLine($"  Field Goals: {player.FieldGoalsMade}/{player.FieldGoalsAttempted}");
            text.AppendLine($"  3-Pointers: {player.ShotsMade3pt}/{player.ShotAttempts3pt}");
            text.AppendLine($"  Free Throws: {player.FreeThrowsMade}/{player.FreeThrowsAttempted}");
            text.AppendLine($"  Rebounds: {player.Rebounds} (Off: {player.OffensiveRebounds}, Def: {player.DefensiveRebounds})");
            text.AppendLine($"  Assists: {player.Assists}, Steals: {player.Steals}, Blocks: {player.Blocks}");
            text.AppendLine($"  Turnovers: {player.Turnovers}, Fouls: {player.FoulsCommitted}");
            text.AppendLine();
        }
    }

    /// <summary>
    /// Get file filter for SaveFileDialog
    /// </summary>
    private static string GetFileFilter(ExportFormat format) => format switch
    {
        ExportFormat.CSV => "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
        ExportFormat.JSON => "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
        ExportFormat.TXT => "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
        _ => "All Files (*.*)|*.*"
    };

    /// <summary>
    /// Get file extension for format
    /// </summary>
    private static string GetFileExtension(ExportFormat format) => format switch
    {
        ExportFormat.CSV => ".csv",
        ExportFormat.JSON => ".json",
        ExportFormat.TXT => ".txt",
        _ => ".txt"
    };

    /// <summary>
    /// Generate default file name for export
    /// </summary>
    private static string GenerateDefaultFileName(Game game, ExportFormat format)
    {
        var homeTeam = game.HomeTeam?.TeamName?.Replace(" ", "_") ?? "Home";
        var awayTeam = game.AwayTeam?.TeamName?.Replace(" ", "_") ?? "Away";
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        var extension = GetFileExtension(format);
        
        return $"Game_{homeTeam}_vs_{awayTeam}_{date}{extension}";
    }
}