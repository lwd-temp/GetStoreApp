﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Settings;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class RegionViewModel : ObservableRecipient
    {
        private IRegionService RegionService { get; } = IOCHelper.GetService<IRegionService>();

        public List<RegionModel> RegionList => RegionService.RegionList;

        private RegionModel _region;

        public RegionModel Region
        {
            get { return _region; }

            set { SetProperty(ref _region, value); }
        }

        // 应用在应用商店对应的区域选择
        public IAsyncRelayCommand RegionSelectCommand => new AsyncRelayCommand(async () =>
        {
            await RegionService.SetRegionAsync(Region);
        });

        public RegionViewModel()
        {
            Region = RegionService.AppRegion;
        }
    }
}
