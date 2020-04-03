using System;

namespace RaspiFanController.Logic
{
    public class DevTemperatureProvider : ITemperatureProvider
    {
        /// <inheritdoc />
        public (double, string) GetTemperature()
        {
            var random = new Random();
            var nextDouble = random.NextDouble();
            return (30 + 30 * nextDouble, "C");
        }
    }
}