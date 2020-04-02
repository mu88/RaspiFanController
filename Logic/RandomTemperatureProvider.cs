using System;

namespace Logic
{
    public class RandomTemperatureProvider : ITemperatureProvider
    {
        /// <inheritdoc />
        public double GetTemperature()
        {
            var random = new Random();
            var nextDouble = random.NextDouble();
            return 30 + 30 * nextDouble;
        }
    }
}