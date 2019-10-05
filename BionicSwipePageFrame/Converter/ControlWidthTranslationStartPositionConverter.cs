using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace BionicCode.BionicSwipePageFrame.Converter
{
  [ValueConversion(typeof(Control), typeof(double))]
  public class ControlWidthTranslationStartPositionConverter : IValueConverter
  {
    #region Implementation of IValueConverter

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is BionicSwipePageFrame pageFrame)
      {
        return pageFrame.NavigationDirection == PageNavigationDirection.Next
          ? pageFrame.ActualWidth
          : pageFrame.NavigationDirection == PageNavigationDirection.Previous
            ? pageFrame.ActualWidth * -1
            : Binding.DoNothing;
      }

      return Binding.DoNothing;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();

    #endregion
  }
}
