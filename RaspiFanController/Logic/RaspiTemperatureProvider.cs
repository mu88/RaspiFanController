using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace RaspiFanController.Logic
{
    public class RaspiTemperatureProvider : ITemperatureProvider
    {
        /// <inheritdoc />
        public (double, string) GetTemperature()
        {
            var fallbackValue = (double.NaN, "#");

            var process = new Process
                          {
                              StartInfo = new ProcessStartInfo
                                          {
                                              FileName = "vcgencmd",
                                              Arguments = "measure_temp",
                                              RedirectStandardOutput = true,
                                              UseShellExecute = false,
                                              CreateNoWindow = false,
                                          }
                          };
            process.Start();
            var standardOutput = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var regex = new Regex(@"temp=(\d+[,.]\d).{1}(.{1})");
            var match = regex.Match(standardOutput);

            if (process.ExitCode != 0 || !match.Success)
            {
                return fallbackValue;
            }

            if (double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return (result, match.Groups[2].Value);
            }
            else
            {
                return fallbackValue;
            }
        }

        /// <inheritdoc />
        public bool IsPlatformSupported()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }
    }
}