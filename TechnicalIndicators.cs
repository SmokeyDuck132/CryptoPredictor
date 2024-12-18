using Skender.Stock.Indicators;
using System.Collections.Generic;
using System.Linq;

namespace CryptoPricePredictor
{
    public class TechnicalIndicators
    {
        public List<SmaResult> CalculateSMA(List<decimal> closes, int period, string interval)
        {
            var quotes = GenerateQuotes(closes, interval);
            return quotes.GetSma(period).ToList();
        }

        public List<EmaResult> CalculateEMA(List<decimal> closes, int period, string interval)
        {
            var quotes = GenerateQuotes(closes, interval);
            return quotes.GetEma(period).ToList();
        }

        public List<RsiResult> CalculateRSI(List<decimal> closes, int period, string interval)
        {
            var quotes = GenerateQuotes(closes, interval);
            return quotes.GetRsi(period).ToList();
        }

        public List<MacdResult> CalculateMACD(List<decimal> closes, string interval)
        {
            var quotes = GenerateQuotes(closes, interval);
            return quotes.GetMacd().ToList();
        }

        public List<BollingerBandsResult> CalculateBollingerBands(List<decimal> closes, int period, double stdDevMultiplier, string interval)
        {
            var quotes = GenerateQuotes(closes, interval);
            return quotes.GetBollingerBands(period, stdDevMultiplier).ToList();
        }

        // Implement other indicators similarly...

        private IEnumerable<Quote> GenerateQuotes(List<decimal> closes, string interval)
        {
            // Determine the TimeSpan based on the interval
            TimeSpan timeSpan = interval switch
            {
                "1m" => TimeSpan.FromMinutes(1),
                "5m" => TimeSpan.FromMinutes(5),
                "15m" => TimeSpan.FromMinutes(15),
                "1h" => TimeSpan.FromHours(1),
                "4h" => TimeSpan.FromHours(4),
                "1d" => TimeSpan.FromDays(1),
                _ => TimeSpan.FromMinutes(1),
            };

            // Assume the latest date is now, adjust as needed
            DateTime startDate = DateTime.UtcNow.Add(timeSpan * -closes.Count);

            return closes.Select((close, index) => new Quote
            {
                Date = startDate.Add(timeSpan * index),
                Close = (decimal)close
            });
        }
    }
}
