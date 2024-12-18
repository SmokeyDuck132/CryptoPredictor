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
                            text = $"Analyze the following cryptocurrency market data and indicators (MA7, MA14, MA28, EMA7, EMA14, EMA28, Bollinger Bands, SAR, RSI6, RSI12, RSI24, StochRSI, MACD, KDJ):\n\n" +
                                   $"{inputData}\n\n" +
                                   "Provide a clear and concise prediction summary for the next 1 day, 5 days, 15 days, and 3 months. " +
                                   "Include the following for each time frame:\n" +
                                   "1. Whether the price will rise or fall.\n" +
                                   "2. The expected price range (predicted high and low).\n" +
                                   "3. The expected percentage change (rise or drop).\n" +
                                   "4. The probability (chances) of each scenario happening, such as 60% rise and 40% fall.\n" +
                                   "5. The key indicators (like MA, EMA, RSI, MACD) that influenced the prediction.\n\n" +
                                   "Format the response strictly as:\n" +
                                   "1 Day:\n- Movement: [Rise/Fall]\n- Range: [Low Price] - [High Price]\n- Change: [+X.XX% / -Y.YY%]\n- Probability: [X% Rise / Y% Fall]\n- Key Indicators: [MA7, RSI, MACD]\n\n" +
                                   "5 Days:\n- Movement: [Rise/Fall]\n- Range: [Low Price] - [High Price]\n- Change: [+X.XX% / -Y.YY%]\n- Probability: [X% Rise / Y% Fall]\n- Key Indicators: [Bollinger Bands, EMA14]\n\n" +
                                   "15 Days:\n- Movement: [Rise/Fall]\n- Range: [Low Price] - [High Price]\n- Change: [+X.XX% / -Y.YY%]\n- Probability: [X% Rise / Y% Fall]\n- Key Indicators: [RSI24, MACD]\n\n" +
                                   "3 Months:\n- Movement: [Rise/Fall]\n- Range: [Low Price] - [High Price]\n- Change: [+X.XX% / -Y.YY%]\n- Probability: [X% Rise / Y% Fall]\n- Key Indicators: [EMA28, KDJ]"
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
