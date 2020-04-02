using System;

namespace Logic
{
    public class RealTemperatureProvider : ITemperatureProvider
    {
        /// <inheritdoc />
        public double GetTemperature()
        {
            // Use platform: https://codepedia.info/dotnet-core-to-detect-operating-system-os-platform/
            throw new NotImplementedException();
        }
    }
}