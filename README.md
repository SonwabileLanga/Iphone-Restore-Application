# iPhone Restore Application Suite

A comprehensive collection of iPhone firmware management and device recovery applications built with multiple technologies.

## 🚀 Applications Included

### 1. Python GUI Application
- **File**: `iphone_firmware_manager.py`
- **Type**: Tkinter-based GUI
- **Features**: Modern graphical interface with real-time device monitoring
- **Run**: `python3 iphone_firmware_manager.py`

### 2. Python Console Application
- **File**: `simple_iphone_manager.py`
- **Type**: Command-line interface
- **Features**: Interactive menu system for device management
- **Run**: `python3 simple_iphone_manager.py`

### 3. C# Console Application
- **Directory**: `ConsoleApp/`
- **Type**: .NET 8.0 Console Application
- **Features**: High-performance device management with async operations
- **Run**: `cd ConsoleApp && dotnet run`

### 4. C# Web Application
- **Directory**: `iPhoneWebManager/`
- **Type**: ASP.NET Core Web Application
- **Features**: Modern web interface accessible via browser
- **Run**: `cd iPhoneWebManager && dotnet run`
- **Access**: http://localhost:5000

### 5. C# Desktop Application (Avalonia)
- **Directory**: `GuiApp/`
- **Type**: Cross-platform desktop GUI
- **Features**: Modern desktop application with Avalonia UI
- **Run**: `cd GuiApp && dotnet run`

## 📸 Screenshots & Interface Previews

### 🌐 Web Application Interface
The modern web interface provides the most user-friendly experience:

```
┌─────────────────────────────────────────────────────────────────┐
│                    📱 iPhone Firmware Manager                   │
│              Professional iPhone Management Tool                │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────┐ ┌─────────────────────────────────┐
│         Device Status           │ │       Firmware Files           │
│ ┌─────────────────────────────┐ │ │ ┌─────────────────────────────┐ │
│ │ Connection: ✅ Connected    │ │ │ │ Select IPSW File:           │ │
│ │ Mode: 🔴 Recovery Mode      │ │ │ │ [iPhone_4.7_15.8.5...▼]    │ │
│ │ [🔄 Refresh Status]         │ │ │ │ [📁 Refresh Files]          │ │
│ └─────────────────────────────┘ │ │ └─────────────────────────────┘ │
└─────────────────────────────────┘ └─────────────────────────────────┘

┌─────────────────────────────────┐ ┌─────────────────────────────────┐
│        Restore Options          │ │           Actions               │
│ ┌─────────────────────────────┐ │ │ ┌─────────────────────────────┐ │
│ │ ☑️ Erase all data           │ │ │ │ [🔴 Start Restore]          │ │
│ │ ☑️ Exclude baseband         │ │ │ │ [⚠️ Force Restart]          │ │
│ │ ☑️ Debug mode               │ │ │ │ [ℹ️ Exit Recovery]          │ │
│ └─────────────────────────────┘ │ │ └─────────────────────────────┘ │
└─────────────────────────────────┘ └─────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                        Output Log                              │
│ ┌─────────────────────────────────────────────────────────────┐ │
│ │ [12:34:56] [System] iPhone Firmware Manager initialized    │ │
│ │ [12:34:57] [Info] Device status: Recovery Mode             │ │
│ │ [12:34:58] [Success] Found 1 IPSW file(s)                 │ │
│ └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### 🖥️ Python GUI Application
The Tkinter-based desktop application:

```
┌─────────────────────────────────────────────────────────────────┐
│                    iPhone Firmware Manager                     │
├─────────────────────────────────────────────────────────────────┤
│ Device Status: [Connected ✅] Mode: [Recovery Mode 🔴]         │
│                                                                 │
│ IPSW File: [Browse...] [iPhone_4.7_15.8.5_19H394_Restore.ipsw]│
│                                                                 │
│ Options:                                                        │
│ ☑️ Erase all data (Full restore)                              │
│ ☑️ Exclude baseband (Recommended for USB issues)              │
│ ☑️ Debug mode (Verbose output)                                │
│                                                                 │
│ [🔴 Start Restore] [⚠️ Force Restart] [ℹ️ Exit Recovery]      │
│                                                                 │
│ ┌─────────────────────────────────────────────────────────────┐ │
│ │ Output Log:                                                │ │
│ │ [12:34:56] System: iPhone Firmware Manager initialized    │ │
│ │ [12:34:57] Info: Device detected in Recovery Mode         │ │
│ │ [12:34:58] Success: IPSW file loaded successfully         │ │
│ └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### 💻 Console Applications
Both Python and C# console applications provide:

