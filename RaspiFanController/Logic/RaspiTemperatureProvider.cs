using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public partial class RaspiTemperatureProvider(ILogger<RaspiTemperatureProvider> logger) : ITemperatureProvider
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

        logger.LogDebug("Process exited with {ProcessExitCode}, output '{StandardOutput}'", process.ExitCode, standardOutput);

        var match = TemperatureRegex().Match(standardOutput);

        if (process.ExitCode != 0 || !match.Success)
        {
            logger.LogDebug("No Regex match");
            return fallbackValue;
        }

        if (double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return (result, match.Groups[2].Value);
        }

        logger.LogDebug("Could not parse double from '{Value}'", match.Groups[1].Value);
        return fallbackValue;
    }

    /// <inheritdoc />
    public bool IsPlatformSupported()
    {
        var isPlatformSupported = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        logger.LogDebug("Is platform supported: {IsPlatformSupported}", isPlatformSupported);
        return isPlatformSupported;
    }
    
    [GeneratedRegex(@"temp=(\d+[,.]\d).{1}(.{1})", RegexOptions.IgnoreCase)]
    private static partial Regex TemperatureRegex();
}