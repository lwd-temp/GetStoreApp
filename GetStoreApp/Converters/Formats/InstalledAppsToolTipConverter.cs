﻿using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Formats
{
    public class InstalledAppsToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is null)
            {
                return string.Empty;
            }

            string param = System.Convert.ToString(parameter);

            if (param is "AppName")
            {
                return string.Format(ResourceService.GetLocalized("WinGet/AppNameToolTip"), value);
            }
            else if (param is "AppVersion")
            {
                return string.Format(ResourceService.GetLocalized("WinGet/AppVersionToolTip"), value);
            }
            else if (param is "AppPublisher")
            {
                return string.Format(ResourceService.GetLocalized("WinGet/AppPublisherToolTip"), value);
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return default;
        }
    }
}
