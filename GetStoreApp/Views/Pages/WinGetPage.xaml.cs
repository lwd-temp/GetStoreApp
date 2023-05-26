using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet �����ҳ��
    /// </summary>
    public sealed partial class WinGetPage : Page
    {
        public WinGetPage()
        {
            InitializeComponent();
            InitializeControlsVieModel();
        }

        private void InitializeControlsVieModel()
        {
            SearchApps.ViewModel.WinGetVMInstance = ViewModel;
            UpgradableApps.ViewModel.WinGetVMInstance = ViewModel;
        }
    }
}
