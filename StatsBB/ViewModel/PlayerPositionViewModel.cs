using StatsBB.Model;
using StatsBB.MVVM;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StatsBB.ViewModel
{
    public class PlayerPositionViewModel : ViewModelBase
    {
        private readonly ResourceDictionary _resources;

        public Player Player { get; }
        public int Row { get; }
        public int Column { get; }

        private Style _buttonStyle;
        public Style ButtonStyle
        {
            get => _buttonStyle;
            set
            {
                _buttonStyle = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName => Player.Number.ToString();

        public ICommand SelectPlayerCommand { get; }

        public bool IsSelectable { get; private set; }

        public Brush TeamColor => Player.IsTeamA
    ? (Brush)_resources["CourtAColor"]
    : (Brush)_resources["CourtBColor"];

        public Brush TeamColorDimmed => new SolidColorBrush(GetDimmedColor());

        private Color GetDimmedColor()
        {
            var baseColor = ((SolidColorBrush)TeamColor).Color;
            byte Dim(byte c) => (byte)(c * 0.8); // adjust brightness
            return Color.FromRgb(Dim(baseColor.R), Dim(baseColor.G), Dim(baseColor.B));
        }

        public PlayerPositionViewModel(Player player, int row, int column, Style initialStyle, Action<Player> onSelect, ResourceDictionary resources)
        {
            Player = player;
            Row = row;
            Column = column;
            _resources = resources;

            _buttonStyle = initialStyle;

            SelectPlayerCommand = new RelayCommand(
                param => onSelect?.Invoke(Player),
                param => true
            );
        }

        public void UpdateButtonStyle(bool isPointSelected, bool isActionSelected, Brush teamColor)
        {
            bool isOnCourt = Player.IsActive;
            IsSelectable = isPointSelected && isActionSelected;

            Style finalStyle;

            if (isOnCourt)
            {
                if (IsSelectable)
                {
                    finalStyle = (Style)_resources["CourtPlayerSelectableStyle"];
                }
                else
                {
                    // Clone CourtPlayerDisabledStyle and apply dimmed background
                    var baseStyle = (Style)_resources["CourtPlayerDisabledStyle"];
                    var dimmedStyle = new Style(typeof(Button), baseStyle);
                    dimmedStyle.Setters.Add(new Setter(Control.BackgroundProperty, TeamColorDimmed));
                    finalStyle = dimmedStyle;
                }
            }
            else // bench
            {
                if (IsSelectable)
                {
                    finalStyle = (Style)_resources["BenchPlayerSelectableStyle"];
                }
                else
                {
                    finalStyle = (Style)_resources["BenchPlayerDisabledStyle"];
                }
            }

            ButtonStyle = finalStyle;

            OnPropertyChanged(nameof(IsSelectable));
            OnPropertyChanged(nameof(ButtonStyle));
        }
    }
}
