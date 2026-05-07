# PowerHub Lite - Quick Reference Guide

## 🎯 What's New

Your PowerHub Lite app now includes 5 major features:

### 1. 🎨 Display Refresh Rate Control

**Tab:** Display  
**What it does:**

- Shows your current monitor refresh rate
- Lists all supported refresh rates (usually 60Hz, 75Hz, 144Hz, 240Hz, etc.)
- Click "Apply Refresh Rate" to change
- Perfect for gaming vs productivity switching

**Before:** No refresh rate control  
**After:** Full control in dedicated tab

### 2. ⚡ Windows 11 Power Modes

**Tab:** Power Modes  
**What it does:**

- Switch between "Standard Mode" (balanced) and "Battery Saver"
- Integrates with Windows 11 Quick Settings
- Overlay modes on top of active power plan
- One-click activation

**Before:** Only power plans available  
**After:** Proper Windows 11 modes + power plans

### 3. 🔋 Battery Saver Enhancement

**Tab:** Power Modes (child of Power Saver)  
**What it does:**

- Now hierarchical - under power management
- Turns on Windows Energy Saver in Quick Settings
- Better integration with system settings
- Can be mixed with any power plan

**Before:** Flat list of options  
**After:** Proper hierarchical structure

### 4. 🖥️ Display Timeout Fix

**Automatic for:** High Performance & Ultimate Performance modes  
**What it does:**

- Prevents display from turning off when using performance modes
- Sets display idle timeout to NEVER (0 minutes)
- Applies when you select High Performance or Ultimate Performance
- No configuration needed - automatic!

**Before:** Display turned off after 10-15 minutes  
**After:** Display stays always-on in performance modes

### 5. 🎯 App Logo

**Location:** Window title bar  
**What it does:**

