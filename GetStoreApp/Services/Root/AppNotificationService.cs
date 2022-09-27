﻿using CommunityToolkit.WinUI.Notifications;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Specialized;
using System.Web;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用通知服务
    /// </summary>
    public class AppNotificationService : IAppNotificationService
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        public INotificationService NotificationService { get; } = IOCHelper.GetService<INotificationService>();

        private string InstallTag { get; } = "InstallTag";

        private string InstallGroup { get; } = "InstallGroup";

        ~AppNotificationService()
        {
            Unregister();
        }

        public void Initialize()
        {
            AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;

            AppNotificationManager.Default.Register();
        }

        /// <summary>
        /// 处理应用通知后的响应事件
        /// </summary>
        public async void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            // 打开网络设置（不涉及到UI主线程）
            if (ParseArguments(args.Argument)["AppNotifications"] == "CheckNetWorkConnection")
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
            }
            // 涉及到UI主线程
            else
            {
                App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    // 先设置应用窗口的显示方式
                    WindowHelper.ShowAppWindow();

                    // 根据点击通知获取到的参数来选择相应的操作
                    if (ParseArguments(args.Argument)["AppNotifications"] == "DownloadingNow" && NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                    {
                        NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                    }

                    if (ParseArguments(args.Argument)["AppNotifications"] == "NotDownload" && NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                    {
                        NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                    }

                    if (ParseArguments(args.Argument)["AppNotifications"] == "ViewDownloadPage" && NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                    {
                        NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                    }

                    if (ParseArguments(args.Argument)["AppNotifications"] == "DownloadCompleted")
                    {
                        if (NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                        {
                            NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                        }

                        //App.NavigationArgs = "DownloadCompleted";
                    }
                });
            }
        }

        /// <summary>
        /// 解析应用通知返回的参数
        /// </summary>
        private NameValueCollection ParseArguments(string arguments)
        {
            return HttpUtility.ParseQueryString(arguments);
        }

        /// <summary>
        /// 显示通知
        /// </summary>
        public void Show(string notificationKey, params string[] notificationContent)
        {
            if (!NotificationService.AppNotification)
            {
                return;
            }

            switch (notificationKey)
            {
                case "DownloadAborted":
                    {
                        if (notificationContent.Length == 0)
                        {
                            return;
                        }

                        // 有任务处于正在下载状态时被迫中断显示相应的通知
                        if (notificationContent[0] == "DownloadingNow")
                        {
                            new ToastContentBuilder().AddArgument("AppNotifications", notificationKey)
                                .AddText(ResourceService.GetLocalized("/Notification/OfflineMode"))

                                .AddText(ResourceService.GetLocalized("/Notification/DownloadingAborted"))

                                .AddButton(new ToastButton()
                                    .SetContent(ResourceService.GetLocalized("/Notification/ViewDownloadPage"))
                                    .AddArgument("AppNotifications", "ViewDownloadPage")
                                    .SetBackgroundActivation())

                                .AddButton(new ToastButton()
                                    .SetContent(ResourceService.GetLocalized("/Notification/CheckNetWorkConnection"))
                                    .AddArgument("AppNotifications", "CheckNetWorkConnection")
                                    .SetBackgroundActivation())
                                .Show();
                        }

                        // 没有任务下载时显示相应的通知
                        else if (notificationContent[0] == "NotDownload")
                        {
                            new ToastContentBuilder().AddArgument("AppNotifications", notificationKey)
                                .AddText(ResourceService.GetLocalized("/Notification/OfflineMode"))

                                .AddText(ResourceService.GetLocalized("/Notification/NotDownload"))

                                .AddButton(new ToastButton()
                                    .SetContent(ResourceService.GetLocalized("/Notification/CheckNetWorkConnection"))
                                    .AddArgument("AppNotifications", "CheckNetWorkConnection")
                                    .SetBackgroundActivation())
                                .Show();
                        }
                        break;
                    }

                // 所有任务下载完成时显示通知
                case "DownloadingCompleted":
                    {
                        new ToastContentBuilder().AddArgument("AppNotifications", notificationKey)
                            .AddText(ResourceService.GetLocalized("/Notification/DownloadCompleted"))

                            .AddText(ResourceService.GetLocalized("/Notification/DownloadCompletedDescription"))

                            .AddButton(new ToastButton()
                                .SetContent(ResourceService.GetLocalized("/Notification/ViewDownloadPage"))
                                .AddArgument("AppNotifications", "ViewDownloadPage")
                                .SetBackgroundActivation())
                            .Show();
                        break;
                    }

                // 安装应用显示相应的通知
                case "InstallApp":
                    {
                        if (notificationContent.Length == 0)
                        {
                            return;
                        }

                        // 成功安装应用通知
                        if (notificationContent[0] == "Successfully")
                        {
                            new ToastContentBuilder().AddText(string.Format(ResourceService.GetLocalized("/Notification/InstallSuccessfully"), notificationContent[1])).Show();
                        }
                        else if (notificationContent[0] == "Error")
                        {
                            new ToastContentBuilder().AddText(string.Format(ResourceService.GetLocalized("/Notification/InstallError"), notificationContent[1]))
                                .AddText(string.Format(ResourceService.GetLocalized("/Notification/InstallErrorDescription"), notificationContent[2]))
                                .Show();
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 注销通知服务
        /// </summary>
        public void Unregister()
        {
            AppNotificationManager.Default.Unregister();
        }
    }
}
