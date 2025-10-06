using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace iPhoneManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("📱 C# iPhone Firmware Manager");
            Console.WriteLine("================================");
            
            var manager = new iPhoneManager();
            await manager.RunAsync();
        }
    }

    public class iPhoneManager
    {
        private bool deviceConnected = false;
        private string deviceMode = "Unknown";
        private string ipswFile = "";

        public async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("📱 iPhone Firmware Manager");
                Console.WriteLine(new string('=', 50));
                
                await CheckDeviceStatusAsync();
                
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. List available IPSW files");
                Console.WriteLine("2. Restore iPhone (Full erase)");
                Console.WriteLine("3. Restore iPhone (Keep data)");
                Console.WriteLine("4. Show force restart instructions");
                Console.WriteLine("5. Refresh device status");
                Console.WriteLine("6. Exit");
                
                Console.Write("\nEnter your choice (1-6): ");
                var choice = Console.ReadLine()?.Trim();
                
                switch (choice)
                {
                    case "1":
                        await ListIpswFilesAsync();
                        break;
                    case "2":
                        await RestoreiPhoneAsync(true);
                        break;
                    case "3":
                        await RestoreiPhoneAsync(false);
                        break;
                    case "4":
                        ShowForceRestartInstructions();
                        break;
                    case "5":
                        await CheckDeviceStatusAsync();
                        break;
                    case "6":
                        Console.WriteLine("👋 Goodbye!");
                        return;
                    default:
                        Console.WriteLine("❌ Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private async Task CheckDeviceStatusAsync()
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
                        Console.WriteLine("📱 iPhone Status: Connected (Recovery Mode)");
                    }
                    else
                    {
                        deviceMode = "Normal";
                        Console.WriteLine("📱 iPhone Status: Connected (Normal Mode)");
                    }
                }
                else
                {
                    deviceConnected = false;
                    deviceMode = "Not Connected";
                    Console.WriteLine("📱 iPhone Status: Not Connected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error checking device: {ex.Message}");
                deviceConnected = false;
            }
        }

        private async Task ListIpswFilesAsync()
        {
            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var ipswFiles = Directory.GetFiles(downloadsPath, "*.ipsw");

            if (ipswFiles.Any())
            {
                Console.WriteLine($"\n📁 Found {ipswFiles.Length} IPSW file(s) in {downloadsPath}:");
                for (int i = 0; i < ipswFiles.Length; i++)
                {
                    var fileInfo = new FileInfo(ipswFiles[i]);
                    var sizeMB = fileInfo.Length / (1024.0 * 1024.0);
                    Console.WriteLine($"  {i + 1}. {Path.GetFileName(ipswFiles[i])} ({sizeMB:F1} MB)");
                }
            }
            else
            {
                Console.WriteLine($"\n❌ No IPSW files found in {downloadsPath}");
            }
        }

        private async Task RestoreiPhoneAsync(bool erase)
        {
            if (!deviceConnected)
            {
                Console.WriteLine("❌ No iPhone detected. Please connect your device.");
                return;
            }

            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var ipswFiles = Directory.GetFiles(downloadsPath, "*.ipsw");

            if (!ipswFiles.Any())
            {
                Console.WriteLine("❌ No IPSW files found in Downloads folder.");
                return;
            }

            Console.WriteLine("\nAvailable IPSW files:");
            for (int i = 0; i < ipswFiles.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {Path.GetFileName(ipswFiles[i])}");
            }

            Console.Write($"\nSelect file (1-{ipswFiles.Length}): ");
            if (int.TryParse(Console.ReadLine(), out int fileIndex) && fileIndex >= 1 && fileIndex <= ipswFiles.Length)
            {
                var selectedFile = ipswFiles[fileIndex - 1];
                await PerformRestoreAsync(selectedFile, erase);
            }
            else
            {
                Console.WriteLine("❌ Invalid selection.");
            }
        }

        private async Task PerformRestoreAsync(string ipswFile, bool erase)
        {
            Console.WriteLine($"\n🔄 Starting restore with {Path.GetFileName(ipswFile)}...");
            Console.WriteLine("⚠️  This will erase all data on your iPhone!");

            var cmd = new List<string> { "idevicerestore" };
            if (erase) cmd.Add("-e");
            cmd.Add("-x"); // Exclude baseband (recommended for USB issues)
            cmd.Add("-d"); // Debug mode
            cmd.Add(ipswFile);

            Console.WriteLine($"Command: {string.Join(" ", cmd)}");

            try
            {
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

                Console.WriteLine("\n📋 Restore Output:");
                Console.WriteLine(new string('-', 50));

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Console.WriteLine(e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Console.WriteLine(e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine("\n✅ Restore completed successfully!");
                }
                else
                {
                    Console.WriteLine($"\n❌ Restore failed with exit code: {process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error during restore: {ex.Message}");
            }
        }

        private void ShowForceRestartInstructions()
        {
            Console.WriteLine("\n🔄 Force Restart Instructions:");
            Console.WriteLine("1. Hold Power + Home buttons together for 15-20 seconds");
            Console.WriteLine("2. Release both buttons");
            Console.WriteLine("3. Wait for Apple logo to appear");
            Console.WriteLine("4. This should exit recovery mode if successful");
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
    }
}
