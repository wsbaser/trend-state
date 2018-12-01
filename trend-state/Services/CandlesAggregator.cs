﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using TrendState.Models;

namespace TrendState.Services
{
    public class CandlesAggregator
    {
        public static CandlesAggregator Inst = new CandlesAggregator(
            new List<string> { "BTCEUR" },
            60,
            new OneForgeQuotesProvider(new List<string>() { "BTCEUR" }));

        public Dictionary<string, List<Candle>> Candles;
        private Dictionary<string, Candle> _activeCandle;
        public ushort CandlePeriod;
        private readonly IQuotesProvider _quotesProvider;
        public readonly List<string> _symbols;

        private CandlesAggregator(List<string> symbols, ushort candlePeriod, IQuotesProvider quotesProvider)
        {
            _symbols = symbols;
            Candles = new Dictionary<string, List<Candle>>();
            _activeCandle = new Dictionary<string, Candle>();
            foreach (var quote in symbols)
            {
                Candles.Add(quote, new List<Candle>());
            }
            CandlePeriod = candlePeriod;
            _quotesProvider = quotesProvider;
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
                    try
                    {
                        Aggregate(_quotesProvider.GetQuotes());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    Thread.Sleep(delay);
                    if (token.IsCancellationRequested)
                        break;
                }

                // cleanup, e.g. close connection
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void Aggregate(List<Quote> quotePrices)
        {
            foreach (var symbol in _symbols)
            {
                var quotePrice = quotePrices.SingleOrDefault(q => q.Symbol == symbol);
                var activeCandle = _activeCandle.ContainsKey(symbol) ? _activeCandle[symbol] : null;
                if (quotePrice == null)
                {
                    Console.WriteLine($"No price for '{symbol}'.");
                    quotePrice = new Quote(symbol, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), activeCandle?.Close ?? 0);
                }
                if (activeCandle == null)
                {
                    // . create first candle
                    _activeCandle[quotePrice.Symbol] = new Candle(DateTime.UtcNow, quotePrice.Price);
                }
                else
                {
                    // .upadte candle
                    var quoteActiveCandle = _activeCandle[quotePrice.Symbol];
                    if (UpdateCandle(quoteActiveCandle, quotePrice))
                    {
                        // . save closed candle, create new
                        Candles[symbol].Add(quoteActiveCandle);
                        var newCandleDate = quoteActiveCandle.Date.AddSeconds(CandlePeriod);
                        _activeCandle[quotePrice.Symbol] = new Candle(newCandleDate, quotePrice.Price);
                    }
                }
            }
        }

        private bool UpdateCandle(Candle canle, Quote newPrice)
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

    }
}
