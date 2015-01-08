﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace KrachConnect
{
    public abstract class BaseConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    [ValueConversion(typeof(object), typeof(int))]
    public class BoolToStrokeThicknessConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
          System.Globalization.CultureInfo culture)
        {
            var booleanValue = (bool)value;
            return booleanValue ? 3 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                        System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(object), typeof(Brush))]
    public class JustMeasuredToBrushConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
          System.Globalization.CultureInfo culture)
        {
            var booleanValue = (bool)value;
            return booleanValue ? Brushes.Chartreuse : Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                        System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(object), typeof(Brush))]
    public class IsArchivedToBrushConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
          System.Globalization.CultureInfo culture)
        {
            var booleanValue = (bool)value;
            return booleanValue ? Brushes.SlateGray : Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                        System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(object), typeof(Brush))]
    public class MEPConverter : BaseConverter, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
          System.Globalization.CultureInfo culture)
        {
            var justMeasured = (bool)values[1];
            var isArchived = (bool)values[0];
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




}