```
==================================================
📱 iPhone Firmware Manager
==================================================
📱 iPhone Status: Connected (Recovery Mode)

Options:
1. List available IPSW files
2. Restore iPhone (Full erase)
3. Restore iPhone (Keep data)
4. Show force restart instructions
5. Refresh device status
6. Exit

Enter your choice (1-6): _
```

### 🖼️ Desktop Application (Avalonia)
The cross-platform desktop GUI:

```
┌─────────────────────────────────────────────────────────────────┐
│                    iPhone Firmware Manager                     │
├─────────────────────────────────────────────────────────────────┤
│ ┌─────────────────┐ ┌─────────────────┐ ┌─────────────────────┐ │
│ │   Device Info   │ │  Firmware File  │ │   Restore Options   │ │
│ │                 │ │                 │ │                     │ │
│ │ Status: ✅      │ │ [Browse...]     │ │ ☑️ Erase Data      │ │
│ │ Mode: Recovery  │ │ [Select File]   │ │ ☑️ Exclude Baseband │ │
│ │                 │ │                 │ │ ☑️ Debug Mode      │ │
│ └─────────────────┘ └─────────────────┘ └─────────────────────┘ │
│                                                                 │
│ ┌─────────────────────────────────────────────────────────────┐ │
│ │                    Action Buttons                          │ │
│ │ [🔴 Start Restore] [⚠️ Force Restart] [ℹ️ Exit Recovery]  │ │
│ └─────────────────────────────────────────────────────────────┘ │
│                                                                 │
│ ┌─────────────────────────────────────────────────────────────┐ │
│ │                      Progress Log                          │ │
│ │ [12:34:56] System: Application started                    │ │
│ │ [12:34:57] Info: iPhone detected in Recovery Mode         │ │
│ │ [12:34:58] Success: Ready to restore                      │ │
│ └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

## 📱 Features

- **Device Detection**: Automatic iPhone detection and status monitoring
- **Recovery Mode Support**: Handle devices stuck in recovery mode
- **Firmware Management**: Restore iPhone with custom IPSW files
- **Real-time Logging**: Monitor restore progress with detailed output
- **Multiple Interfaces**: GUI, Console, Web, and Desktop applications
- **Cross-platform**: Works on Windows, Linux, and macOS

## 🛠️ Prerequisites

### System Requirements
```bash
# Ubuntu/Debian
sudo apt update
sudo apt install -y libimobiledevice-utils usbutils python3-tk dotnet-sdk-8.0

# macOS
brew install libimobiledevice
brew install --cask dotnet-sdk

# Windows
# Install .NET 8.0 SDK and libimobiledevice
```

### Python Dependencies
```bash
# No additional packages required - uses standard library
python3 -m pip install --upgrade pip
```

### .NET Dependencies
```bash
# Restore packages for C# applications
cd ConsoleApp && dotnet restore
cd ../iPhoneWebManager && dotnet restore
cd ../GuiApp && dotnet restore
```

## 🚀 Quick Start

### 1. Python Applications
```bash
# GUI Application
python3 iphone_firmware_manager.py

# Console Application
python3 simple_iphone_manager.py
```

### 2. C# Applications
```bash
# Console Application
cd ConsoleApp
dotnet run

# Web Application
cd iPhoneWebManager
dotnet run
# Open browser to http://localhost:5000

