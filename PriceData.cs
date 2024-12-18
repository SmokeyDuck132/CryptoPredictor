using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoPricePredictor
{
    public class PriceData
    {
        public List<DateTime> Timestamps { get; set; } = new List<DateTime>(); // Initialized
        public List<decimal> OpenPrices { get; set; } = new List<decimal>(); // Initialized
        public List<decimal> HighPrices { get; set; } = new List<decimal>(); // Initialized
        public List<decimal> LowPrices { get; set; } = new List<decimal>(); // Initialized
        public List<decimal> ClosePrices { get; set; } = new List<decimal>(); // Initialized
        public List<double> Volumes { get; set; } = new List<double>(); // Initialized
    }

    public static class DataExtractor
    {
        public static PriceData ExtractPriceData(List<KlineData> klineData)
        {
            return new PriceData
            {
                Timestamps = klineData.Select(k => DateTimeOffset.FromUnixTimeMilliseconds(k.OpenTime).UtcDateTime).ToList(),
                OpenPrices = klineData.Select(k => (decimal)k.Open).ToList(),
                HighPrices = klineData.Select(k => (decimal)k.High).ToList(),
                LowPrices = klineData.Select(k => (decimal)k.Low).ToList(),
                ClosePrices = klineData.Select(k => (decimal)k.Close).ToList(),
                Volumes = klineData.Select(k => k.Volume).ToList()
            };
        }
    }
}
