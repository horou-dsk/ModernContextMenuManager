using ModernContextMenuManager.Base;
using ModernContextMenuManager.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernContextMenuManager.Models
{
    public partial class ContextMenuItemCheckModel : ObservableObject
    {
        private bool enabled;

        public ContextMenuItemCheckModel(ContextMenuItem contextMenuItem, bool enabled, bool canModify, string packageFullName)
        {
            ContextMenuItem = contextMenuItem;
            CanModify = canModify;
            this.enabled = enabled;
            var clsidStr = contextMenuItem.Clsid.ToString("B").ToUpperInvariant();
            var verb = !string.IsNullOrEmpty(contextMenuItem.Id) ? contextMenuItem.Id : clsidStr;
            var type = !string.IsNullOrEmpty(contextMenuItem.Type) ? contextMenuItem.Type : "*";
            VirtualRegistryPath = $@"HKEY_CLASSES_ROOT\{type}\shell\{verb}\ExplorerCommandHandler";
            PhysicalRegistryPath = $@"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\PackagedCom\Package\{packageFullName}\Class\{clsidStr}";
        }

        public ContextMenuItem ContextMenuItem { get; }

        public bool CanModify { get; }

        public string VirtualRegistryPath { get; }

        public string PhysicalRegistryPath { get; }

        public bool Enabled
        {
            get => enabled;
            set => SetProperty(ref enabled, value,
                onPropertyChanging: (oldValue, newValue) =>
                    PackagedComHelper.SetBlockedClsid(ContextMenuItem.Clsid, PackagedComHelper.BlockedClsidType.CurrentUser, !newValue),
                notifyWhenNotChanged: true,
                asyncNotifyWhenNotChanged: true);
        }

        internal void SetEnabledSilently(bool value)
        {
            if (enabled == value) return;
            // Registry op may "fail" if another item with the same CLSID
            // already added/removed the value — still update the field.
            PackagedComHelper.SetBlockedClsid(ContextMenuItem.Clsid, PackagedComHelper.BlockedClsidType.CurrentUser, !value);
            enabled = value;
        }

        internal void NotifyEnabledChanged()
        {
            OnPropertyChanged(nameof(Enabled));
        }

        public string DisplayName
        {
            get
            {
                var title = ContextMenuItem.Title;
                if (string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(ContextMenuItem.Id)) title = $"[{ContextMenuItem.Id}]";

                if (!string.IsNullOrEmpty(ContextMenuItem.Type) && !string.IsNullOrEmpty(title))
                {
                    return $"[{ContextMenuItem.Type}] {title}\n{ContextMenuItem.Clsid:B}";
                }
                else if (!string.IsNullOrEmpty(ContextMenuItem.Type))
                {
                    return $"[{ContextMenuItem.Type}]\n{ContextMenuItem.Clsid:B}";
                }
                else if (!string.IsNullOrEmpty(title))
                {
                    return $"{title}\n{ContextMenuItem.Clsid:B}";
                }

                return $"{ContextMenuItem.Clsid:B}";
            }
        }
    }
}
