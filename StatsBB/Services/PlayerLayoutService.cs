using StatsBB.Domain;
using StatsBB.Model;
using StatsBB.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace StatsBB.Services;

public static class PlayerLayoutService
{
    /*
    public static void PopulateTeams(ObservableCollection<Player> players)
    {
        players.Add(new Player() { Id = 1, Name = "Jabril Durham", Number = 1, IsTeamA = true, IsActive = true });
        players.Add(new Player() { Id = 2, Name = "Joel Ćwik", Number = 4, IsTeamA = true, IsActive = false });
        players.Add(new Player() { Id = 3, Name = "Wiktor Sewioł", Number = 6, IsTeamA = true, IsActive = false });
        players.Add(new Player() { Id = 4, Name = "Łukasz Kolenda", Number = 10, IsTeamA = true, IsActive = true });
        players.Add(new Player() { Id = 5, Name = "Grzegorz Kamiński", Number = 11, IsTeamA = true, IsActive = true });
        players.Add(new Player() { Id = 6, Name = "Filip Kowalczyk", Number = 14, IsTeamA = true, IsActive = false });
        players.Add(new Player() { Id = 7, Name = "Daniel Szymkiewicz", Number = 22, IsTeamA = true, IsActive = false });
        players.Add(new Player() { Id = 8, Name = "Jakub Garbacz", Number = 30, IsTeamA = true, IsActive = true });
        players.Add(new Player() { Id = 9, Name = "Adam Hrycaniuk", Number = 34, IsTeamA = true, IsActive = false });
        players.Add(new Player() { Id = 10, Name = "Stefan Djordjević", Number = 45, IsTeamA = true, IsActive = true });

        players.Add(new Player() { Id = 11, Name = "Bartosz Jankowski", Number = 0, IsTeamA = false, IsActive = false });
        players.Add(new Player() { Id = 12, Name = "Aaron Best", Number = 3, IsTeamA = false, IsActive = true });
        players.Add(new Player() { Id = 13, Name = "Robert Trey Mcgowens III", Number = 5, IsTeamA = false, IsActive = true });
        players.Add(new Player() { Id = 14, Name = "Wiktor Jaszczerski", Number = 9, IsTeamA = false, IsActive = false });
        players.Add(new Player() { Id = 15, Name = "Mikołaj Witliński", Number = 12, IsTeamA = false, IsActive = true });
        players.Add(new Player() { Id = 16, Name = "Jarosław Zyskowski", Number = 15, IsTeamA = false, IsActive = false });
        players.Add(new Player() { Id = 17, Name = "Tarik Phillip", Number = 22, IsTeamA = false, IsActive = true });
        players.Add(new Player() { Id = 18, Name = "Geoffrey Groselle", Number = 32, IsTeamA = false, IsActive = false });
        players.Add(new Player() { Id = 19, Name = "Marcus Weathers", Number = 50, IsTeamA = false, IsActive = true });
        players.Add(new Player() { Id = 20, Name = "Jakub Schenk", Number = 55, IsTeamA = false, IsActive = false });
        players.Add(new Player() { Id = 21, Name = "Andy Van Vliet", Number = 81, IsTeamA = false, IsActive = false });
    }
    */
    public static ObservableCollection<PlayerPositionViewModel> CreatePositionedPlayers(
        IEnumerable<Player> players,
        ResourceDictionary resources,
        Action<Player> onSelect)
    {
        var list = new ObservableCollection<PlayerPositionViewModel>();

        var sorted = players
            .OrderBy(p => p.IsActive ? 0 : 1)
            .ThenBy(p => p.Number)
            .ToList();

        int activeRow = 0;
        var usedSpots = new HashSet<(int col, int row)>();

        foreach (var player in sorted)
        {
            // Choose styles
            string courtStyleKey = player.IsTeamA
                ? "CourtAPlayerButtonStyle"
                : "CourtBPlayerButtonStyle";
            string benchStyleKey = player.IsTeamA
                ? "BenchAPlayerButtonStyle"
                : "BenchBPlayerButtonStyle";

            Style style = player.IsActive
                ? (Style)resources[courtStyleKey]
                : (Style)resources[benchStyleKey];

            // Court players
            if (player.IsActive && activeRow < 5)
            {
                int courtColumn = player.IsTeamA ? 2 : 0;
                int courtRow = activeRow;
                list.Add(new PlayerPositionViewModel(player, courtRow, courtColumn, style, onSelect, resources));
                activeRow++;
            }
            else
            {
                // Bench players: find an open bench spot
                int[] benchCols = player.IsTeamA ? new[] { 1, 0 } : new[] { 1, 2 };
                bool placed = false;

                foreach (int col in benchCols)
                {
                    for (int row = 0; row < 5; row++)
                    {
                        if (!usedSpots.Contains((col, row)))
                        {
                            usedSpots.Add((col, row));
                            list.Add(new PlayerPositionViewModel(player, row, col, style, onSelect, resources));
                            placed = true;
                            break;
                        }
                    }
                    if (placed) break;
                }
            }
        }

        return list;
    }
}
