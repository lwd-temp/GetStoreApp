using GetStoreApp.Contracts.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.ContentDialogs.Common
{
    public sealed partial class AppRunningDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public AppRunningDialog()
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            InitializeComponent();
        }
    }
}