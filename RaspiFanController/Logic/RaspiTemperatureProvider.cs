using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public class RaspiTemperatureProvider : ITemperatureProvider
{
    public RaspiTemperatureProvider(ILogger<RaspiTemperatureProvider> logger)
    {
        Logger = logger;
    }

    private ILogger<RaspiTemperatureProvider> Logger { get; }

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

        Logger.LogDebug($"Process exited with {process.ExitCode}, output '{standardOutput}'");

        var regex = new Regex(@"temp=(\d+[,.]\d).{1}(.{1})");
        var match = regex.Match(standardOutput);

        if (process.ExitCode != 0 || !match.Success)
        {
            Logger.LogDebug("No Regex match");
            return fallbackValue;
        }

        if (double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return (result, match.Groups[2].Value);
        }
        else
        {
            Logger.LogDebug($"Could not parse double from '{match.Groups[1].Value}'");
            return fallbackValue;
        }
    }

    /// <inheritdoc />
    public bool IsPlatformSupported()
    {
        var isPlatformSupported = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        Logger.LogDebug($"Is platform supported: {isPlatformSupported}");
        return isPlatformSupported;
    }
}