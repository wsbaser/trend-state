using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using TrendState.Models;

namespace TrendState.Services
{
    public class OneForgeQuotesProvider : IQuotesProvider
    {
        public List<string> SupportedSymbols => new List<string>() { "EURUSD" };

        public List<string> Symbols { get; private set; }

        public OneForgeQuotesProvider(List<string> symbols)
        {
            Symbols = symbols;
            if (SupportedSymbols.Intersect(symbols).Count() != symbols.Count)
            {
                throw new ArgumentException("Provider does not support some of requested symbols.");
            }
        }

        public List<Quote> GetQuotes()
        {
            WebClient webClient = new WebClient();
            var symbols = string.Join(",", Symbols);
            var apiKey = "xq9A5Y91fi4orCZOagDdyBHpEdDcTa3w";
            var url = $"https://forex.1forge.com/1.0.3/quotes?pairs={symbols}&api_key={apiKey}";
            string jsonString;
            try
            {
                jsonString = webClient.DownloadString(url);
            }
            catch (WebException)
            {
                // TODO: log error here
                return new List<Quote>();
            }
            List<JsonQuote> responseQuotes;

            try
            {
                responseQuotes = JsonConvert.DeserializeObject<List<JsonQuote>>(jsonString);
            }
            catch (JsonException)
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(jsonString);
                Console.WriteLine(errorResponse.message);
                return new List<Quote>();
            }

            var symbolToQuote = responseQuotes.ToDictionary(q => q.symbol);
            var quotes = new List<Quote>();
            foreach (var symbol in Symbols)
            {
                if (!symbolToQuote.ContainsKey(symbol))
                {
                    throw new Exception($"Source has no quote for '{symbol}' symbol.");
                }
                quotes.Add(symbolToQuote[symbol].ToQuote());
            }
            return quotes;
        }

        private class ErrorResponse
        {
            public string error;
            public string message;

        }

        private class JsonQuote
        {
            public string symbol;
            public float price;
            public float bid;
            public float ask;
            public long timestamp;

            internal Quote ToQuote()
            {
                return new Quote(symbol, timestamp * 1000, price);
            }
        }
    }
}
