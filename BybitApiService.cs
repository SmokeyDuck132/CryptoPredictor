using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CryptoPricePredictor
{
    public class BybitApiService
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string _apiKey;
        private readonly string _apiSecret;

        public BybitApiService(string apiKey, string apiSecret)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
        }

        public async Task<List<KlineData>> FetchKlineDataAsync(string category, string symbol, string interval, int limit = 200)
        {
            if (string.IsNullOrEmpty(category))
                throw new ArgumentException("Category cannot be null or empty.", nameof(category));
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));
            if (string.IsNullOrEmpty(interval))
                throw new ArgumentException("Interval cannot be null or empty.", nameof(interval));

            string url = $"https://api.bybit.com/v5/market/kline?category={category}&symbol={symbol}&interval={interval}&limit={limit}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                // Ensure the response is successful
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                // Deserialize the response
                BybitKlineResponse? apiResponse = JsonConvert.DeserializeObject<BybitKlineResponse>(json);

                if (apiResponse == null || apiResponse.RetCode != 0)
                {
                    throw new Exception($"API Error: {apiResponse?.RetMsg ?? "Unknown error"}");
                }

                // Parse the Kline data from the "list" field
                var result = new List<KlineData>();
                foreach (var item in apiResponse.Result.List)
                {
                    result.Add(new KlineData
                    {
                        OpenTime = Convert.ToInt64(item[0]),
                        Open = Convert.ToDouble(item[1]),
                        High = Convert.ToDouble(item[2]),
                        Low = Convert.ToDouble(item[3]),
                        Close = Convert.ToDouble(item[4]),
                        Volume = Convert.ToDouble(item[5]),
                        Turnover = 0.0 // Update if the API provides this information
                    });
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching data from Bybit API: {ex.Message}");
            }
        }
    }



    public class BybitKlineResponse
    {
        [JsonProperty("retCode")]
        public int RetCode { get; set; }

        [JsonProperty("retMsg")]
        public string RetMsg { get; set; } = string.Empty;

        [JsonProperty("result")]
        public BybitKlineResult Result { get; set; } = new BybitKlineResult();
    }

    public class BybitKlineResult
{
    [JsonProperty("category")]
    public string Category { get; set; } = string.Empty;

    [JsonProperty("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonProperty("interval")]
    public string Interval { get; set; } = string.Empty;

    [JsonProperty("list")]
    public List<List<object>> List { get; set; } = new List<List<object>>();
}
}
