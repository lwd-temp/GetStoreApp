﻿using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using GetStoreApp.WindowsAPI.PInvoke.Winrtact;
using Microsoft.Management.Deployment;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// WinGet 服务
    /// </summary>
    public static class WinGetService
    {
        // 普通版本的GUID（CLSID）值
        private static Guid clsid_PackageManager = new("C53A4F16-787E-42A4-B304-29EFFB4BF597");

        private static Guid clsid_InstallOptions = new("1095F097-EB96-453B-B4E6-1613637F3B14");
        private static Guid clsid_UnInstallOptions = new("E1D9A11E-9F85-4D87-9C17-2B93143ADB8D");
        private static Guid clsid_FindPackagesOptions = new("572DED96-9C60-4526-8F92-EE7D91D38C1A");
        private static Guid clsid_PackageMatchFilter = new("D02C9DAF-99DC-429C-B503-4E504E4AB000");
        private static Guid clsid_CreateCompositePackageCatalogOptions = new("526534B8-7E46-47C8-8416-B1685C327D37");

        // 开发版本的GUID（CLSID）值
        private static Guid clsid_PackageManager_Dev = new("74CB3139-B7C5-4B9E-9388-E6616DEA288C");

        private static Guid clsid_InstallOptions_Dev = new("44FE0580-62F7-44D4-9E91-AA9614AB3E86");
        private static Guid clsid_UnInstallOptions_Dev = new("AA2A5C04-1AD9-46C4-B74F-6B334AD7EB8C");
        private static Guid clsid_FindPackagesOptions_Dev = new("1BD8FF3A-EC50-4F69-AEEE-DF4C9D3BAA96");
        private static Guid clsid_PackageMatchFilter_Dev = new("3F85B9F4-487A-4C48-9035-2903F8A6D9E8");
        private static Guid clsid_CreateCompositePackageCatalogOptions_Dev = new("EE160901-B317-4EA7-9CC6-5355C6D7D8A7");

        // COM接口：IUnknown 接口
        private static Guid iid_IUnknown = new("00000000-0000-0000-C000-000000000046");

        private static bool actuallyUseDev = false;

        public static bool IsOfficialVersionExisted { get; private set; } = false;

        public static bool IsDevVersionExisted { get; private set; } = false;

        /// <summary>
        /// 判断 WinGet 服务是否存在
        /// </summary>
        public static void InitializeService()
        {
            Task.Run(() =>
            {
                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_PackageManager, ref iid_IUnknown, 0, out IntPtr obj);
                    IsOfficialVersionExisted = obj != IntPtr.Zero;
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_PackageManager, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    IsOfficialVersionExisted = obj != IntPtr.Zero;
                }

                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_PackageManager_Dev, ref iid_IUnknown, 0, out IntPtr obj);
                    IsDevVersionExisted = obj != IntPtr.Zero;
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_PackageManager_Dev, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    IsDevVersionExisted = obj != IntPtr.Zero;
                }

                if (WinGetConfigService.UseDevVersion)
                {
                    if (IsDevVersionExisted)
                    {
                        actuallyUseDev = true;
                    }
                }
            });
        }

        /// <summary>
        /// 创建包管理器
        /// </summary>
        public static PackageManager CreatePackageManager()
        {
            if (actuallyUseDev)
            {
                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_PackageManager_Dev, ref iid_IUnknown, 0, out IntPtr obj);
                    return PackageManager.FromAbi(obj);
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_PackageManager_Dev, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    return PackageManager.FromAbi(obj);
                }
            }
            else
            {
                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_PackageManager, ref iid_IUnknown, 0, out IntPtr obj);
                    return PackageManager.FromAbi(obj);
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_PackageManager, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    return PackageManager.FromAbi(obj);
                }
            }
        }

        /// <summary>
        /// 创建安装选项
        /// </summary>
        public static InstallOptions CreateInstallOptions()
        {
            if (actuallyUseDev)
            {
                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_InstallOptions_Dev, ref iid_IUnknown, 0, out IntPtr obj);
                    return InstallOptions.FromAbi(obj);
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_InstallOptions_Dev, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    return InstallOptions.FromAbi(obj);
                }
            }
            else
            {
                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_InstallOptions, ref iid_IUnknown, 0, out IntPtr obj);
                    return InstallOptions.FromAbi(obj);
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_InstallOptions, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    return InstallOptions.FromAbi(obj);
                }
            }
        }

        /// <summary>
        /// 创建卸载选项
        /// </summary>
        public static UninstallOptions CreateUninstallOptions()
        {
            if (actuallyUseDev)
            {
                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_UnInstallOptions_Dev, ref iid_IUnknown, 0, out IntPtr obj);
                    return UninstallOptions.FromAbi(obj);
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_UnInstallOptions_Dev, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    return UninstallOptions.FromAbi(obj);
                }
            }
            else
            {
                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_UnInstallOptions, ref iid_IUnknown, 0, out IntPtr obj);
                    return UninstallOptions.FromAbi(obj);
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_UnInstallOptions, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    return UninstallOptions.FromAbi(obj);
                }
            }
        }

        /// <summary>
        /// 创建查找包选项
        /// </summary>
        public static FindPackagesOptions CreateFindPackagesOptions()
        {
            if (actuallyUseDev)
            {
                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_FindPackagesOptions_Dev, ref iid_IUnknown, 0, out IntPtr obj);
                    return FindPackagesOptions.FromAbi(obj);
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_FindPackagesOptions_Dev, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    return FindPackagesOptions.FromAbi(obj);
                }
            }
            else
            {
                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_FindPackagesOptions, ref iid_IUnknown, 0, out IntPtr obj);
                    return FindPackagesOptions.FromAbi(obj);
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_FindPackagesOptions, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    return FindPackagesOptions.FromAbi(obj);
                }
            }
        }

        /// <summary>
        /// 创建复合包目录选项
        /// </summary>
        public static CreateCompositePackageCatalogOptions CreateCreateCompositePackageCatalogOptions()
        {
            if (actuallyUseDev)
            {
                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_CreateCompositePackageCatalogOptions_Dev, ref iid_IUnknown, 0, out IntPtr obj);
                    return CreateCompositePackageCatalogOptions.FromAbi(obj);
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_CreateCompositePackageCatalogOptions_Dev, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    return CreateCompositePackageCatalogOptions.FromAbi(obj);
                }
            }
            else
            {
                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_CreateCompositePackageCatalogOptions, ref iid_IUnknown, 0, out IntPtr obj);
                    return CreateCompositePackageCatalogOptions.FromAbi(obj);
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_CreateCompositePackageCatalogOptions, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    return CreateCompositePackageCatalogOptions.FromAbi(obj);
                }
            }
        }

        /// <summary>
        /// 创建包查询匹配过滤选项
        /// </summary>
        public static PackageMatchFilter CreatePacakgeMatchFilter()
        {
            if (actuallyUseDev)
            {
                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_PackageMatchFilter_Dev, ref iid_IUnknown, 0, out IntPtr obj);
                    return PackageMatchFilter.FromAbi(obj);
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_PackageMatchFilter_Dev, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    return PackageMatchFilter.FromAbi(obj);
                }
            }
            else
            {
                if (RuntimeHelper.IsElevated)
                {
                    WinrtactLibrary.WinGetServerManualActivation_CreateInstance(ref clsid_PackageMatchFilter, ref iid_IUnknown, 0, out IntPtr obj);
                    return PackageMatchFilter.FromAbi(obj);
                }
                else
                {
                    Ole32Library.CoCreateInstance(ref clsid_PackageMatchFilter, IntPtr.Zero, CLSCTX.CLSCTX_ALL, ref iid_IUnknown, out IntPtr obj);
                    return PackageMatchFilter.FromAbi(obj);
                }
            }
        }
    }
}
