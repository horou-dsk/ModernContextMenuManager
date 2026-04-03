using Avalonia.Media.Imaging;
using ModernContextMenuManager.Base;
using ModernContextMenuManager.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernContextMenuManager.Models
{
    public partial class PackagedAppModel : ObservableObject
    {
        private bool updatingAllEnabled;

        public PackagedAppModel(
            AppInfo appInfo,
            PackageInfo packageInfo,
            Dictionary<Guid, PackagedComHelper.BlockedClsidType> blockedClsids)
        {
            AppInfo = appInfo;
            PackageInfo = packageInfo;
            ContextMenuItems = appInfo.ContextMenuItems.Select(c =>
            {
                if (blockedClsids.TryGetValue(c.Clsid, out var blockedClsidType))
                {
                    return new ContextMenuItemCheckModel(c, false, blockedClsidType != PackagedComHelper.BlockedClsidType.LocalMachine, packageInfo.PackageFullName);
                }
                return new ContextMenuItemCheckModel(c, true, true, packageInfo.PackageFullName);
            }).ToArray();

            foreach (var item in ContextMenuItems)
            {
                item.PropertyChanged += OnChildEnabledChanged;
            }

            ToggleAllCommand = new RelayCommand(ToggleAll);

            if (string.IsNullOrEmpty(AppInfo.DisplayName))
            {
                DisplayName = $"{PackageInfo.PackageFamilyName}";
            }
            else
            {
                DisplayName = $"{AppInfo.DisplayName}\n{PackageInfo.PackageFamilyName}";
            }
        }

        public AppInfo AppInfo { get; }

        public PackageInfo PackageInfo { get; }

        public IReadOnlyList<ContextMenuItemCheckModel> ContextMenuItems { get; }

        public string DisplayName { get; }

        public RelayCommand ToggleAllCommand { get; }

        public bool? AllEnabled
        {
            get
            {
                var modifiable = ContextMenuItems.Where(c => c.CanModify).ToArray();
                if (modifiable.Length == 0) return null;

                bool allEnabled = modifiable.All(c => c.Enabled);
                bool allDisabled = modifiable.All(c => !c.Enabled);

                if (allEnabled) return true;
                if (allDisabled) return false;
                return null;
            }
        }

        private void ToggleAll()
        {
            var modifiable = ContextMenuItems.Where(c => c.CanModify).ToArray();
            if (modifiable.Length == 0) return;

            // If not all enabled, enable all; otherwise disable all
            bool newValue = !modifiable.All(c => c.Enabled);

            updatingAllEnabled = true;
            try
            {
                foreach (var item in modifiable)
                {
                    item.SetEnabledSilently(newValue);
                }
                foreach (var item in modifiable)
                {
                    item.NotifyEnabledChanged();
                }
            }
            finally
            {
                updatingAllEnabled = false;
                OnPropertyChanged(nameof(AllEnabled));
            }
        }

        public bool HasModifiableItems => ContextMenuItems.Any(c => c.CanModify);

        private void OnChildEnabledChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ContextMenuItemCheckModel.Enabled) && !updatingAllEnabled)
            {
                OnPropertyChanged(nameof(AllEnabled));
            }
        }

        public Bitmap? Icon
        {
            get
            {
                if (!string.IsNullOrEmpty(AppInfo.IconPath))
                {
                    var iconPath = System.IO.Path.Combine(PackageInfo.PackageInstallLocation, AppInfo.IconPath);
                    if (System.IO.File.Exists(iconPath))
                    {
                        return new Bitmap(iconPath);
                    }
                }
                return null;
            }
        }
    }
}
