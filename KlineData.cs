namespace CryptoPricePredictor
{
    // Data class for storing Kline (candlestick) information
    public class KlineData
    {
        public long OpenTime { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public double Turnover { get; set; }
    }
}
