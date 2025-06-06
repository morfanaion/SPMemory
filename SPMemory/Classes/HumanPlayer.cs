using SPMemory.Messaging;

namespace SPMemory.Classes
{
    internal class HumanPlayer : BaseNotifier, IPlayer
    {
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

        public void StartTurn()
        {
            MessengerService.Instance.Subscribe<MemoryCardClickedMessage>(this, (sender, args) =>
            {
                MessengerService.Instance.SendMessage(this, new TurnCardOpenMessage() { Idx = args.Idx });
            });
        }

        public void EndTurn()
        {
            MessengerService.Instance.Unsubscribe<MemoryCardClickedMessage>(this);
        }


    }
}
