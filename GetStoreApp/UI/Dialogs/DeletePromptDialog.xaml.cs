﻿using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Extensions.Enum;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class DeletePromptDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public string DeleteContent { get; set; }

        public DeletePromptDialog(DeletePrompt deletePrompt)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;

            switch (deletePrompt)
            {
                case DeletePrompt.History: DeleteContent = ResourceService.GetLocalized(string.Format("/Dialog/Delete{0}", deletePrompt.ToString())); break;
                case DeletePrompt.Download: DeleteContent = ResourceService.GetLocalized(string.Format("/Dialog/Delete{0}", deletePrompt.ToString())); break;
                case DeletePrompt.DownloadWithFile: DeleteContent = ResourceService.GetLocalized(string.Format("/Dialog/Delete{0}", deletePrompt.ToString())); break;
            }

            InitializeComponent();
        }
    }
}
