﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class LanguageViewModel : ObservableRecipient
    {
        private string _selectedLanguage = LanguageService.PriLangCodeName;

        public string SelectedLanguage
        {
            get
            {
                return _selectedLanguage;
            }
            set
            {
                SetProperty(ref _selectedLanguage, value);
                LanguageService.SetLanguage(value);
            }
        }

        public ICommand LaunchSettingsInstalledAppsCommand { get; set; }

        public IReadOnlyList<LanguageModel> LanguageList { get; } = LanguageService.LanguageList;

        public LanguageViewModel()
        {
            LaunchSettingsInstalledAppsCommand = new RelayCommand(async () => { await LaunchSettingsInstalledAppsClickedAsync(); });
        }

        private static async Task LaunchSettingsInstalledAppsClickedAsync()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
        }
    }
}