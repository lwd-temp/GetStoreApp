﻿using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Controls.Settings.Common
{
    public interface INotificationService
    {
        bool AppNotification { get; set; }

        Task InitializeNotificationAsync();

        Task SetNotificationAsync(bool appNotification);
    }
}