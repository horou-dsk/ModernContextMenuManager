# Modern ContextMenu Manager

A modern Windows 11 context menu manager built with Avalonia UI. Manage (enable/disable) context menu items registered by packaged apps (MSIX/AppX).

## Known Issues

### Disabling one menu type affects other types from the same app

Some packaged apps register multiple context menu types (e.g., `*` for files, `Directory` for folders, `Directory\Background` for folder backgrounds) using the **same CLSID**. Since the Windows Shell Extensions blocking mechanism works at the CLSID level (`HKCU\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked`), it is not possible to block a specific type independently. Disabling any one of them will disable all menu items sharing that CLSID.

This is a limitation of the Windows Shell Extensions blocking mechanism, not a bug in this application.

---

# Modern ContextMenu Manager

一个基于 Avalonia UI 构建的现代 Windows 11 右键菜单管理工具。用于管理（启用/禁用）由打包应用（MSIX/AppX）注册的右键菜单项。

## 已知问题

### 禁用某一类型的菜单项会影响同应用的其他类型

部分打包应用会将多种右键菜单类型（如 `*` 文件、`Directory` 文件夹、`Directory\Background` 文件夹背景）注册到**同一个 CLSID** 上。由于 Windows Shell Extensions 的屏蔽机制是基于 CLSID 级别的（`HKCU\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked`），无法单独屏蔽某一类型。禁用其中任意一项，所有共享该 CLSID 的菜单项都会被一并禁用。

这是 Windows Shell Extensions 屏蔽机制的限制，并非本应用的 Bug。
