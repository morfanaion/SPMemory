using System.Windows.Input;

namespace SPMemory.Classes
{
	public class RelayCommand<T>(Action<T> execute, Func<T, bool> canExecute) : ICommand
	{
		public RelayCommand(Action<T> execute) : this(execute, _ => true)
		{
		}

		public event EventHandler? CanExecuteChanged;

		public Func<T, bool> _canExecute = canExecute;
		public Action<T> _execute = execute;

		public void InvalidateCanExecute()
		{
			CanExecuteChanged?.Invoke(this, new EventArgs());
		}

		public bool CanExecute(object? parameter)
		{
			if(parameter is not T typedParam)
			{
				throw new ArgumentException("parameter is not of the correct type");
			}
			return _canExecute(typedParam);
		}

		public void Execute(object? parameter)
		{
			if (parameter is not T typedParam)
			{
				throw new ArgumentException("parameter is not of the correct type");
			}
			_execute(typedParam);
		}
	}

	public class RelayCommand(Action execute, Func<bool> canExecute) : ICommand
	{
		public RelayCommand(Action execute) : this(execute, () => true)
		{
		}

		public event EventHandler? CanExecuteChanged;

		public Func<bool> _canExecute = canExecute;
		public Action _execute = execute;

		public void InvalidateCanExecute()
		{
			CanExecuteChanged?.Invoke(this, new EventArgs());
		}

		public bool CanExecute(object? parameter)
		{
			return _canExecute();
		}

		public void Execute(object? parameter)
		{
			_execute();
		}
	}
}
