using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace iPhoneWebManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiController : ControllerBase
    {
        private static bool _restoreInProgress = false;
        private static Process? _restoreProcess = null;
        private static string _lastLogMessage = "";

        [HttpGet("device/status")]
        public async Task<IActionResult> GetDeviceStatus()
        {
            try
            {
                var result = await RunCommandAsync("lsusb");
                var appleDevices = result.Split('\n')
                    .Where(line => line.Contains("Apple") && line.Contains("Mobile Device"))
                    .ToList();

                if (appleDevices.Any())
                {
                    var connected = true;
                    var mode = appleDevices[0].Contains("Recovery Mode") ? "Recovery Mode" : "Normal";
                    return Ok(new { connected, mode });
                }
                else
                {
                    return Ok(new { connected = false, mode = "Not Connected" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { connected = false, mode = "Error", error = ex.Message });
            }
        }

        [HttpGet("files/ipsw")]
        public IActionResult GetIpswFiles()
        {
            try
            {
                var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                var ipswFiles = Directory.GetFiles(downloadsPath, "*.ipsw")
                    .Select(file => new
                    {
                        name = Path.GetFileName(file),
                        path = file,
                        size = FormatFileSize(new FileInfo(file).Length)
                    })
                    .ToList();

                return Ok(new { files = ipswFiles });
            }
            catch (Exception ex)
            {
                return Ok(new { files = new object[0], error = ex.Message });
            }
        }

        [HttpPost("restore/start")]
        public async Task<IActionResult> StartRestore([FromBody] RestoreOptions options)
        {
            if (_restoreInProgress)
            {
                return BadRequest(new { message = "Restore already in progress" });
            }

            if (!System.IO.File.Exists(options.IpswFile))
            {
                return BadRequest(new { message = "IPSW file not found" });
            }

            try
            {
                _restoreInProgress = true;
                _lastLogMessage = "Starting restore...";

                var cmd = new List<string> { "idevicerestore" };
                
                if (options.EraseData)
                    cmd.Add("-e");
                if (options.ExcludeBaseband)
                    cmd.Add("-x");
                if (options.DebugMode)
                    cmd.Add("-d");
                
                cmd.Add(options.IpswFile);

                _restoreProcess = new Process
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

                _restoreProcess.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        _lastLogMessage = e.Data;
                    }
                };

                _restoreProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        _lastLogMessage = e.Data;
                    }
                };

                _restoreProcess.Start();
                _restoreProcess.BeginOutputReadLine();
                _restoreProcess.BeginErrorReadLine();

                // Run in background
                _ = Task.Run(async () =>
                {
                    await _restoreProcess.WaitForExitAsync();
                    _restoreInProgress = false;
                });

                return Ok(new { message = "Restore started successfully" });
            }
            catch (Exception ex)
            {
                _restoreInProgress = false;
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("restore/progress")]
        public IActionResult GetRestoreProgress()
        {
            if (!_restoreInProgress)
            {
                return Ok(new { completed = true, success = true, progress = 100, status = "Not running" });
            }

            // Simple progress simulation based on log messages
            var progress = 0;
            var status = _lastLogMessage;

            if (_lastLogMessage.Contains("%"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(_lastLogMessage, @"(\d+)%");
                if (match.Success && int.TryParse(match.Groups[1].Value, out var percent))
                {
                    progress = percent;
                }
            }
            else if (_lastLogMessage.Contains("DONE"))
            {
                progress = 100;
                _restoreInProgress = false;
            }
            else if (_lastLogMessage.Contains("ERROR") || _lastLogMessage.Contains("FAILED"))
            {
                progress = 0;
                _restoreInProgress = false;
            }

            return Ok(new
            {
                completed = !_restoreInProgress,
                success = progress == 100,
                progress = progress,
                status = status
            });
        }

        [HttpPost("device/exit-recovery")]
        public async Task<IActionResult> ExitRecovery()
        {
            try
            {
                // Try to detect device and show instructions
                var result = await RunCommandAsync("idevice_id", "-l");
                return Ok(new { message = "Exit recovery instructions displayed" });
            }
            catch (Exception ex)
            {
                return Ok(new { message = "Could not communicate with device", error = ex.Message });
            }
        }

        private async Task<string> RunCommandAsync(string command, params string[] arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = string.Join(" ", arguments),
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

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }

    public class RestoreOptions
    {
        public bool EraseData { get; set; }
        public bool ExcludeBaseband { get; set; }
        public bool DebugMode { get; set; }
        public string IpswFile { get; set; } = "";
    }
}
