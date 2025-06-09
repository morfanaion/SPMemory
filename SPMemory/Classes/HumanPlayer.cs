using SPMemory.Messaging;

namespace SPMemory.Classes
{
	internal class HumanPlayer : BaseNotifier, IPlayer, IDisposable
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = string.Empty;
        private int _score = 0;
        private bool _myTurn = false;
        public int Score
        {
            get => _score; set
            {
                _score = value;
                RaisePropertyChanged();
            }
        }

        public void StartTurn()
        {
            _myTurn = true;
            MessengerService.Instance.Subscribe<MemoryCardClickedMessage>(this, (sender, args) =>
            {
                MessengerService.Instance.SendMessage(this, new TurnCardOpenMessage() { Idx = args.Idx });
            });
        }

        public void EndTurn()
        {
            _myTurn = false;
            MessengerService.Instance.Unsubscribe<MemoryCardClickedMessage>(this);
        }

		public void Dispose()
		{
			if(_myTurn)
            {
                EndTurn();
            }
		}
	}
}
