#!/bin/bash
# iPhone Firmware Manager Launcher

echo "Starting iPhone Firmware Manager..."
echo "Make sure your iPhone is connected!"

# Check if required tools are installed
if ! command -v idevicerestore &> /dev/null; then
    echo "Error: idevicerestore not found. Installing..."
    sudo apt update && sudo apt install -y libimobiledevice-utils
fi

if ! command -v lsusb &> /dev/null; then
    echo "Error: lsusb not found. Installing..."
    sudo apt update && sudo apt install -y usbutils
fi

# Launch the application
python3 /home/sonwabile/IPHONE/iphone_firmware_manager.py
