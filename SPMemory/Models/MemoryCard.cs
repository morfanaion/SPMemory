namespace SPMemory.Models
{
	public class MemoryCard
	{
		private bool _open;

		public bool Open
		{
			get { return _open; }
			set { _open = value; }
		}

		private int _cardPairId;

		public int CardPairId
		{
			get { return _cardPairId; }
			set { _cardPairId = value; }
		}
	}
}
