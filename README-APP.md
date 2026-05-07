# PowerHub - Digital Utility Application

## 🌟 Overview

PowerHub is a modern, feature-rich Windows system utility designed with the philosophy of **"The Crystalline Engine."** It combines elegant UI with powerful functionality to help users monitor, optimize, and customize their Windows systems.

## 📋 Features

### Dashboard

- **Real-time System Monitoring**: CPU usage, memory consumption, disk space
- **System Status**: Health, temperature, background apps, battery status
- **Quick Actions**: System optimization, cache clearing, updates, security scans
- **Recent Activity**: Track recent system operations

### Display Settings

- Resolution and refresh rate configuration
- Brightness and color temperature controls
- Eye care features (Night Light, HDR support)
- Advanced options (VSync, FreeSync/G-Sync)

### System Tweaks

- Performance optimization options
- Visual effects management
- Privacy & security configurations
- System maintenance scheduling

### Power Management

- Multiple power plans (Balanced, High Performance, Power Saver)
- Screen and sleep settings
- Battery optimization
- Lid and button action configuration

### Activity Log

- Comprehensive system operation history
- Event filtering and search
- Export functionality
- Detailed activity information

### About

- Application information and version details
- Feature overview
- Design philosophy explanation
- System requirements

## 🎨 Design System: "The Crystalline Engine"

The application follows a sophisticated design system built on:

### Color Palette

- **Primary**: #004f96 (Professional Blue)
- **Tertiary**: #833900 (Warm Accent)
- **Secondary**: #5e5e5e (Neutral Gray)
- **Surface Colors**: Custom tonal depth system

### Typography

- **Font Family**: Inter (Google Fonts)
- **Weights**: 300-900
- **Hierarchy**: Headline, Body, Label

### Components

- **Mica Effect**: Glass morphism with blur and saturation
- **Material Design 3**: Modern, accessible color tokens
- **Soft Minimalism**: Clean interfaces with tonal depth

## 📁 File Structure

```
PowerHub/
├── index.html          # Dashboard page
├── display.html        # Display settings
├── tweaks.html         # System tweaks
├── power.html          # Power management
├── activity.html       # Activity log
├── about.html          # About page
└── README-APP.md       # This file
```

## 🚀 Getting Started

1. Open `index.html` in a modern web browser
2. Navigate through the sidebar menu to access different features
3. Each page is fully functional with interactive controls

## 💻 Technology Stack

- **Frontend**: HTML5 (Semantic & Structured)
- **Styling**: Tailwind CSS v3 with custom Material Design 3 theme
- **Icons**: Material Symbols Outlined (Google)
- **Fonts**: Inter from Google Fonts (weights: 300-900)
- **Build Tool**: No build step required - works in any modern browser

### HTML Structure

```html
<!doctype html>
<html class="light" lang="en">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
  </head>
  <body class="flex">
    <!-- Content -->
  </body>
</html>
```

### Tailwind Configuration

Each page includes a custom Tailwind config with:

- **Color System**: Material Design 3 palette
- **Border Radius**: 0.25rem default with lg (0.5rem) and xl (0.75rem) variants
- **Font Family**: Inter for all text layers (headline, body, label)
- **Dark Mode**: Class-based with `dark:` utility support

### Key Color Tokens

```javascript
colors: {
  primary: "#004f96",
  "primary-container": "#0067c0",
  secondary: "#5e5e5e",
  tertiary: "#833900",
  surface: "#f9f9f9",
  outline: "#717783",
  "outline-variant": "#c1c6d4",
  error: "#ba1a1a",
  // ... more MD3 tokens
}
```

## 🎯 Design Features

### Responsive Sidebar Navigation

- **Width**: 280px fixed width with sticky positioning
- **Styling**: `bg-slate-100/80` with `backdrop-blur-xl` for transparency
- **Active States**: Blue highlight (#004f96/10) with left border indicator
- **Icons**: Material Symbols Outlined (24px, FILL 0, WGHT 400)
- **Hover Effects**: Smooth color transitions on all navigation items

### Glass Card Components

- **Base Class**: `glass-card` with rgba(255,255,255,0.6) background
- **Effect**: `backdrop-filter: blur(12px)` for frosted glass appearance
- **Borders**: `border border-outline-variant/30` for subtle depth
- **Spacing**: Consistent padding with rounded corners (0.5rem-0.75rem)
- **Variations**: Surface colors from darkest to brightest container variants

### Interactive Elements

- **Toggle Switches**: HTML checkboxes with `accent-blue-600` styling
- **Slider Controls**: Input range type with full-width responsive layout
- **Dropdowns**: Select elements with `border border-gray-300 rounded-lg` styling
- **Action Buttons**:
  - Primary: `bg-blue-600 text-white hover:bg-blue-700`
  - Secondary: `bg-gray-300 text-gray-900 hover:bg-gray-400`
  - Ghost: Hover state only with background transitions
- **Animations**: `smooth-transition` class with `transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1)`
- **Focus States**: Proper accessibility with visible focus indicators

## 🔧 Customization & Configuration

### Per-Page Tailwind Configuration

Each HTML file has a `<script id="tailwind-config">` block with:

```javascript
tailwind.config = {
  darkMode: "class",
  theme: {
    extend: {
      colors: {
        /* Material Design 3 palette */
      },
      borderRadius: {
        DEFAULT: "0.25rem",
        lg: "0.5rem",
        xl: "0.75rem",
        full: "9999px",
      },
      fontFamily: {
        headline: ["Inter"],
        body: ["Inter"],
        label: ["Inter"],
      },
    },
  },
};
```

### Global CSS Styles

Defined in `<style>` tags within each HTML file:

- **Material Symbols Configuration**: `font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24`
- **Mica Effect**: `backdrop-filter: blur(40px) saturate(180%)` for glass morphism
- **Glass Card**: Semi-transparent with backdrop blur for layered depth
- **Smooth Transitions**: Cubic-bezier timing functions for natural motion

## 📱 Browser Support

- Chrome/Edge 90+
- Firefox 88+
- Safari 14+
- Modern mobile browsers

## 🌙 Dark Mode

The application is built with dark mode support:

- Add `dark` class to `<html>` element
- All colors have dark mode variants
- Media queries support `prefers-color-scheme`

## 📝 Future Enhancements

- Dark mode toggle
- Settings persistence (localStorage)
- Real backend integration
- System notification support
- Advanced analytics dashboard

## 📄 License

PowerHub Digital Utility © 2024. All rights reserved.

## 🤝 Credits

- Design inspired by Windows 11's Fluent Design System
- Icons from Google Material Symbols
- Styling powered by Tailwind CSS
- Built with web standards

---

**PowerHub: Where functionality meets elegance.**
