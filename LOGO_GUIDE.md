# PowerHub Lite - Logo & Branding Guide

## Current Branding

The app now features:

- **Lightning bolt icon (⚡)** as the visual identifier
- **Modern blue accent color** (#0078D7 - Windows standard)
- **Clean, professional UI** with WPF styling

## Creating a Custom Logo/Icon

### Option 1: Online ICO Generator (Easiest)

1. Go to https://icoconvert.com/ or similar tool
2. Create or upload a PNG image (preferably square, 256x256 pixels)
3. Convert to ICO format
4. Save as `PowerHub.ico` in the `src/PowerHub.Lite/` directory
5. Uncomment the Icon property in `MainWindow.xaml`:
   ```xml
   Icon="pack://application:,,,/PowerHub.ico"
   ```
6. Add to project file if needed

### Option 2: Design Software

- Use Adobe Illustrator, Figma, or Inkscape
- Create a square design (256x256 minimum)
- Export as PNG
- Convert to ICO using an online tool

### Option 3: Use Existing Asset

- Copy any 32x32 or larger PNG icon
- Convert to ICO format
- Place in project directory

### Suggested Design Elements

- Lightning bolt (⚡) as main element
- Power button icon (⊙)
- Speedometer or gauge
- Modern, flat design style
- Windows Fluent Design principles

### App Manifest Icon

The app also uses an icon specified in `app.manifest`. You can update:

```xml
<asmv3:visual>
  <asmv3:windowsSettings xmlns="http://schemas.microsoft.com/solution/windowssettings/manifests/windowsSettings/ms-settings">
    <ms:dpiAware>true</ms:dpiAware>
  </asmv3:windowsSettings>
</asmv3:visual>
```

## Implementation

Once you have your ICO file:

1. Add to `src/PowerHub.Lite/` folder
2. Set Build Action to "Content"
3. Uncomment Icon binding in MainWindow.xaml
4. Rebuild the project

The icon will appear in:

- Window title bar
- Taskbar
- Alt+Tab switcher
- File explorer (for exe file)
