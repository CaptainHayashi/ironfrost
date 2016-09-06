using System;
using System.Windows.Data;
using FontAwesome.WPF;
using System.Windows;
using System.Globalization;

namespace Ironfrost
{
    /// <summary>
    ///   Value converter changing player states into FontAwesome icons.
    /// </summary>
    public class PlayerStateToFAConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (typeof(FontAwesomeIcon) == targetType &&
                value is PlayerClientRole.PlayerState)
            {
                switch ((PlayerClientRole.PlayerState)value)
                {
                    case PlayerClientRole.PlayerState.Ejected:
                        return FontAwesomeIcon.Eject;
                    case PlayerClientRole.PlayerState.Playing:
                        return FontAwesomeIcon.Play;
                    case PlayerClientRole.PlayerState.Stopped:
                        return FontAwesomeIcon.Stop;
                    case PlayerClientRole.PlayerState.Unknown:
                        return FontAwesomeIcon.Question;
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (typeof(PlayerClientRole.PlayerState) == targetType &&
                value is FontAwesomeIcon)
            {
                switch ((FontAwesomeIcon)value)
                {
                    case FontAwesomeIcon.Eject:
                        return PlayerClientRole.PlayerState.Ejected;
                    case FontAwesomeIcon.Play:
                        return PlayerClientRole.PlayerState.Playing;
                    case FontAwesomeIcon.Stop:
                        return PlayerClientRole.PlayerState.Stopped;
                    case FontAwesomeIcon.Question:
                        return PlayerClientRole.PlayerState.Unknown;
                }
            }
            return DependencyProperty.UnsetValue;
        }
    }

}
