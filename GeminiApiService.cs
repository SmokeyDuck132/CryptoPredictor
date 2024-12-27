using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoPricePredictor
{
    public class GeminiApiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GeminiApiService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        public async Task<string> GetPredictionAsync(string inputData)
        {
            if (string.IsNullOrEmpty(_apiKey))
                throw new InvalidOperationException("API Key is not set.");

            try
            {
                var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent?key={_apiKey}";

                var requestBody = new
                {
                    contents = new[]
                    {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new
                        {
                            text = $"You are a virtual currency investment expert specializing in swing trading to accumulate more assets. Analyze the following cryptocurrency market data and technical indicators (MA7, MA14, MA28, EMA7, EMA14, EMA28, Bollinger Bands, SAR, RSI6, RSI12, RSI24, StochRSI, MACD, KDJ):\n\n" +
         $"{inputData}\n\n" +
         "Provide a concise and actionable prediction summary for the next 1 day, 5 days, 15 days, and 3 months. For each time frame, include the following details:\n" +
         "1. **Price Movement**: Indicate whether the price is expected to Rise or Fall.\n" +
         "2. **Price Range**: Specify the predicted Low and High Prices.\n" +
         "3. **Percentage Change**: Detail the expected percentage Increase or Decrease.\n" +
         "4. **Probability Assessment**: Provide the likelihood of each scenario (e.g., 60% Rise, 40% Fall).\n" +
         "5. **Key Indicators**: Highlight the technical indicators influencing the prediction.\n" +
         "6. **Swing Trading Zones**:\n" +
         "   - **BuyZone**: Recommended price to initiate a buy order.\n" +
         "   - **SellZone**: Strategic price to execute a sell order.\n" +
         "   - **ReBuyZone**: Suggested price to rebuy after selling (e.g., sell at 99k, rebuy at 95k).\n\n" +
         "Ensure the response strictly follows the structure below without additional explanations:\n\n" +
         "### 1 Day:\n" +
         "- **BuyZone**: [BuyPrice]\n" +
         "- **SellZone**: [SellPrice]\n" +
         "- **ReBuyZone**: [ReBuyPrice]\n" +
         "- **Movement**: [Rise/Fall]\n" +
         "- **Range**: [Low Price] - [High Price]\n" +
         "- **Change**: [+X.XX% / -Y.YY%]\n" +
         "- **Probability**: [X% Rise / Y% Fall]\n" +
         "- **Key Indicators**: [MA7, RSI, MACD]\n\n" +
         "### 5 Days:\n" +
         "- **BuyZone**: [BuyPrice]\n" +
         "- **SellZone**: [SellPrice]\n" +
         "- **ReBuyZone**: [ReBuyPrice]\n" +
         "- **Movement**: [Rise/Fall]\n" +
         "- **Range**: [Low Price] - [High Price]\n" +
         "- **Change**: [+X.XX% / -Y.YY%]\n" +
         "- **Probability**: [X% Rise / Y% Fall]\n" +
         "- **Key Indicators**: [Bollinger Bands, EMA14]\n\n" +
         "### 15 Days:\n" +
         "- **BuyZone**: [BuyPrice]\n" +
         "- **SellZone**: [SellPrice]\n" +
         "- **ReBuyZone**: [ReBuyPrice]\n" +
         "- **Movement**: [Rise/Fall]\n" +
         "- **Range**: [Low Price] - [High Price]\n" +
         "- **Change**: [+X.XX% / -Y.YY%]\n" +
         "- **Probability**: [X% Rise / Y% Fall]\n" +
         "- **Key Indicators**: [RSI24, MACD]\n\n" +
         "### 3 Months:\n" +
         "- **BuyZone**: [BuyPrice]\n" +
         "- **SellZone**: [SellPrice]\n" +
         "- **ReBuyZone**: [ReBuyPrice]\n" +
         "- **Movement**: [Rise/Fall]\n" +
         "- **Range**: [Low Price] - [High Price]\n" +
         "- **Change**: [+X.XX% / -Y.YY%]\n" +
         "- **Probability**: [X% Rise / Y% Fall]\n" +
         "- **Key Indicators**: [EMA28, KDJ]"
                        }
                    }
                }
            },
                    generationConfig = new
                    {
                        temperature = 0.7,  // Balanced randomness
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 512, // Adjust for enough response length
                        responseMimeType = "text/plain"
                    }
                };

                var jsonRequest = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(requestUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Request failed: {response.StatusCode}, {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return ExtractPrediction(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching prediction: {ex.Message}");
            }
        }





        private string ExtractPrediction(string jsonResponse)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                var responseText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return responseText?.Replace("\n", "\r\n").Trim() ?? "No valid prediction returned.";
            }
            catch
            {
                return "Error parsing the prediction response.";
            }
        }
    }
}
