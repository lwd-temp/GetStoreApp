﻿using GetStoreApp.Helpers.Converters;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text;

namespace GetStoreApp.ViewModels.Dialogs.Download
{
    /// <summary>
    /// 文件信息对话框视图模型
    /// </summary>
    public sealed class FileInformationViewModel : ViewModelBase
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileSize { get; set; }

        public string FileSHA1 { get; set; }

        public bool FileSHA1Load { get; set; } = true;

        private bool _fileCheckState = false;

        public bool FileCheckState
        {
            get { return _fileCheckState; }

            set
            {
                _fileCheckState = value;
                OnPropertyChanged();
            }
        }

        private string _checkFileSHA1;

        public string CheckFileSHA1
        {
            get { return _checkFileSHA1; }

            set
            {
                _checkFileSHA1 = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 初始化信息
        /// </summary>
        public async void OnLoaded(object sender, RoutedEventArgs args)
        {
            CheckFileSHA1 = await IOHelper.GetFileSHA1Async(FilePath);
            FileCheckState = true;
        }

        /// <summary>
        /// 复制文件信息
        /// </summary>
        public void OnCopyFileInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/FileName") + FileName);
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/FilePath") + FilePath);
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/FileSize") + FileSize);
            if (FileSHA1Load)
            {
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/FileSHA1") + FileSHA1);
            }
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/CheckFileSHA1") + CheckFileSHA1);

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            sender.Hide();
            new FileInformationCopyNotification().Show();
        }

        /// <summary>
        /// 初始化文件信息
        /// </summary>
        public void InitializeFileInformation(CompletedModel completedItem)
        {
            FileName = completedItem.FileName;
            FilePath = completedItem.FilePath;
            FileSize = StringConverterHelper.DownloadSizeFormat(completedItem.TotalSize);
            if (FileSHA1 is "WebDownloadUnknown")
            {
                FileSHA1Load = false;
            }
            else
            {
                FileSHA1 = completedItem.FileSHA1;
            }
        }
    }
}
