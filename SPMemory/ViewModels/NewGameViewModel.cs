using SPMemory.Classes;
using SPMemory.Enums;
using System.Collections.ObjectModel;

namespace SPMemory.ViewModels
{
    public class NewGameViewModel : BaseNotifier
    {
        public NewGameViewModel()
        {
            Players = new ObservableCollection<PlayerDefinition>();
            NumberOfPlayers = 1;
		}

        private int _numberOfPlayers = 0;
        public int NumberOfPlayers
        {
            get => _numberOfPlayers; set
            {
                _numberOfPlayers = value;
                while (_numberOfPlayers > Players.Count)
                {
                    Players.Add(new PlayerDefinition() { Name = $"Player {Players.Count + 1}", PlayerType = PlayerType.Human, DifficultyLevel = DifficultyLevel.Easy });
                }
                while(_numberOfPlayers < Players.Count)
                {
                    Players.RemoveAt(Players.Count - 1);
                }
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PlayerDefinition> Players { get; }

    }
}
