using SPMemory.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SPMemory.Controls
{
	/// <summary>
	/// Interaction logic for MemoryBoard.xaml
	/// </summary>
	public partial class MemoryBoard : UserControl, INotifyPropertyChanged
	{
		public class CardRecord(int idx, MemoryCard card)
		{
			public int Idx { get; set; } = idx;
			public MemoryCard Card { get; set; } = card;
		}

		public MemoryBoard()
		{
			InitializeComponent();
			//DataContext = this;
		}

		public static DependencyProperty MemoryCardsProperty = DependencyProperty.Register(nameof(MemoryCards), typeof(List<MemoryCard>), typeof(MemoryBoard), new PropertyMetadata() { PropertyChangedCallback = MemoryCardsChangedCallback } );
		public static DependencyProperty NumCardsHorizontalProperty = DependencyProperty.Register(nameof(NumCardsHorizontal), typeof(int), typeof(MemoryBoard), new PropertyMetadata() { PropertyChangedCallback = NumCardsHorizontalChangedCallback });

		public event PropertyChangedEventHandler? PropertyChanged;

		private static void MemoryCardsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((MemoryBoard)d).RaisePropertyChanged(nameof(PlayingGrid));
		}
		
		private static void NumCardsHorizontalChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((MemoryBoard)d).RaisePropertyChanged(nameof(PlayingGrid));
		}

		public List<MemoryCard> MemoryCards
		{
			get => (List<MemoryCard>)GetValue(MemoryCardsProperty);
			set => SetValue(MemoryCardsProperty, value);
		}

		public int NumCardsHorizontal
		{
			get => (int)GetValue(NumCardsHorizontalProperty);
			set => SetValue(NumCardsHorizontalProperty, value);
		}

		public IEnumerable<IEnumerable<CardRecord>> PlayingGrid
		{
			get
			{
				if(MemoryCards == null)
				{
					yield break;
				}
				for(int i = 0; i < MemoryCards.Count; i+= NumCardsHorizontal)
				{
					yield return MemoryCards.Skip(i).Take(NumCardsHorizontal).Zip(Enumerable.Range(i, NumCardsHorizontal)).Select(pair => new CardRecord(pair.Second, pair.First));
				}
			}
		}

		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			MemoryCard card = ((CardRecord)((Button)sender).DataContext).Card;
			card.Open = !card.Open;
		}
	}
}
