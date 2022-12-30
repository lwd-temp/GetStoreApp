﻿using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.Window;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.Common
{
    public sealed partial class ClosingWindowDialog : ContentDialog
    {
        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public ClosingWindowDialog()
        {
            XamlRoot = MainWindow.GetMainWindowXamlRoot();
            InitializeComponent();
        }
    }
}