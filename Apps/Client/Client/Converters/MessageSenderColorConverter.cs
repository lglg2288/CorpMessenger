using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Client.Converters
{
    public class MessageSenderColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isMyMessage)
            {
                return isMyMessage
                    ? new SolidColorBrush(Color.Parse("#4A9EFF"))
                    : new SolidColorBrush(Color.Parse("#FF6B6B"));
            }

            return new SolidColorBrush(Colors.Gray); // fallback
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
