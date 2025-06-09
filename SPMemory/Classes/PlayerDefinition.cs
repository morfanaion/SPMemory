using SPMemory.Enums;

namespace SPMemory.Classes
{
	public class PlayerDefinition : BaseNotifier
	{
		private string _name = string.Empty;

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				RaisePropertyChanged();
			}
		}

		private PlayerType _playerType;

		public PlayerType PlayerType
		{
			get { return _playerType; }
			set
			{
				_playerType = value;
				RaisePropertyChanged();
			}
		}

		private DifficultyLevel _difficultyLevel;

		public DifficultyLevel DifficultyLevel
		{
			get { return _difficultyLevel; }
			set
			{
				_difficultyLevel = value;
				RaisePropertyChanged();
			}
		}
	}
}