# Desktop Application
cd GuiApp
dotnet run
```

## 📋 Usage Instructions

### Device Detection
All applications automatically detect connected iPhones and show their status:
- **Connected (Normal Mode)**: Device is working normally
- **Connected (Recovery Mode)**: Device is in recovery mode
- **Not Connected**: No iPhone detected

### Restore Process
1. **Connect your iPhone** via USB cable
2. **Select your IPSW file** from Downloads folder
3. **Choose restore options**:
   - Erase all data (Full restore)
   - Exclude baseband (Recommended for USB issues)
   - Debug mode (Verbose output)
4. **Start the restore** and monitor progress
5. **Wait for completion** and device restart

### Troubleshooting

#### Common Issues
- **"No iPhone detected"**: Check USB connection and cable
- **"Required tools not found"**: Install libimobiledevice-utils
- **"Tkinter not found"**: Install python3-tk
- **Restore fails at 37% or 92%**: Use "Exclude baseband" option

#### Recovery Mode Issues
If your iPhone is stuck in recovery mode:
1. **Force Restart**: Hold Power + Home buttons for 15-20 seconds
2. **Use the Applications**: Select IPSW file and start restore
3. **Try Different USB**: Use original Apple Lightning cable

## 🌐 Web Interface Features

The ASP.NET Core web application provides the most comprehensive interface:

### ✨ Key Features
- **Real-time Device Monitoring**: Live status updates every second
- **Responsive Design**: Works on desktop, tablet, and mobile devices
- **Modern UI**: Bootstrap 5 with Font Awesome icons
- **Progress Tracking**: Real-time restore progress with animated progress bars
- **Live Logging**: Real-time output display with color-coded messages
- **RESTful API**: Clean API endpoints for all operations
- **Cross-platform**: Access from any device with a web browser

### 🔧 Web API Endpoints
- `GET /api/device/status` - Get device connection status
- `GET /api/files/ipsw` - List available IPSW files
- `POST /api/restore/start` - Start restore process
- `GET /api/restore/progress` - Get restore progress
- `POST /api/device/exit-recovery` - Exit recovery mode

### 🎨 Interface Highlights
- **Device Status Card**: Shows connection status and mode with color-coded badges
- **Firmware Files Card**: Dropdown selection with file size information
- **Restore Options Card**: Checkbox options for restore configuration
- **Action Buttons**: Large, clearly labeled action buttons
- **Progress Section**: Animated progress bar with status text
- **Live Log Output**: Terminal-style output with timestamps and color coding

## 🏗️ Project Structure

```
iPhone-Restore-Application/
├── README.md                           # This file
├── screenshots/                        # Interface screenshots
├── iphone_firmware_manager.py         # Python GUI application
├── simple_iphone_manager.py           # Python console application
├── csharp_iphone_manager.cs           # C# console source
├── iPhoneManager.csproj               # C# console project
├── ConsoleApp/                        # C# console application
│   ├── Program.cs
│   └── iPhoneManager.csproj
├── iPhoneWebManager/                  # ASP.NET Core web application
│   ├── Controllers/
│   │   └── ApiController.cs
│   ├── Pages/
│   │   ├── Index.cshtml
│   │   └── Shared/
│   │       └── _Layout.cshtml
│   ├── wwwroot/                       # Web assets
│   │   ├── css/
│   │   ├── js/
│   │   └── lib/                       # Bootstrap, jQuery
│   └── iPhoneWebManager.csproj
├── GuiApp/                            # Avalonia desktop application
│   ├── AvaloniaMainWindow.axaml
│   ├── AvaloniaMainWindow.axaml.cs
│   └── AvaloniaiPhoneManager.csproj
├── launch_manager.sh                  # Python GUI launcher
├── iPhoneFirmwareManager.desktop      # Desktop shortcut
├── requirements.txt                   # Python dependencies
└── CSharp_Development_Guide.md        # C# development guide
```

## 🔧 Development

### Python Development
```bash
# No additional setup required
python3 iphone_firmware_manager.py
```

### C# Development
```bash
# Install .NET SDK
sudo apt install dotnet-sdk-8.0

# Build applications
dotnet build ConsoleApp/iPhoneManager.csproj
dotnet build iPhoneWebManager/iPhoneWebManager.csproj
dotnet build GuiApp/AvaloniaiPhoneManager.csproj

# Run applications
dotnet run --project ConsoleApp/iPhoneManager.csproj
dotnet run --project iPhoneWebManager/iPhoneWebManager.csproj
dotnet run --project GuiApp/AvaloniaiPhoneManager.csproj
```

## 📚 API Documentation

### Web Application API Endpoints

- `GET /api/device/status` - Get device connection status
- `GET /api/files/ipsw` - List available IPSW files
- `POST /api/restore/start` - Start restore process
- `GET /api/restore/progress` - Get restore progress
- `POST /api/device/exit-recovery` - Exit recovery mode

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is open source and available under the MIT License.

## 🆘 Support

For issues and questions:
1. Check the troubleshooting section
2. Enable debug mode for detailed logs
3. Try different USB cables/ports
4. Ensure all prerequisites are installed

## 🎯 Supported Devices

- iPhone 6s and newer
- All iOS versions supported by idevicerestore
- Recovery mode and normal mode
- Cross-platform compatibility

---

**Created by**: Sonwabile Langa  
**Repository**: [https://github.com/SonwabileLanga/Iphone-Restore-Application.git](https://github.com/SonwabileLanga/Iphone-Restore-Application.git)  
**Last Updated**: 2024