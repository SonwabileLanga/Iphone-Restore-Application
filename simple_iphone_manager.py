#!/usr/bin/env python3
"""
Simple iPhone Firmware Manager (Command Line)
A simple command-line interface for iPhone firmware management
"""

import subprocess
import sys
import os
import time
from pathlib import Path

class SimpleiPhoneManager:
    def __init__(self):
        self.device_connected = False
        self.device_mode = "Unknown"
        
    def check_device_status(self):
        """Check if iPhone is connected and its mode"""
        try:
            result = subprocess.run(['lsusb'], capture_output=True, text=True, timeout=10)
            apple_devices = [line for line in result.stdout.split('\n') if 'Apple' in line and 'Mobile Device' in line]
            
            if apple_devices:
                self.device_connected = True
                if 'Recovery Mode' in apple_devices[0]:
                    self.device_mode = "Recovery Mode"
                    print("📱 iPhone Status: Connected (Recovery Mode)")
                else:
                    self.device_mode = "Normal"
                    print("📱 iPhone Status: Connected (Normal Mode)")
            else:
                self.device_connected = False
                self.device_mode = "Not Connected"
                print("📱 iPhone Status: Not Connected")
                
        except Exception as e:
            print(f"❌ Error checking device: {str(e)}")
            self.device_connected = False
            
    def list_ipsw_files(self, directory="~/Downloads"):
        """List available IPSW files"""
        downloads_path = os.path.expanduser(directory)
        ipsw_files = list(Path(downloads_path).glob("*.ipsw"))
        
        if ipsw_files:
            print(f"\n📁 Found {len(ipsw_files)} IPSW file(s) in {downloads_path}:")
            for i, file in enumerate(ipsw_files, 1):
                size_mb = file.stat().st_size / (1024 * 1024)
                print(f"  {i}. {file.name} ({size_mb:.1f} MB)")
            return ipsw_files
        else:
            print(f"\n❌ No IPSW files found in {downloads_path}")
            return []
            
    def restore_iphone(self, ipsw_file, erase=True, exclude_baseband=True, debug=True):
        """Restore iPhone with given options"""
        if not self.device_connected:
            print("❌ No iPhone detected. Please connect your device.")
            return False
            
        if not os.path.exists(ipsw_file):
            print(f"❌ IPSW file not found: {ipsw_file}")
            return False
            
        print(f"\n🔄 Starting restore with {os.path.basename(ipsw_file)}...")
        print("⚠️  This will erase all data on your iPhone!")
        
        # Build command
        cmd = ['idevicerestore']
        if erase:
            cmd.append('-e')
        if exclude_baseband:
            cmd.append('-x')
        if debug:
            cmd.append('-d')
        cmd.append(ipsw_file)
        
        print(f"Command: {' '.join(cmd)}")
        
        try:
            # Run restore
            process = subprocess.Popen(cmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, 
                                     text=True, bufsize=1, universal_newlines=True)
            
            print("\n📋 Restore Output:")
            print("-" * 50)
            
            for line in iter(process.stdout.readline, ''):
                if line:
                    print(line.strip())
                    
            process.wait()
            
            if process.returncode == 0:
                print("\n✅ Restore completed successfully!")
                return True
            else:
                print(f"\n❌ Restore failed with exit code: {process.returncode}")
                return False
                
        except Exception as e:
            print(f"\n❌ Error during restore: {str(e)}")
            return False
            
    def force_restart_instructions(self):
        """Show force restart instructions"""
        print("\n🔄 Force Restart Instructions:")
        print("1. Hold Power + Home buttons together for 15-20 seconds")
        print("2. Release both buttons")
        print("3. Wait for Apple logo to appear")
        print("4. This should exit recovery mode if successful")
        
    def main_menu(self):
        """Main menu interface"""
        while True:
            print("\n" + "="*60)
            print("📱 iPhone Firmware Manager")
            print("="*60)
            
            self.check_device_status()
            
            print("\nOptions:")
            print("1. List available IPSW files")
            print("2. Restore iPhone (Full erase)")
            print("3. Restore iPhone (Keep data)")
            print("4. Show force restart instructions")
            print("5. Refresh device status")
            print("6. Exit")
            
            choice = input("\nEnter your choice (1-6): ").strip()
            
            if choice == '1':
                self.list_ipsw_files()
                
            elif choice == '2':
                ipsw_files = self.list_ipsw_files()
                if ipsw_files:
                    try:
                        file_num = int(input(f"\nSelect file (1-{len(ipsw_files)}): ")) - 1
                        if 0 <= file_num < len(ipsw_files):
                            self.restore_iphone(str(ipsw_files[file_num]), erase=True)
                        else:
                            print("❌ Invalid selection")
                    except ValueError:
                        print("❌ Please enter a valid number")
                        
            elif choice == '3':
                ipsw_files = self.list_ipsw_files()
                if ipsw_files:
                    try:
                        file_num = int(input(f"\nSelect file (1-{len(ipsw_files)}): ")) - 1
                        if 0 <= file_num < len(ipsw_files):
                            self.restore_iphone(str(ipsw_files[file_num]), erase=False)
                        else:
                            print("❌ Invalid selection")
                    except ValueError:
                        print("❌ Please enter a valid number")
                        
            elif choice == '4':
                self.force_restart_instructions()
                
            elif choice == '5':
                self.check_device_status()
                
            elif choice == '6':
                print("👋 Goodbye!")
                break
                
            else:
                print("❌ Invalid choice. Please try again.")

def main():
    print("🚀 Starting Simple iPhone Firmware Manager...")
    
    # Check if required tools are installed
    try:
        subprocess.run(['which', 'idevicerestore'], check=True, capture_output=True)
        subprocess.run(['which', 'lsusb'], check=True, capture_output=True)
    except subprocess.CalledProcessError:
        print("❌ Error: Required tools not found.")
        print("Please install: sudo apt install libimobiledevice-utils usbutils")
        sys.exit(1)
    
    manager = SimpleiPhoneManager()
    manager.main_menu()

if __name__ == "__main__":
    main()
