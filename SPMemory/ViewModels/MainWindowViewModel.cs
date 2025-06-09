using SPMemory.Classes;
using SPMemory.Enums;
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
				MessengerService.Instance.SendMessage(this, new CardOpenedMessage() { CardIdx = args.Idx, PairIdx = MemoryCardsList[args.Idx].CardPairId });
				if (_openedCards.Count == 2)
				{
                    CurrentPlayer.EndTurn();
					if (_openedCards[0].CardPairId == _openedCards[1].CardPairId)
					{
						PairFoundMessage message = new PairFoundMessage() { CardIdx1 = -1};
						for (int i = 0; i < MemoryCardsList.Count; i++)
						{
							if (MemoryCardsList[i].CardPairId == _openedCards[0].CardPairId)
							{
								if(message.CardIdx1 == -1)
								{
									message.CardIdx1 = i;
								}
								else
								{
									message.CardIdx2 = i;
									break;
								}
							}
						}
						MessengerService.Instance.SendMessage(this, message);
                        // add to score, start turn
                        CurrentPlayer.Score++;
						if(MemoryCardsList.Any(c => !c.Open))
						{
							CurrentPlayer.StartTurn();
						}
						else
						{
							string endOfGameMessage = $"Game is over!" + Environment.NewLine +
							Environment.NewLine +
							string.Join(Environment.NewLine, Players.Select(p => $"{p.Name}: {p.Score}")) + Environment.NewLine +
							Environment.NewLine;

							int maxScore = Players.Max(p => p.Score);
							IEnumerable<IPlayer> winners = Players.Where(p => p.Score == maxScore);
							if(winners.Skip(1).Any())
							{
								string winnersString = string.Join(", ", winners.Select(p => p.Name));
								winnersString = winnersString.Substring(0, winnersString.LastIndexOf(',')) + " and" + winnersString.Substring(winnersString.LastIndexOf(',') + 1);
								endOfGameMessage += $"Players {winnersString} are tied for first place!";
							}
							else
							{
								endOfGameMessage += $"{winners.First().Name} is the winner!";
							}
							MessageBox.Show(endOfGameMessage);
						}
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
			NewGameView newGameView = new NewGameView()
			{
				ViewModel = new NewGameViewModel()
			};
			if(newGameView.ShowDialog() ?? false)
			{
				foreach(IDisposable disposable in Players)
				{
					disposable.Dispose();
				}

				Players.Clear();
				int nextPlayerId = 1;
				foreach (PlayerDefinition player in newGameView.ViewModel.Players)
				{
					IPlayer newPlayer;
					switch (player.PlayerType)
					{
						case PlayerType.Human:
							newPlayer = new HumanPlayer() { Name = player.Name, PlayerId = nextPlayerId++, Score = 0 };
							Players.Add(newPlayer);
							break;
						case PlayerType.Cpu:
							newPlayer = CpuPlayer.CreatePlayerForDifficulty(player.DifficultyLevel, 40);
							newPlayer.Name = player.Name;
							newPlayer.PlayerId = nextPlayerId++;
							Players.Add(newPlayer);
							break;
					}
				}
				MemoryCardsList = Enumerable.Range(0, 20).Concat(Enumerable.Range(0, 20)).Select(n => new { OrderNr = new Random().Next(), CardPairId = n }).OrderBy(p => p.OrderNr).Select(p => new MemoryCard() { CardPairId = p.CardPairId, Open = false }).ToList();
				ActivePlayerIdx = 0;
				CurrentPlayer.StartTurn();
			}
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
