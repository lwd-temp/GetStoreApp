﻿using GetStoreApp.Models.Controls.Settings.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Contracts.Controls.Settings.Common
{
    public interface IDownloadOptionsService
    {
        StorageFolder DefaultFolder { get; }

        StorageFolder DownloadFolder { get; set; }

        int DownloadItem { get; set; }

        DownloadModeModel DownloadMode { get; set; }

        List<int> DownloadItemList { get; }

        List<DownloadModeModel> DownloadModeList { get; }

        Task InitializeAsync();

        Task OpenFolderAsync(StorageFolder folder);

        Task OpenItemFolderAsync(string filePath);

        Task CreateFolderAsync(string folderPath);

        Task SetFolderAsync(StorageFolder folder);

        Task SetItemAsync(int itemValue);

        Task SetModeAsync(DownloadModeModel downloadMode);
    }
}