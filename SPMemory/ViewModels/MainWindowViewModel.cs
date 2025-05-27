using SPMemory.Models;

namespace SPMemory.ViewModels
{
	public class MainWindowViewModel
	{
		public List<MemoryCard> Test { get; set; } = Enumerable.Range(0, 20).Concat(Enumerable.Range(0, 20)).Select(n => new { OrderNr = new Random().Next(), CardPairId = n }).OrderBy(p => p.OrderNr).Select(p => new MemoryCard() { CardPairId = p.CardPairId, Open = false}).ToList();

		public int NumCardsHorizontal { get; set; } = 8;
	}
}
