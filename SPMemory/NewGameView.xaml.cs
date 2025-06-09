using SPMemory.ViewModels;
using System.Windows;

namespace SPMemory
{
	/// <summary>
	/// Interaction logic for NewGameView.xaml
	/// </summary>
	public partial class NewGameView : Window
    {
        public NewGameViewModel? ViewModel
        {
            get => DataContext as NewGameViewModel;
            set => DataContext = value;
        }

        public NewGameView()
        {
            InitializeComponent();
        }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
            DialogResult = true;
            Close();
		}
	}
}
