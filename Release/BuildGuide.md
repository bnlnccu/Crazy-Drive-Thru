# Unity Game Build SOP (UnityPlay / itch.io)

## Prerequisites

- Unity 2022.3.x (LTS) installed via Unity Hub
- Build module installed for your target platform:
  - **WebGL**: `WebGL Build Support` (strongly recommended: browser-based, zero install for players)
  - If missing: Unity Hub > Installs > your version > Add Modules


> **CRITICAL PATH WARNING:** Your entire Unity project **MUST** be in an **English-only** file path (e.g., `C:\UnityProjects\MyGame`). If your Windows username or any folder contains Chinese/non-English characters (e.g., `C:\Users\陳小明\桌面`), **WebGL compilation WILL FAIL** with cryptic C++ errors. Move the project to a safe path before building.

---

## Step 1: Verify Scene & Settings

### 1.1 Check Scenes in Build

1. Open Unity menu: **File > Build Settings** (shortcut: `Ctrl+Shift+B` / `Cmd+Shift+B`)
2. Check **Scenes In Build** list
3. Confirm your main scene is listed and checkbox is checked
4. If the list is empty:
   - Click **Add Open Scenes** (make sure your main scene is open)
   - It should appear with index 0

> **Common mistake:** If you see multiple scenes listed, make sure your main scene is index 0 (the first scene that loads). Drag to reorder if needed.

### 1.2 Force Windowed Mode (Prevents Fullscreen Panic)

1. In Build Settings, click **Player Settings** (bottom-left)
2. Select **Player** > expand **Resolution and Presentation**
3. Set **Fullscreen Mode** to **Windowed**
4. Default Screen Width: `1280`, Height: `720`

---

## Step 2: Choose Your Build Strategy

### Option A: WebGL (Recommended)

*Zero install, cross-platform, anyone can play via browser URL.*

1. In Build Settings, select **WebGL** and click **Switch Platform** (wait 1-2 min)
![这是图片](../img/WebGL01.png "Magic Gardens")
2. Click **Build**, create a folder named `Builds_WebGL`
3. Wait for compilation (~3-5 minutes, longer than local builds)
4. Output: a folder containing `index.html`
5. Zip the **contents** of the `Builds_WebGL` folder (make sure `index.html` is inside the zip root)
6. Upload to [Unity Play](https://play.unity.com/):
   - Log in with your Unity account
   - Click **Post Game** (top-right)
   - Fill in title, drag-and-drop the `.zip` file
   - Publish and share the URL

> **DO NOT** double-click `index.html` to test! Web browsers block local file execution (CORS policy). You **MUST** upload to Unity Play, or use **"Build And Run"** in Unity (which spins up a temporary local server).

> **First-time WebGL build warning:** The first WebGL build may take **15-30 minutes** on a standard laptop. DO NOT force close Unity. Be patient.

### Option B: Local Build (Windows / Mac)

*Use when you need precise hardware performance and millisecond-level RT measurements.*

In Build Settings, select **PC, Mac & Linux Standalone**. Then configure the right panel:

| Your Computer | Target OS | Architecture |
|---|---|---|
| Windows | Windows | Intel 64-bit |
| Mac (Intel) | macOS | Intel 64-bit |
| Mac (Apple Silicon) | macOS | Apple Silicon |

If your current platform is different from the target, click **Switch Platform** and wait for re-import (may take 1-2 minutes).

---

## Step 3: Build (Local)

1. Click **Build** (not "Build And Run")
2. Choose an output folder:
   - Create a new folder like `Builds/Windows` or `Builds/Mac` inside your project
   - **Windows**: Name the file `YourGameName.exe`
   - **Mac**: Name the file `YourGameName`
3. Wait for build to complete (about 30-60 seconds)
4. If successful, Unity will open the output folder

---

## Step 4: Run the Build

**Windows:**
- Navigate to the output folder
- Double-click `YourGameName.exe`
- If Windows Defender warns you, click "More info" > "Run anyway"

**Mac:**
- Navigate to the output folder
- Double-click `YourGameName.app`
- If macOS blocks it: Right-click > Open > "Open" in the dialog
- Or: System Settings > Privacy & Security > "Open Anyway"

---

## Step 5: Distribute

To share the built game with others:

**Windows:**
- Zip the **entire output folder** (not just the .exe)
- Must include: `YourGameName.exe` + `YourGameName_Data/` folder + `UnityPlayer.dll`
- Missing any of these = game won't run
- **Google Drive warning:** Google Classroom / Google Drive often flags `.zip` files containing `.exe` or `.dll` as viruses. If classmates cannot download your Local Build, use the **WebGL (Option A)** method instead.

**Mac:**
- Zip the `YourGameName.app` bundle
- Recipients may need to bypass Gatekeeper (right-click > Open)

---

## Troubleshooting

### Build fails with "No scenes in build"
- File > Build Settings > Add Open Scenes

### Build fails with compilation errors
- Check Console window (Window > General > Console)
- Fix all red errors before building
- Yellow warnings are usually OK

### Game runs but screen is black
- Your main scene might not be index 0 in Build Settings
- Or essential prefabs/objects are missing from the scene

### Windows: clicking .exe causes black screen / instant crash
- **Unzip trap!** You must NOT double-click the .exe inside the zip preview window
- Right-click the .zip > "Extract All..." > open the extracted folder > then run the .exe

### Windows Defender blocks the game
- This is normal for unsigned executables
- Click "More info" > "Run anyway"
- Or add the folder to Windows Defender exclusions

### Mac blocks the app ("unidentified developer")
- Right-click the .app > Open > Click "Open" in dialog
- This only needs to be done once

### Mac shows "file is damaged, move to trash"
- This is macOS Gatekeeper quarantine, not actual file corruption
- **Fix:**
  1. Open **Terminal** (`Cmd + Space`, search "Terminal")
  2. Type `xattr -cr ` (note: there must be a space after `cr`)
  3. **Drag** the .app file from Finder into the Terminal window
  4. Press Enter
  5. Double-click the app again — it will now open

### Build is very large (> 200MB)
- File > Build Settings > Player Settings > Other Settings
- Set **Managed Stripping Level** to "Medium" or "High"
- Uncheck **Development Build** if it's checked

### WebGL: Color Space error during build
- Player Settings > Other Settings > set **Color Space** from `Linear` to `Gamma`

### WebGL: game crashes in browser (Out of Memory)
- Your images or audio files are too large
- Select images in Project window > Inspector > reduce **Max Size** to `1024`
- Select audio files > Inspector > set **Load Type** to `Compressed in Memory`
- Rebuild

---

## Quick Reference

| Item | Value |
|---|---|
| Scene | Your main scene (index 0 in Build Settings) |
| Build target | WebGL (recommended) / Windows x86_64 / macOS |
| Expected build size | ~80 MB (Windows), varies for WebGL |
| Expected build time | ~30-60 sec (local), ~3-5 min (WebGL) |
| Output (Windows) | `YourGameName.exe` + `YourGameName_Data/` folder + `UnityPlayer.dll` |
| Output (Mac) | `YourGameName.app` bundle |
| Output (WebGL) | folder with `index.html` → zip → upload to Unity Play |
| Recommended resolution | 1280 x 720, Windowed mode |
