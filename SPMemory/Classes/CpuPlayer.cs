using SPMemory.Messaging;
using System.Windows;
using System.Windows.Threading;

namespace SPMemory.Classes
{
	internal class CpuPlayer : BaseNotifier, IPlayer, IDisposable
	{
		public enum DifficultyLevel
		{
			Nitwit = 1,
			Easy = 3,
			Medium = 10,
			Hard = 20
		}

		private List<(int cardIdx, int pairIdx)> SeenCards { get; }
		private int MemoryBufferSize { get; }
		public int PlayerId { get; set; }
		public string Name { get; set; } = string.Empty;
		private int _score = 0;
		public int Score
		{
			get => _score; set
			{
				_score = value;
				RaisePropertyChanged();
			}
		}
		private int CardOpened = -1;
		private List<int> CardIdxsStillInPlay { get; }
		private bool _myTurn = false;
		SemaphoreSlim turnLocker = new SemaphoreSlim(1);

		private CpuPlayer(int memoryBufferSize, int boardSize)
		{
			SeenCards = new List<(int cardIdx, int pairIdx)>();
			MemoryBufferSize = memoryBufferSize;
			CardIdxsStillInPlay = Enumerable.Range(0, boardSize).ToList();
			MessengerService.Instance.Subscribe<CardOpenedMessage>(this, CardOpenedMessageHandler);
			MessengerService.Instance.Subscribe<PairFoundMessage>(this, PairFoundMessageHandler);
		}

		private void PairFoundMessageHandler(object arg1, PairFoundMessage message)
		{
			for (int i = 0; i < SeenCards.Count; i++)
			{
				if (SeenCards[i].cardIdx == message.CardIdx1 || SeenCards[i].cardIdx == message.CardIdx2)
				{
					SeenCards.RemoveAt(i);
					i--;
				}
			}
			CardIdxsStillInPlay.Remove(message.CardIdx1);
			CardIdxsStillInPlay.Remove(message.CardIdx2);
		}

		private void CardOpenedMessageHandler(object sender, CardOpenedMessage message)
		{
			for (int i = 0 ; i < SeenCards.Count; i++)
			{
				if (SeenCards[i].cardIdx == message.CardIdx)
				{
					SeenCards.RemoveAt(i);
					break;
				}
			}
			SeenCards.Add((message.CardIdx, message.PairIdx));
			if (SeenCards.Count > MemoryBufferSize)
			{
				SeenCards.RemoveAt(0);
			}
			if(_myTurn)
			{
				_myTurn = false;
				Task.Run(SelectCard);
			}
		}

		public static CpuPlayer CreatePlayerForDifficulty(DifficultyLevel difficultyLevel, int boardSize)
		{
			return new CpuPlayer((int)difficultyLevel, boardSize);
		}

		public void EndTurn()
		{
			_myTurn = false;
		}

		public void StartTurn()
		{
			_myTurn = true;
			CardOpened = -1;
			Task.Run(SelectCard);
		}

		private async Task SelectCard()
		{
			await Task.Delay(500);
			int idxToSelect = -1;
			if(CardOpened != -1)
			{
				IGrouping<int, (int cardIdx, int pairIdx)>? occurencesOfOpenedCardInHistory = SeenCards.GroupBy(cardHistory => cardHistory.pairIdx).Where(g => g.Key == SeenCards.Last().pairIdx).FirstOrDefault();
				if(occurencesOfOpenedCardInHistory?.Count() > 1)
				{
					idxToSelect = occurencesOfOpenedCardInHistory.FirstOrDefault(cardHistory => cardHistory.cardIdx != CardOpened).cardIdx;
					await Application.Current.Dispatcher.BeginInvoke(() => MessengerService.Instance.SendMessage(this, new TurnCardOpenMessage() { Idx =  idxToSelect }));
					return;
				}
			}
			else
			{
				IGrouping<int, (int cardIdx, int pairIdx)>? seenPair = SeenCards.GroupBy(cardHistory => cardHistory.pairIdx).FirstOrDefault(g => g.Count() == 2);
				if (seenPair != null)
				{
					idxToSelect = seenPair.First().cardIdx;
					CardOpened = idxToSelect;
					await Application.Current.Dispatcher.BeginInvoke(() => MessengerService.Instance.SendMessage(this, new TurnCardOpenMessage() { Idx = idxToSelect}));
					return;
				}
			}
			Random rand = new Random();
			Func<int, bool> predicate = i => !SeenCards.Any(c => c.cardIdx == i);
			if(CardOpened != -1)
			{
				predicate = i => i != CardOpened && !SeenCards.Any(c => c.cardIdx == i);
			}
			IEnumerable<int> randomSelectableList = CardIdxsStillInPlay.Where(predicate);
			idxToSelect = randomSelectableList.Skip(rand.Next(randomSelectableList.Count() - 1)).First();
			CardOpened = idxToSelect;
			await Application.Current.Dispatcher.BeginInvoke(() => MessengerService.Instance.SendMessage(this, new TurnCardOpenMessage() { Idx = idxToSelect }));
		}

		public void Dispose()
		{
			MessengerService.Instance.Unsubscribe<CardOpenedMessage>(this);
			MessengerService.Instance.Unsubscribe<PairFoundMessage>(this);
		}
	}
}
