﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Caliburn.Micro;

namespace KrachConnect
{
  public abstract class BaseConverter : MarkupExtension
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }

  [ValueConversion(typeof (object), typeof (int))]
  public class BoolToStrokeThicknessConverter : BaseConverter, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      var booleanValue = (bool) value;
      return booleanValue ? 3 : 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof (object), typeof (Brush))]
  public class JustMeasuredToBrushConverter : BaseConverter, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      var booleanValue = (bool) value;
      return booleanValue ? Brushes.Chartreuse : Brushes.Green;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof (object), typeof (Brush))]
  public class IsArchivedToBrushConverter : BaseConverter, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      var booleanValue = (bool) value;
      return booleanValue ? Brushes.SlateGray : Brushes.Green;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof (object), typeof (Brush))]
  public class MEPConverter : BaseConverter, IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter,
      CultureInfo culture)
    {
      var justMeasured = (bool) values[1];
      var isArchived = (bool) values[0];
      if (isArchived)
      {
        return Brushes.SlateGray;
      }

      if (justMeasured)
      {
        return Brushes.Chartreuse;
      }
      return Brushes.Green;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  [ValueConversion(typeof (object), typeof (Cursor))]
  public class IsArchivedToCursorConverter : BaseConverter, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      var isArchived = (bool) value;
      return isArchived ? Cursors.No : Cursors.Hand;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof (object), typeof (Visibility))]
  public class BoolToVisibilityConverter : BaseConverter, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      var booleanValue = (bool) value;
      return booleanValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(object), typeof(FontWeight))]
  public class StringToBrushConverter : BaseConverter, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      var selectedItem = ((Screen)value).DisplayName;
      var compareItem = (string) parameter;
      return selectedItem == compareItem ? FontWeights.Bold : FontWeights.Normal;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(object), typeof(Visibility))]
  public class AverageToVisibilityConverter : BaseConverter, IValueConverter
  {
      public object Convert(object value, Type targetType, object parameter,
        CultureInfo culture)
      {
          var floatValue = (float)value;
          if (floatValue>80)
          {
              return Visibility.Visible;
          }
          return Visibility.Hidden;
      }

      public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture)
      {
          return null;
      }
  }


  [ValueConversion(typeof(object), typeof(Brush))]
  public class IntToBrushConverter : BaseConverter, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      var floatValue = (float)value;
      if (floatValue < 40) return Brushes.Green;
      if (floatValue < 80) return Brushes.Orange;
      return Brushes.Red;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
      CultureInfo culture)
    {
      return null;
    }
  }

}