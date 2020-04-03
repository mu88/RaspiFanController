using System;

namespace RaspiFanController.Logic
{
    public class RaspiTemperatureProvider : ITemperatureProvider
    {
        /// <inheritdoc />
        public (double,string) GetTemperature()
        {
            // Use platform: https://codepedia.info/dotnet-core-to-detect-operating-system-os-platform/
            throw new NotImplementedException();
        }
    }
}