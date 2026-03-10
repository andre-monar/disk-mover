# 📁 [DiskMover](https://github.com/andre-monar/disk-mover/releases/download/v1.1.0/DiskMover.exe)

**DiskMover** is a Windows utility that lets you move files and folders between different drives while keeping everything working through symbolic links. Perfect for freeing up space on small SSDs!

## 🎯 The problem it solves

Your C: drive is full, but you have plenty of space on D:. You want to move heavy folders (like AppData, game libraries, Downloads) to free up space, but programs expect them to be in their original location.

**DiskMover solves this by:**
1. Moving the content to another drive
2. Creating a **symbolic link** at the original location pointing to the new place
3. Everything continues working as if nothing changed!

## ⚡ Three Operation Modes

### 🔗 **Link Only**
Creates a symbolic link at the destination pointing to the source.  
*Use this to "redirect" a folder without moving anything.*

### ➡️ **Move Only**
Simply moves the file/folder to the destination.  
*Like regular cut+paste, but works across drives.*

### 🔄 **Move + Link (The Main Feature)**
1. Moves content to destination (frees space on source drive)
2. Creates a link at the original location pointing to the new location

**Example:**  
`C:\Users\You\AppData\Local\Game` → MOVES to → `D:\Games\GameData`  
Creates LINK at `C:\Users\You\AppData\Local\Game` pointing to `D:\Games\GameData`

**Result:** Game still finds its data in AppData, but it's actually on D:!

## ✨ Key Features

### 🔗 **Link Creation**
- **Folders:** Creates junctions (`/J`)
- **Files:** Creates symbolic links or hard links on same drive
- Automatically detects if source is file or folder
- Validates target drive is NTFS (links only work on NTFS)

### 🔄 **Cross-Drive Moving**
- Automatically detects if source and destination are on the same drive
- Same drive → instant move (Directory.Move/File.Move)
- Different drives → copies files with progress dialog, then deletes originals

### ↩️ **Undo & Link Management**

- **Smart Undo:** Saves last Move/Move+Link operations - buttons appear only when available
- **Link Deletion:** Select and safely delete links
- **Works with both files and folders**
- **Warning system** prevents accidental deletion of non-link items

### 🔀 **Path Swap**
- One-click swap between source and target paths
- Perfect for reverse operations

### 🛡️ **Safety Features**
- Admin privilege detection and auto-restart
- NTFS validation before creating links
- Checks if target already exists before operations
- Validates source exists before any operation
- Cannot move system folders (warns user)
- Undo system prevents data loss

### 🖥️ **User Experience**
- Progress dialogs for long operations (cross-drive moves)
- Clear warning messages about **risks**!
- Suggests safe folders to move (AppData, game libraries, etc.)
- Context-aware browse dialogs (remembers last location)

## 🧪 Technical Implementation

### 📁 **FolderFileDialog Class**
The custom dialog solves a limitation of Windows Forms: there's no native dialog to select both files AND folders. Using API `SHBrowseForFolder` with `BIF_BROWSEINCLUDEFILES` flag, we created a dialog that:

- Shows both files and folders
- Allows selecting any item
- Starts in My Computer but allows full navigation
- Uses callback system to set initial folder without locking navigation

### ↩️ **Undo System Design**
Two separate undo systems:
- **`lastMoveSource`/`lastMoveTarget`**: For Move Only operations
- **`lastMoveLinkSource`/`lastMoveLinkTarget`**: For Move + Link operations

Buttons visibility is controlled by checking if these variables are set, providing clear visual feedback.

### 🔗 **Link Detection**
Uses `FileAttributes.ReparsePoint` to identify if a path is a symbolic link or junction, preventing accidental deletion of real data.

## ⚠️ Important Warnings

- 🔸 Only move **NON-CRITICAL** data. **Some bugs may cause data loss.**
- 🔸 Backup important data before moving
- 🔸 **Never move system folders (Windows, Program Files)**

## ✅ Generally Safe to Move (frees disk space!)

- **AppData** (game saves, app configurations)
- **Steam/Epic/GOG** game libraries
- **Downloads** folder
- **Documents, Pictures, Videos, Music**
- **Large project folders** (coding, design)
- **Virtual machine disks**

## 📥 Download

Single `.exe` file - [**Download here!**](https://github.com/andre_monar/disk-mover/releases/latest)

## 🖥️ Requirements

- Windows 7 or later
- .NET Framework 4.7.2 (pre-installed on Windows 10/11)
- NTFS drives for link creation

## 🐛 Reporting Issues

Found a bug? Have a suggestion? Open an issue on GitHub!
