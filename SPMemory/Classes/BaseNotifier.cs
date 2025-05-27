using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SPMemory.Classes
{
	public class BaseNotifier : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