- Shows ⚡ PowerHub Lite with professional styling
- Blue accent (#0078D7 - Windows standard color)
- Can be replaced with custom icon (see LOGO_GUIDE.md)

**Before:** Generic window title  
**After:** Professional branding

---

## 📊 Dashboard Tab (Updated)

Shows real-time status:

- ✅ Active Power Plan
- ✅ Power Mode (NEW - shows Standard/Battery Saver)
- ✅ Display Refresh Rate (NEW - e.g., "144 Hz")
- ✅ Battery Saver Status

**"Refresh Info" Button** - Updates all status instantly

---

## 🔧 How to Use Each Feature

### Changing Refresh Rate

1. Open app → "Display" tab
2. See current refresh rate at top
3. Select desired rate from dropdown
4. Click "Apply Refresh Rate"
5. Status shows result (green = success)

### Switching Power Modes

1. Open app → "Power Modes" tab
2. Choose "Standard Mode" or "Battery Saver"
3. Click "Activate"
4. System applies mode + syncs with Quick Settings
5. Go to "Dashboard" to verify

### Applying Power Plans

1. Open app → "Power Plans" tab (original tab)
2. Select plan (Power saver, Balanced, High performance, Ultimate)
3. Click "Activate"
4. Display stays on automatically for performance modes
5. Check Dashboard tab for status

### Checking System Status

1. Go to "Dashboard" tab
2. Review all 4 status items
3. Click "Refresh Info" to update
4. Switch between modes in other tabs and refresh

---

## 📋 Tab Overview

| Tab                 | Purpose                  | Key Features                          |
| ------------------- | ------------------------ | ------------------------------------- |
| **Dashboard**       | System status            | Real-time info, refresh button        |
| **Power Plans**     | Core power management    | 5 pre-configured plans, import/unlock |
| **Power Modes**     | Windows 11 overlay modes | Standard, Battery Saver options       |
| **Display**         | Monitor settings         | Refresh rate selection & apply        |
| **One-Click Boost** | Quick optimization       | System tweaks, cleanup                |

---

## ✅ Verification Checklist

After building, verify these work:

### Refresh Rate

- [ ] Display tab shows current refresh rate
- [ ] Dropdown lists multiple rates
- [ ] Can select and apply a different rate
- [ ] Monitor actually changes refresh rate

### Power Modes

- [ ] Power Modes tab accessible
- [ ] Can activate Standard mode
- [ ] Can activate Battery Saver mode
- [ ] Dashboard updates after activation

### Display Fix

- [ ] Switch to High Performance mode
- [ ] Wait ~5 minutes
- [ ] Display stays on (doesn't turn off)
- [ ] Try Ultimate Performance too

### Power Plans

- [ ] All 5 plans still work
- [ ] Can activate each plan
- [ ] Display checkbox setting is applied
- [ ] Dashboard shows active plan

### Battery Saver

- [ ] Shows in Power Plans list
- [ ] Shows in Power Modes tab
- [ ] Can be activated both ways
- [ ] Status updates correctly

---

## 🏗️ Building the App

### Command Line (Recommended)

```bash
cd "d:\Learning ml\PowerHub"
dotnet build -c Release
```

**Output location:** `src\PowerHub.Lite\bin\Release\net8.0-windows\PowerHubLite.exe`

### Using build.bat

```bash
cd "d:\Learning ml\PowerHub"
build.bat
```

### In Visual Studio

1. Open `PowerHub.sln`
2. Right-click PowerHub.Lite project
3. Select "Build"
4. Check for any errors in Output window

---

## 🚀 Running the App

**IMPORTANT: Run as Administrator!**

```bash
# Right-click exe and select "Run as administrator"
# OR
cd path/to/bin/Release/net8.0-windows
powershell -Command "Start-Process -FilePath .\PowerHubLite.exe -Verb RunAs"
```

**Why admin?**

- Powercfg requires admin for power plan changes
- Display settings require admin access
- Registry modifications need admin
- System tweaks need admin

---

## 🎨 Custom Logo Setup

As described in `LOGO_GUIDE.md`:

1. **Create a logo** (PNG, 256x256):
   - Use online tools, Figma, or Photoshop
   - Design with lightning bolt or power theme
   - Keep it simple and square

2. **Convert to ICO**:
   - Go to https://icoconvert.com/
   - Upload your PNG
   - Download as ICO

3. **Add to project**:
   - Save as `PowerHub.ico` in `src/PowerHub.Lite/`
   - In MainWindow.xaml, uncomment:
     ```xml
     Icon="pack://application:,,,/PowerHub.ico"
     ```
   - Rebuild

---

## 📝 Technical Summary

### New Public Methods in PowerManager

```csharp
GetPowerModeDisplayName(modeGuid)      // "Battery Saver", etc
GetAvailablePowerModes()                // List of modes
ApplyRefreshRate(frequency)             // Apply refresh rate
GetCurrentRefreshRate()                 // Current Hz
GetAvailableRefreshRates()              // List of Hz options
```

### New Private Methods in PowerManager

```csharp
PreserveDisplaySettings(guid)  // Fixes display timeout for performance modes
```

### Power Mode GUIDs

- Standard: `00000000-0000-0000-0000-000000000000`
- Battery Saver: `961cc777-2547-4f9d-8174-7d86181b8a7a`

### Display Setting GUID

- Display: `7516b95f-f776-4464-8c53-06167f40cc99`
- Timeout (AC): `3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e`

---

## 🐛 Troubleshooting

**"Admin required" errors:**

- Right-click exe → Run as administrator
- Use PowerShell with `-Verb RunAs`

**Refresh rate not changing:**

- Check if your monitor supports the selected rate
- Some rates only work with specific drivers
- Try a rate closer to current (e.g., 60→75 instead of 60→240)

**Power Modes not syncing with Quick Settings:**

- Restart ShellExperienceHost: `taskkill /F /IM ShellExperienceHost.exe`
- App does this automatically when applying modes

**Display still turns off:**

- Verify you're selecting HIGH PERFORMANCE or ULTIMATE
- Other modes don't have timeout disabled
- Check Power Options in Windows Settings

**Logo icon not showing:**

- Verify .ico file is in correct folder
- Check icon property is uncommented in XAML
- Rebuild the project
- Icon appears in window title and taskbar after rebuild

---

## 📞 Support

For issues with:

- **Power plans**: Check Windows Power Options (Settings > System > Power)
- **Display**: Check Monitor's OSD settings
- **Modes**: Verify Windows version is 11+
- **Admin access**: Ensure app is run with administrator privileges

---

**Version:** 2.0 with Lite enhancements  
**Last Updated:** 2025  
**Status:** ✅ Ready to use
