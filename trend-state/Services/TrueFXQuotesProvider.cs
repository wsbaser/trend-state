using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using TrendState.Models;

namespace TrendState.Services
{
    public class TrueFXQuotesProvider : IQuotesProvider
    {
        public List<string> SupportedSymbols => new List<string>() { "EURUSD" };
        public List<string> Symbols { get; private set; }

        public TrueFXQuotesProvider(List<string> symbols)
        {
            Symbols = symbols;
            if (SupportedSymbols.Intersect(symbols).Count() != symbols.Count)
            {
                throw new ArgumentException("Provider does not support some of requested symbols.");
            }
        }

        public List<Quote> GetQuotes()
        {
            var allQuotes = new List<Quote>();
            WebClient webClient = new WebClient();
            var xml = webClient.DownloadString("http://webrates.truefx.com/rates/connect.html?f=html");
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);
            XmlNodeList trNodes = document.DocumentElement.SelectNodes("//table/tr");
            foreach (XmlNode trNode in trNodes)
            {
                var symbolNode = trNode.SelectSingleNode("td[1]");
                var timestampNode = trNode.SelectSingleNode("td[2]");
                var priceNode1 = trNode.SelectSingleNode("td[3]");
                var priceNode2 = trNode.SelectSingleNode("td[4]");
                var symbol = symbolNode.InnerText.Replace("/", string.Empty);
                var price = float.Parse(priceNode1.InnerText.Trim() + priceNode2.InnerText.Trim());
                allQuotes.Add(new Quote(symbol, long.Parse(timestampNode.InnerText), price));
            }

            var symbolToQuote = allQuotes.ToDictionary(q => q.Symbol);
            var quotes = new List<Quote>();
            foreach (var symbol in Symbols)
            {
                if (!symbolToQuote.ContainsKey(symbol))
                {
                    throw new Exception($"Source has no quote for '{symbol}' symbol.");
                }
                quotes.Add(symbolToQuote[symbol]);
            }
            return quotes;
        }
    }
}
