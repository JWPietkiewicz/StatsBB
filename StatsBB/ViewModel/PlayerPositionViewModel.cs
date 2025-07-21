using StatsBB.Model;
using StatsBB.MVVM;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using StatsBB.Domain;

namespace StatsBB.ViewModel
{
    public class PlayerPositionViewModel : ViewModelBase
    {

        public Player Player { get; }
        public int Row { get; }
        public int Column { get; }


        public string DisplayName => Player.DisplayName;
        public bool IsTeamA => Player.IsTeamA;

        public ICommand SelectPlayerCommand { get; }

        public bool IsSelectable { get; private set; }

        private bool _isSelectableForAssist;
        public bool IsSelectableForAssist
        {
            get => _isSelectableForAssist;
            set
            {
                _isSelectableForAssist = value;
                OnPropertyChanged();
                // Update style or enable/disable as needed
            }
        }
        public void SetAssistSelectionMode(bool isSelectable)
        {
            // This should affect button style/command state
            IsSelectableForAssist = isSelectable;
            OnPropertyChanged(nameof(IsSelectableForAssist));
        }


        private bool _isSelectableForRebound;
        public bool IsSelectableForRebound
        {
            get => _isSelectableForRebound;
            set
            {
                _isSelectableForRebound = value;
                OnPropertyChanged();
            }
        }

        public void SetReboundSelectionMode(bool isSelectable)
        {
            IsSelectableForRebound = isSelectable;
        }

        private bool _isSelectableForBlock;
        public bool IsSelectableForBlock
        {
            get => _isSelectableForBlock;
            set
            {
                _isSelectableForBlock = value;
                OnPropertyChanged();
            }
        }

        public void SetBlockerSelectionMode(bool isSelectable)
        {
            IsSelectableForBlock = isSelectable;
        }

        private bool _isSelectableForTurnover;
        public bool IsSelectableForTurnover
        {
            get => _isSelectableForTurnover;
            set
            {
                _isSelectableForTurnover = value;
                OnPropertyChanged();
            }
        }

        public void SetTurnoverSelectionMode(bool isSelectable)
        {
            IsSelectableForTurnover = isSelectable;
        }

        private bool _isSelectableForSteal;
        public bool IsSelectableForSteal
        {
            get => _isSelectableForSteal;
            set
            {
                _isSelectableForSteal = value;
                OnPropertyChanged();
            }
        }

        public void SetStealSelectionMode(bool isSelectable)
        {
            IsSelectableForSteal = isSelectable;
        }

        private bool _isSelectableForFoulCommiter;
        public bool IsSelectableForFoulCommiter
        {
            get => _isSelectableForFoulCommiter;
            set
            {
                _isSelectableForFoulCommiter = value;
                OnPropertyChanged();
            }
        }

        public void SetFoulCommiterSelectionMode(bool isSelectable)
        {
            IsSelectableForFoulCommiter = isSelectable;
        }

        private bool _isSelectableForFouled;
        public bool IsSelectableForFouled
        {
            get => _isSelectableForFouled;
            set
            {
                _isSelectableForFouled = value;
                OnPropertyChanged();
            }
        }

        public void SetFouledPlayerSelectionMode(bool isSelectable)
        {
            IsSelectableForFouled = isSelectable;
        }

        private bool _isSelectedAsFreeThrowShooter;
        public bool IsSelectedAsFreeThrowShooter
        {
            get => _isSelectedAsFreeThrowShooter;
            set
            {
                _isSelectedAsFreeThrowShooter = value;
                OnPropertyChanged();
            }
        }

        private bool _isSelectedAsAssist;
        public bool IsSelectedAsAssist
        {
            get => _isSelectedAsAssist;
            set
            {
                _isSelectedAsAssist = value;
                OnPropertyChanged();
            }
        }

        public PlayerPositionViewModel(Player player, ICommand selectCommand)
        {
            Player = player;
            SelectPlayerCommand = new RelayCommand(_ => selectCommand.Execute(player));
        }


        public PlayerPositionViewModel(Player player, int row, int column, Action<Player> onSelect)
        {
            Player = player;
            Row = row;
            Column = column;

            SelectPlayerCommand = new RelayCommand(
                param => onSelect?.Invoke(Player),
                param => true
            );
        }

        public void UpdateButtonStyle(bool isPointSelected, bool isActionSelected)
        {
            IsSelectable = isPointSelected && isActionSelected;
            OnPropertyChanged(nameof(IsSelectable));
        }
    }
}
