using SPMemory.Models;
using System.Windows;
using System.Windows.Controls;

namespace SPMemory.Controls
{
	/// <summary>
	/// Interaction logic for MemoryBoard.xaml
	/// </summary>
	public partial class MemoryBoard : UserControl
	{
		public MemoryBoard()
		{
			InitializeComponent();
		}

		public static DependencyProperty MemoryCardsProperty = DependencyProperty.Register(nameof(MemoryCard), typeof(List<MemoryCard>), typeof(MemoryBoard), new PropertyMetadata() { PropertyChangedCallback = MemoryCardsChangedCallback } );

		public List<MemoryCard> MemoryCards
		{
			get => (List<MemoryCard>)GetValue(MemoryCardsProperty);
			set => SetValue(MemoryCardsProperty, value);
		}

		public static void MemoryCardsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{

		}
	}
}
