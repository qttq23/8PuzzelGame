using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace _8PuzzelGame
{
    public class IntValueToImageNameConverter : IValueConverter
    {
        public static readonly IntValueToImageNameConverter Instance = new IntValueToImageNameConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            CroppedBitmap result;

            if (index == Game.EmptyValue)
                return null;

            result = MainWindow.ImageTiles[index];
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
