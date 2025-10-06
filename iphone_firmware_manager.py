#!/usr/bin/env python3
"""
iPhone Firmware Manager
A GUI application for managing iPhone firmware and device recovery
"""

import tkinter as tk
from tkinter import ttk, filedialog, messagebox, scrolledtext
import subprocess
import threading
import os
import sys
import time
from pathlib import Path

class iPhoneFirmwareManager:
    def __init__(self, root):
        self.root = root
        self.root.title("iPhone Firmware Manager")
        self.root.geometry("800x600")
        self.root.configure(bg='#f0f0f0')
        
        # Variables
        self.device_connected = False
        self.device_mode = "Unknown"
        self.ipsw_file = ""
        self.restore_in_progress = False
        
        self.setup_ui()
        self.check_device_status()
        
    def setup_ui(self):
        # Main frame
        main_frame = ttk.Frame(self.root, padding="10")
        main_frame.grid(row=0, column=0, sticky=(tk.W, tk.E, tk.N, tk.S))
        
        # Configure grid weights
        self.root.columnconfigure(0, weight=1)
        self.root.rowconfigure(0, weight=1)
        main_frame.columnconfigure(1, weight=1)
        
        # Title
        title_label = ttk.Label(main_frame, text="iPhone Firmware Manager", 
                               font=('Arial', 16, 'bold'))
        title_label.grid(row=0, column=0, columnspan=3, pady=(0, 20))
        
        # Device Status Section
        status_frame = ttk.LabelFrame(main_frame, text="Device Status", padding="10")
        status_frame.grid(row=1, column=0, columnspan=3, sticky=(tk.W, tk.E), pady=(0, 10))
        status_frame.columnconfigure(1, weight=1)
        
        ttk.Label(status_frame, text="Connection:").grid(row=0, column=0, sticky=tk.W, padx=(0, 10))
        self.status_label = ttk.Label(status_frame, text="Checking...", foreground="orange")
        self.status_label.grid(row=0, column=1, sticky=tk.W)
        
        ttk.Label(status_frame, text="Mode:").grid(row=1, column=0, sticky=tk.W, padx=(0, 10))
        self.mode_label = ttk.Label(status_frame, text="Unknown", foreground="gray")
        self.mode_label.grid(row=1, column=1, sticky=tk.W)
        
        # Refresh button
        self.refresh_btn = ttk.Button(status_frame, text="Refresh Status", 
                                     command=self.check_device_status)
        self.refresh_btn.grid(row=0, column=2, rowspan=2, padx=(10, 0))
        
        # IPSW File Selection
        ipsw_frame = ttk.LabelFrame(main_frame, text="Firmware File (IPSW)", padding="10")
        ipsw_frame.grid(row=2, column=0, columnspan=3, sticky=(tk.W, tk.E), pady=(0, 10))
        ipsw_frame.columnconfigure(1, weight=1)
        
        ttk.Label(ipsw_frame, text="IPSW File:").grid(row=0, column=0, sticky=tk.W, padx=(0, 10))
        self.ipsw_entry = ttk.Entry(ipsw_frame, state="readonly")
        self.ipsw_entry.grid(row=0, column=1, sticky=(tk.W, tk.E), padx=(0, 10))
        
        ttk.Button(ipsw_frame, text="Browse", command=self.browse_ipsw).grid(row=0, column=2)
        
        # Restore Options
        options_frame = ttk.LabelFrame(main_frame, text="Restore Options", padding="10")
        options_frame.grid(row=3, column=0, columnspan=3, sticky=(tk.W, tk.E), pady=(0, 10))
        
        self.erase_var = tk.BooleanVar(value=True)
        ttk.Checkbutton(options_frame, text="Erase all data (Full restore)", 
                       variable=self.erase_var).grid(row=0, column=0, sticky=tk.W)
        
        self.exclude_baseband_var = tk.BooleanVar(value=True)
        ttk.Checkbutton(options_frame, text="Exclude baseband (Recommended for USB issues)", 
                       variable=self.exclude_baseband_var).grid(row=1, column=0, sticky=tk.W)
        
        self.debug_var = tk.BooleanVar(value=True)
        ttk.Checkbutton(options_frame, text="Debug mode (Verbose output)", 
                       variable=self.debug_var).grid(row=2, column=0, sticky=tk.W)
        
        # Action Buttons
        button_frame = ttk.Frame(main_frame)
        button_frame.grid(row=4, column=0, columnspan=3, pady=(0, 10))
        
        self.restore_btn = ttk.Button(button_frame, text="Start Restore", 
                                     command=self.start_restore, style="Accent.TButton")
        self.restore_btn.grid(row=0, column=0, padx=(0, 10))
        
        self.force_restart_btn = ttk.Button(button_frame, text="Force Restart Device", 
                                           command=self.force_restart)
        self.force_restart_btn.grid(row=0, column=1, padx=(0, 10))
        
        self.exit_recovery_btn = ttk.Button(button_frame, text="Exit Recovery Mode", 
                                           command=self.exit_recovery)
        self.exit_recovery_btn.grid(row=0, column=2)
        
        # Progress Bar
        self.progress = ttk.Progressbar(main_frame, mode='indeterminate')
        self.progress.grid(row=5, column=0, columnspan=3, sticky=(tk.W, tk.E), pady=(0, 10))
        
        # Log Output
        log_frame = ttk.LabelFrame(main_frame, text="Output Log", padding="10")
        log_frame.grid(row=6, column=0, columnspan=3, sticky=(tk.W, tk.E, tk.N, tk.S), pady=(0, 10))
        log_frame.columnconfigure(0, weight=1)
        log_frame.rowconfigure(0, weight=1)
        main_frame.rowconfigure(6, weight=1)
        
        self.log_text = scrolledtext.ScrolledText(log_frame, height=15, wrap=tk.WORD)
        self.log_text.grid(row=0, column=0, sticky=(tk.W, tk.E, tk.N, tk.S))
        
        # Clear log button
        ttk.Button(log_frame, text="Clear Log", command=self.clear_log).grid(row=1, column=0, pady=(5, 0))
        
    def log_message(self, message):
        """Add message to log with timestamp"""
        timestamp = time.strftime("%H:%M:%S")
        self.log_text.insert(tk.END, f"[{timestamp}] {message}\n")
        self.log_text.see(tk.END)
        self.root.update_idletasks()
        
    def clear_log(self):
        """Clear the log output"""
        self.log_text.delete(1.0, tk.END)
        
    def check_device_status(self):
        """Check if iPhone is connected and its mode"""
        def check_thread():
            try:
                # Check USB devices
                result = subprocess.run(['lsusb'], capture_output=True, text=True, timeout=10)
                apple_devices = [line for line in result.stdout.split('\n') if 'Apple' in line and 'Mobile Device' in line]
                
                if apple_devices:
                    self.device_connected = True
                    if 'Recovery Mode' in apple_devices[0]:
                        self.device_mode = "Recovery Mode"
                        self.status_label.config(text="Connected", foreground="green")
                        self.mode_label.config(text="Recovery Mode", foreground="red")
                    else:
                        self.device_mode = "Normal"
                        self.status_label.config(text="Connected", foreground="green")
                        self.mode_label.config(text="Normal Mode", foreground="blue")
                else:
                    self.device_connected = False
                    self.device_mode = "Not Connected"
                    self.status_label.config(text="Not Connected", foreground="red")
                    self.mode_label.config(text="Unknown", foreground="gray")
                    
                self.log_message(f"Device status: {self.device_mode}")
                
            except Exception as e:
                self.log_message(f"Error checking device: {str(e)}")
                self.device_connected = False
                self.status_label.config(text="Error", foreground="red")
        
        threading.Thread(target=check_thread, daemon=True).start()
        
    def browse_ipsw(self):
        """Browse for IPSW file"""
        file_path = filedialog.askopenfilename(
            title="Select IPSW File",
            filetypes=[("IPSW files", "*.ipsw"), ("All files", "*.*")]
        )
        if file_path:
            self.ipsw_file = file_path
            self.ipsw_entry.config(state="normal")
            self.ipsw_entry.delete(0, tk.END)
            self.ipsw_entry.insert(0, file_path)
            self.ipsw_entry.config(state="readonly")
            self.log_message(f"Selected IPSW file: {os.path.basename(file_path)}")
            
    def start_restore(self):
        """Start the restore process"""
        if not self.device_connected:
            messagebox.showerror("Error", "No iPhone detected. Please connect your device.")
            return
            
        if not self.ipsw_file or not os.path.exists(self.ipsw_file):
            messagebox.showerror("Error", "Please select a valid IPSW file.")
            return
            
        if self.restore_in_progress:
            messagebox.showwarning("Warning", "Restore already in progress.")
            return
            
        # Confirm restore
        if messagebox.askyesno("Confirm Restore", 
                              "This will erase all data on your iPhone. Continue?"):
            self.restore_in_progress = True
            self.restore_btn.config(state="disabled")
            self.progress.start()
            self.log_message("Starting restore process...")
            
            # Start restore in separate thread
            threading.Thread(target=self.perform_restore, daemon=True).start()
            
    def perform_restore(self):
        """Perform the actual restore"""
        try:
            # Build idevicerestore command
            cmd = ['idevicerestore']
            
            if self.erase_var.get():
                cmd.append('-e')
                
            if self.exclude_baseband_var.get():
                cmd.append('-x')
                
            if self.debug_var.get():
                cmd.append('-d')
                
            cmd.append(self.ipsw_file)
            
            self.log_message(f"Running command: {' '.join(cmd)}")
            
            # Run the restore command
            process = subprocess.Popen(cmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, 
                                     text=True, bufsize=1, universal_newlines=True)
            
            # Read output in real-time
            for line in iter(process.stdout.readline, ''):
                if line:
                    self.log_message(line.strip())
                    
            process.wait()
            
            if process.returncode == 0:
                self.log_message("✅ Restore completed successfully!")
                messagebox.showinfo("Success", "iPhone restore completed successfully!")
            else:
                self.log_message(f"❌ Restore failed with exit code: {process.returncode}")
                messagebox.showerror("Error", "Restore failed. Check the log for details.")
                
        except Exception as e:
            self.log_message(f"❌ Error during restore: {str(e)}")
            messagebox.showerror("Error", f"Restore failed: {str(e)}")
            
        finally:
            self.restore_in_progress = False
            self.restore_btn.config(state="normal")
            self.progress.stop()
            self.check_device_status()
            
    def force_restart(self):
        """Force restart the iPhone"""
        if not self.device_connected:
            messagebox.showerror("Error", "No iPhone detected.")
            return
            
        self.log_message("Attempting to force restart iPhone...")
        messagebox.showinfo("Instructions", 
                           "To force restart your iPhone:\n\n"
                           "1. Hold Power + Home buttons together for 15-20 seconds\n"
                           "2. Release both buttons\n"
                           "3. Wait for Apple logo to appear\n\n"
                           "This will exit recovery mode if successful.")
        
    def exit_recovery(self):
        """Try to exit recovery mode"""
        if not self.device_connected or self.device_mode != "Recovery Mode":
            messagebox.showwarning("Warning", "Device is not in recovery mode.")
            return
            
        self.log_message("Attempting to exit recovery mode...")
        
        def exit_thread():
            try:
                # Try to send exit recovery command
                cmd = ['idevice_id', '-l']
                result = subprocess.run(cmd, capture_output=True, text=True, timeout=10)
                
                if result.returncode == 0:
                    self.log_message("Device detected, attempting to exit recovery...")
                    # Try to force restart
                    time.sleep(2)
                    self.check_device_status()
                else:
                    self.log_message("Could not communicate with device")
                    
            except Exception as e:
                self.log_message(f"Error: {str(e)}")
                
        threading.Thread(target=exit_thread, daemon=True).start()

def main():
    # Check if required tools are installed
    try:
        subprocess.run(['which', 'idevicerestore'], check=True, capture_output=True)
        subprocess.run(['which', 'lsusb'], check=True, capture_output=True)
    except subprocess.CalledProcessError:
        print("Error: Required tools not found. Please install:")
        print("sudo apt install libimobiledevice-utils")
        sys.exit(1)
    
    root = tk.Tk()
    app = iPhoneFirmwareManager(root)
    
    # Configure style
    style = ttk.Style()
    style.theme_use('clam')
    
    # Configure accent button style
    style.configure('Accent.TButton', foreground='white', background='#0078d4')
    
    root.mainloop()

if __name__ == "__main__":
    main()
