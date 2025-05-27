using System.Collections;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SPMemory.Converters
{
	internal class CardStatusConverter : IMultiValueConverter
	{
		public IEnumerable Cards { get; set; } = new BitmapImage[0];
		public BitmapImage Back { get; set; } = new BitmapImage();

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if(values.Length != 2)
			{
				throw new ArgumentException("Need exactly 2 values");
			}
			if (values[0] is not int cardPairId)
			{
				throw new ArgumentException("First value should be the Id of the card pair (int)");
			}
			if (values[1] is not bool open)
			{
				throw new ArgumentException("Second value should be a boolean indicating whether the card has been flipped over or not");
			}
			if(!open)
			{
				return Back;
			}
			return Cards.OfType<BitmapImage>().Skip(cardPairId).First();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
