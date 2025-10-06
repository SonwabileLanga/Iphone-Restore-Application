using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AvaloniaiPhoneManager
{
    public partial class MainWindow : Window
    {
        private bool deviceConnected = false;
        private string deviceMode = "Unknown";
        private string ipswFile = "";
        private bool restoreInProgress = false;

        public MainWindow()
        {
            InitializeComponent();
            CheckDeviceStatus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void CheckDeviceStatus()
        {
            try
            {
                var result = await RunCommandAsync("lsusb");
                var appleDevices = result.Split('\n')
                    .Where(line => line.Contains("Apple") && line.Contains("Mobile Device"))
                    .ToList();

                if (appleDevices.Any())
                {
                    deviceConnected = true;
                    if (appleDevices[0].Contains("Recovery Mode"))
                    {
                        deviceMode = "Recovery Mode";
                        StatusText.Text = "Connected";
                        StatusText.Foreground = Avalonia.Media.Brushes.Green;
                        ModeText.Text = "Recovery Mode";
                        ModeText.Foreground = Avalonia.Media.Brushes.Red;
                    }
                    else
                    {
                        deviceMode = "Normal";
                        StatusText.Text = "Connected";
                        StatusText.Foreground = Avalonia.Media.Brushes.Green;
                        ModeText.Text = "Normal Mode";
                        ModeText.Foreground = Avalonia.Media.Brushes.Blue;
                    }
                }
                else
                {
                    deviceConnected = false;
                    deviceMode = "Not Connected";
                    StatusText.Text = "Not Connected";
                    StatusText.Foreground = Avalonia.Media.Brushes.Red;
                    ModeText.Text = "Unknown";
                    ModeText.Foreground = Avalonia.Media.Brushes.Gray;
                }

                LogMessage($"Device status: {deviceMode}");
            }
            catch (Exception ex)
            {
                LogMessage($"Error checking device: {ex.Message}");
                deviceConnected = false;
                StatusText.Text = "Error";
                StatusText.Foreground = Avalonia.Media.Brushes.Red;
            }
        }

        private void LogMessage(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            LogText.Text += $"[{timestamp}] {message}\n";
            LogScrollViewer.ScrollToEnd();
        }

        private async Task<string> RunCommandAsync(string command)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            return output;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => CheckDeviceStatus());
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select IPSW File",
                Filters = new[]
                {
                    new FileDialogFilter { Name = "IPSW files", Extensions = { "ipsw" } },
                    new FileDialogFilter { Name = "All files", Extensions = { "*" } }
                }
            };

            var result = dialog.ShowAsync(this).Result;
            if (result != null && result.Length > 0)
            {
                ipswFile = result[0];
                IpswTextBox.Text = Path.GetFileName(ipswFile);
                LogMessage($"Selected IPSW file: {Path.GetFileName(ipswFile)}");
            }
        }

        private async void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (!deviceConnected)
            {
                LogMessage("❌ No iPhone detected. Please connect your device.");
                return;
            }

            if (string.IsNullOrEmpty(ipswFile) || !File.Exists(ipswFile))
            {
                LogMessage("❌ Please select a valid IPSW file.");
                return;
            }

            if (restoreInProgress)
            {
                LogMessage("⚠️ Restore already in progress.");
                return;
            }

            restoreInProgress = true;
            RestoreButton.IsEnabled = false;
            LogMessage("Starting restore process...");

            await Task.Run(() => PerformRestore());
        }

        private async Task PerformRestore()
        {
            try
            {
                var cmd = new[] { "idevicerestore" }.ToList();
                
                if (EraseCheckBox.IsChecked == true)
                    cmd.Add("-e");
                if (ExcludeBasebandCheckBox.IsChecked == true)
                    cmd.Add("-x");
                if (DebugCheckBox.IsChecked == true)
                    cmd.Add("-d");
                
                cmd.Add(ipswFile);

                LogMessage($"Running command: {string.Join(" ", cmd)}");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "idevicerestore",
                        Arguments = string.Join(" ", cmd.Skip(1)),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Dispatcher.UIThread.Post(() => LogMessage(e.Data));
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Dispatcher.UIThread.Post(() => LogMessage(e.Data));
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    Dispatcher.UIThread.Post(() => LogMessage("✅ Restore completed successfully!"));
                }
                else
                {
                    Dispatcher.UIThread.Post(() => LogMessage($"❌ Restore failed with exit code: {process.ExitCode}"));
                }
            }
            catch (Exception ex)
            {
                Dispatcher.UIThread.Post(() => LogMessage($"❌ Error during restore: {ex.Message}"));
            }
            finally
            {
                Dispatcher.UIThread.Post(() =>
                {
                    restoreInProgress = false;
                    RestoreButton.IsEnabled = true;
                    CheckDeviceStatus();
                });
            }
        }

        private void ForceRestartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!deviceConnected)
            {
                LogMessage("❌ No iPhone detected.");
                return;
            }

            LogMessage("Attempting to force restart iPhone...");
            LogMessage("Instructions:");
            LogMessage("1. Hold Power + Home buttons together for 15-20 seconds");
            LogMessage("2. Release both buttons");
            LogMessage("3. Wait for Apple logo to appear");
            LogMessage("4. This will exit recovery mode if successful");
        }

        private async void ExitRecoveryButton_Click(object sender, RoutedEventArgs e)
        {
            if (!deviceConnected || deviceMode != "Recovery Mode")
            {
                LogMessage("⚠️ Device is not in recovery mode.");
                return;
            }

            LogMessage("Attempting to exit recovery mode...");
            await Task.Run(() => CheckDeviceStatus());
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            LogText.Text = "";
        }
    }
}
