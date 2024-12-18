using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace CryptoPricePredictor
{
    public class IndicatorAnalyzer
    {
        public string AnalyzeIndicators(
            List<EmaResult> ema,
            List<MacdResult> macd,
            List<RsiResult> rsi,
            List<BollingerBandsResult> bollingerBands,
            double currentClose
        )
        {
            int bullishCount = 0;
            int bearishCount = 0;

            // Latest index
            int latestIndex = ema.Count - 1;

            if (latestIndex < 1 || macd.Count == 0 || rsi.Count == 0 || bollingerBands.Count == 0)
            {
                return "Not enough data to analyze.";
            }

            // EMA Signal: EMA rising vs falling
            if (ema[latestIndex].Ema > ema[latestIndex - 1].Ema)
                bullishCount++;
            else
                bearishCount++;

            // MACD Signal: MACD above signal line vs below
            if (macd[latestIndex].Macd > macd[latestIndex].Signal)
                bullishCount++;
            else
                bearishCount++;

            // RSI Signal: Oversold vs Overbought
            if (rsi[latestIndex].Rsi < 30)
                bullishCount++;
            else if (rsi[latestIndex].Rsi > 70)
                bearishCount++;

            // Bollinger Bands Signal: Price touching lower or upper band
            var lowerBand = (double?)bollingerBands[latestIndex].LowerBand;
            var upperBand = (double?)bollingerBands[latestIndex].UpperBand;

            if (currentClose <= lowerBand)
                bullishCount++;
            else if (currentClose >= upperBand)
                bearishCount++;

            // Additional indicator signals can be added here...

            // Decide Prediction
            if (bullishCount > bearishCount)
            {
                return "Price is likely to rise.";
            }
            else if (bearishCount > bullishCount)
            {
                return "Price is likely to fall.";
            }
            else
            {
                return "Price movement is uncertain. Wait for further confirmation.";
            }
        }
    }
}
