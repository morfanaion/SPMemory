using SPMemory.Classes;
using SPMemory.Messaging;
using SPMemory.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SPMemory.ViewModels
{
	public class MainWindowViewModel : BaseNotifier
	{
		private bool _waitingForReset = false;
		public MainWindowViewModel() 
		{
			ClickCardCommand = new RelayCommand<int>(CardClicked);
			NewGameCommand = new RelayCommand(NewGameCommandExecute);
			MessengerService.Instance.Subscribe<TurnCardOpenMessage>(this, (obj, args) =>
			{
				if(!Equals(obj, CurrentPlayer))
				{
					MessageBox.Show("It's not your turn");
					return;
				}
				MemoryCardsList[args.Idx].Open = true;
				_openedCards.Add(MemoryCardsList[args.Idx]);
				if (_openedCards.Count == 2)
				{
                    CurrentPlayer.EndTurn();
					if (_openedCards[0].CardPairId == _openedCards[1].CardPairId)
					{
                        // add to score, start turn
                        CurrentPlayer.Score++;
                        CurrentPlayer.StartTurn();
						_openedCards.Clear();
					}
					else
					{
						// wait for close before starting the turn for the next player
						_waitingForReset = true;
					}
				}
			});
		}

		public ObservableCollection<IPlayer> Players { get; set; } = new ObservableCollection<IPlayer>();

        private void NewGameCommandExecute()
        {
			Players.Clear();
			Players.Add(new HumanPlayer() { PlayerId = 1, Name = "André" });
            Players.Add(new HumanPlayer() { PlayerId = 2, Name = "Pietertje" });
            MemoryCardsList = Enumerable.Range(0, 20).Concat(Enumerable.Range(0, 20)).Select(n => new { OrderNr = new Random().Next(), CardPairId = n }).OrderBy(p => p.OrderNr).Select(p => new MemoryCard() { CardPairId = p.CardPairId, Open = false }).ToList();
            CurrentPlayer.StartTurn();
        }

        public List<MemoryCard> _openedCards = new List<MemoryCard>();
		private int _activePlayerIdx = 0;
		public int ActivePlayerIdx
		{
			get => _activePlayerIdx;
			set
			{
				_activePlayerIdx= value;
				RaisePropertyChanged();
				RaisePropertyChanged(nameof(CurrentPlayer));
			}
		}

		public IPlayer CurrentPlayer
		{
			get
			{
				if(ActivePlayerIdx >= Players.Count)
				{
					return new HumanPlayer() { Name = "Jij faalhaas" , PlayerId = -1};
				}
				return Players[ActivePlayerIdx];
			}
		}

		private List<MemoryCard> _memoryCardsList = new List<MemoryCard>();
		public List<MemoryCard> MemoryCardsList { get => _memoryCardsList; set
			{
				_memoryCardsList = value;
				RaisePropertyChanged();
			}
		}

		public int NumCardsHorizontal { get; set; } = 8;

		public ICommand ClickCardCommand { get; set; }
		public ICommand NewGameCommand { get; set; }


        private void CardClicked(int cardIdx)
		{
			if(_waitingForReset)
			{
                foreach (MemoryCard card in _openedCards)
				{
					card.Open = false;
				}
				_openedCards.Clear();
				ActivePlayerIdx = (ActivePlayerIdx + 1) % Players.Count;
                CurrentPlayer.StartTurn();
				_waitingForReset = false;
			}
			else
			{
				if (!MemoryCardsList[cardIdx].Open)
				{
					MessengerService.Instance.SendMessage(this, new MemoryCardClickedMessage() { Idx = cardIdx });
				}
			}
		}
	}
}
