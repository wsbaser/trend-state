using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;
using TrendState.Models;

namespace TrendState.Services
{
    public class CandlesAggregator
    {
        public static CandlesAggregator Inst = new CandlesAggregator(new List<string> { "EUR/USD" });
        public Dictionary<string, List<Candle>> Candles;
        public Dictionary<string, Candle> ActiveCandle;
        public ushort CandlePeriod;

        private CandlesAggregator(List<string> quotes, ushort candlePeriod=60)
        {
            Candles = new Dictionary<string, List<Candle>>();
            ActiveCandle = new Dictionary<string, Candle>();
            foreach(var quote in quotes){
                Candles.Add(quote, new List<Candle>());
                ActiveCandle.Add(quote, new Candle(DateTime.UtcNow));
            }
            CandlePeriod = candlePeriod;
        }

        public void Start()
        {
            int delay = 1000;
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            var listener = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    // poll hardware
                    var quotes = GetCurrentQuotePrices();
                    Aggregate(quotes);

                    Thread.Sleep(delay);
                    if (token.IsCancellationRequested)
                        break;
                }

                // cleanup, e.g. close connection
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void Aggregate(List<QuotePrice> quotePrices)
        {
            foreach (var symbol in ActiveCandle.Keys)
            {
                var quotePrice = quotePrices.SingleOrDefault(q => q.Symbol == symbol);
                if (quotePrice == null)
                {
                    throw new ArgumentException($"Input data has no price for '{symbol}'.");
                }
                var quoteActiveCandle = ActiveCandle[quotePrice.Symbol];
                if (UpdateCandle(quoteActiveCandle, quotePrice))
                {
                    Candles[symbol].Add(quoteActiveCandle);
                    var newCandleDate = quoteActiveCandle.Date.AddSeconds(CandlePeriod);
                    ActiveCandle[quotePrice.Symbol] = new Candle(newCandleDate, quotePrice.Price);
                }
            }
        }

        private bool UpdateCandle(Candle canle, QuotePrice newPrice)
        {
            if (newPrice.Price > canle.High)
            {
                canle.High = newPrice.Price;
            }
            if (newPrice.Price < canle.Low)
            {
                canle.Low = newPrice.Price;
            }
            if ((newPrice.Date - canle.Date).TotalSeconds >= CandlePeriod)
            {
                canle.Close = newPrice.Price;
                return true;
            }
            return false;
        }

        private static List<QuotePrice> GetCurrentQuotePrices()
        {
            var quotes = new List<QuotePrice>();
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
                var price = float.Parse(priceNode1.InnerText.Trim() + priceNode2.InnerText.Trim());
                quotes.Add(new QuotePrice(symbolNode.InnerText, long.Parse(timestampNode.InnerText), price));
            }

            return quotes;
        }
    }
}
