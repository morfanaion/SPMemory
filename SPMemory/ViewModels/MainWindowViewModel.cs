using SPMemory.Models;

namespace SPMemory.ViewModels
{
	public class MainWindowViewModel
	{
		public List<MemoryCard> Test { get; set; } = Enumerable.Repeat(new MemoryCard(), 35).ToList();

		public int NumCardsHorizontal { get; set; } = 5;
	}
}
