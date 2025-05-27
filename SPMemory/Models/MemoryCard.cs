using SPMemory.Classes;

namespace SPMemory.Models
{
	public class MemoryCard : BaseNotifier
	{
		private bool _open;

		public bool Open
		{
			get { return _open; }
			set { _open = value; RaisePropertyChanged(); }
		}

		private int _cardPairId;

		public int CardPairId
		{
			get { return _cardPairId; }
			set { _cardPairId = value; RaisePropertyChanged(); }
		}
	}
}
