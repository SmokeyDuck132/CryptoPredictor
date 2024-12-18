using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoPricePredictor
{
    public class PricePredictor
    {
        private List<CryptoPriceData> historicalData;

        public PricePredictor(List<CryptoPriceData> data)
        {
            historicalData = data;
        }

        public string GeneratePrediction()
        {
            var latest = historicalData.LastOrDefault();
            if (latest == null)
                return "No data available to generate predictions.";

            return $"Predictions based on recent data:\n" +
                   $"1 Day: {PredictNextPrice(latest.Close, 1):F4}\n" +
                   $"5 Days: {PredictNextPrice(latest.Close, 5):F4}\n" +
                   $"15 Days: {PredictNextPrice(latest.Close, 15):F4}\n" +
                   $"3 Months: {PredictNextPrice(latest.Close, 90):F4}";
        }

        private decimal PredictNextPrice(decimal currentClose, int daysAhead)
        {
            // Simple logic to predict future price (Replace with ML model or advanced logic)
            return currentClose * (1 + (daysAhead * 0.001m));
        }
    }
}
