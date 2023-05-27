﻿using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Window;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppNotifications;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用通知服务
    /// </summary>
    public static class AppNotificationService
    {
        private static AppNotificationManager AppNotificationManager { get; } = AppNotificationManager.Default;

        /// <summary>
        /// 初始化应用通知
        /// </summary>
        public static void Initialize()
        {
            AppNotificationManager.NotificationInvoked += OnNotificationInvoked;

            AppNotificationManager.Register();
        }

        /// <summary>
        /// 处理应用通知后的响应事件
        /// </summary>
        public static async void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            await HandleAppNotificationAsync(args, true);
        }

        /// <summary>
        /// 处理应用通知
        /// </summary>
        public static async Task HandleAppNotificationAsync(AppNotificationActivatedEventArgs args, bool isFirstExecute)
        {
            string AppNotificationArguments = new WwwFormUrlDecoder(args.Argument).GetFirstValueByName("action");

            if (AppNotificationArguments is "CheckNetWorkConnection" && isFirstExecute)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
                if (Program.ApplicationRoot is null)
                {
                    Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
                }
            }
            else if(AppNotificationArguments is "OpenDownloadFolder")
            {
                string wingetTempPath = Path.Combine(Path.GetTempPath(), "WinGet");
                if (Directory.Exists(wingetTempPath))
                {
                    await Windows.System.Launcher.LaunchFolderPathAsync(wingetTempPath);
                }
                else
                {
                    await Windows.System.Launcher.LaunchFolderPathAsync(Path.GetTempPath());
                }
                if (Program.ApplicationRoot is null)
                {
                    Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
                }
            }
            else if(AppNotificationArguments is "OpenSettings")
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
                if (Program.ApplicationRoot is null)
                {
                    Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
                }
            }
            else
            {
                if (Program.IsAppLaunched)
                {
                    HandleMainThreadNotification(AppNotificationArguments);
                }
            }
        }

        /// <summary>
        /// 处理必须主线程才能处理的应用通知
        /// </summary>
        public static void HandleMainThreadNotification(string args)
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
            {
                // 先设置应用窗口的显示方式
                WindowHelper.ShowAppWindow();

                switch (args)
                {
                    // 正常打开应用
                    case "OpenApp":
                        {
                            break;
                        }
                    case "ViewDownloadPage":
                        {
                            if (NavigationService.GetCurrentPageType() != typeof(DownloadPage))
                            {
                                NavigationService.NavigateTo(typeof(DownloadPage));
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            });
        }

        /// <summary>
        /// 显示通知
        /// </summary>
        public static void Show(NotificationArgs notificationKey, params string[] notificationContent)
        {
            if (!NotificationService.AppNotification)
            {
                return;
            }

            switch (notificationKey)
            {
                case NotificationArgs.DownloadAborted:
                    {
                        if (notificationContent.Length is 0)
                        {
                            return;
                        }

                        // 有任务处于正在下载状态时被迫中断显示相应的通知
                        if (notificationContent[0] is "DownloadingNow")
                        {
                            AppNotification notification = new AppNotification(ResourceService.GetLocalized("Notification/DownloadingNowOfflineMode"));
                            notification.ExpiresOnReboot = true;
                            AppNotificationManager.Show(notification);
                        }

                        // 没有任务下载时显示相应的通知
                        else if (notificationContent[0] is "NotDownload")
                        {
                            AppNotification notification = new AppNotification(ResourceService.GetLocalized("Notification/NotDownloadOfflineMode"));
                            notification.ExpiresOnReboot = true;
                            AppNotificationManager.Show(notification);
                        }
                        break;
                    }

                // 安装应用显示相应的通知
                case NotificationArgs.InstallApp:
                    {
                        if (notificationContent.Length is 0)
                        {
                            return;
                        }

                        // 成功安装应用通知
                        if (notificationContent[0] is "Successfully")
                        {
                            AppNotification notification = new AppNotification(string.Format(ResourceService.GetLocalized("Notification/InstallSuccessfully"), notificationContent[1]));
                            notification.ExpiresOnReboot = true;
                            AppNotificationManager.Show(notification);
                        }
                        else if (notificationContent[0] is "Error")
                        {
                            AppNotification notification = new AppNotification(string.Format(ResourceService.GetLocalized("Notification/InstallError"), notificationContent[1], notificationContent[2]));
                            notification.ExpiresOnReboot = true;
                            AppNotificationManager.Show(notification);
                        }
                        break;
                    }

                // 所有任务下载完成时显示通知
                case NotificationArgs.DownloadCompleted:
                    {
                        AppNotification notification = new AppNotification(ResourceService.GetLocalized("Notification/DownloadCompleted"));
                        notification.ExpiresOnReboot = true;
                        AppNotificationManager.Show(notification);
                        break;
                    }
                // 应用安装成功通知
                case NotificationArgs.InstallSuccessfully:
                    {
                        AppNotification notification = new AppNotification(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallSuccessfully"), notificationContent[0]));
                        notification.ExpiresOnReboot = true;
                        AppNotificationManager.Show(notification);
                        break;
                    }
                // 应用安装失败通知
                case NotificationArgs.InstallFailed:
                    {
                        AppNotification notification = new AppNotification(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallFailed"), notificationContent[0]));
                        notification.ExpiresOnReboot = true;
                        AppNotificationManager.Show(notification);
                        break;
                    }
                // 应用卸载成功通知
                case NotificationArgs.UnInstallSuccessfully:
                    {
                        AppNotification notification = new AppNotification(string.Format(ResourceService.GetLocalized("Notification/WinGetUnInstallSuccessfully"), notificationContent[0]));
                        notification.ExpiresOnReboot = true;
                        AppNotificationManager.Show(notification);
                        break;
                    }
                // 应用卸载失败通知
                case NotificationArgs.UnInstallFailed:
                    {
                        AppNotification notification = new AppNotification(string.Format(ResourceService.GetLocalized("Notification/WinGetUnInstallFailed"), notificationContent[0]));
                        notification.ExpiresOnReboot = true;
                        AppNotificationManager.Show(notification);
                        break;
                    }
                // 应用升级成功通知
                case NotificationArgs.UpgradeSuccessfully:
                    {
                        AppNotification notification = new AppNotification(string.Format(ResourceService.GetLocalized("Notification/WinGetUpgradeSuccessfully"), notificationContent[0]));
                        notification.ExpiresOnReboot = true;
                        AppNotificationManager.Show(notification);
                        break;
                    }
                // 应用升级失败通知
                case NotificationArgs.UpgradeFailed:
                    {
                        AppNotification notification = new AppNotification(string.Format(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed"), notificationContent[0]));
                        notification.ExpiresOnReboot = true;
                        AppNotificationManager.Show(notification);
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 注销应用通知
        /// </summary>
        public static void Unregister()
        {
            AppNotificationManager.NotificationInvoked -= OnNotificationInvoked;
            AppNotificationManager.Unregister();
        }
    }
}
