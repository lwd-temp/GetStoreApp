﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Checks
{
    /// <summary>
    /// 继续下载任务按钮显示值转换器
    /// </summary>
    public class ContinueDownloadCheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            int result = System.Convert.ToInt32(value);

            return result == 2 ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
