using SPMemory.Classes;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SPMemory.Converters
{
    internal class CurrentPlayerToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length == 2 && values[0] is IPlayer player && values[1] is IPlayer activePlayer && activePlayer.PlayerId == player.PlayerId)
            {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
