using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrendState.Analyzers;
using TrendState.Models;
using TrendState.Services;
using TrendState.Utils;

namespace TrendState.Controllers
{
    [Route("api/[controller]")]
    public partial class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        [HttpGet("[action]")]
        public IEnumerable<CandleDTO> Candles()
        {
            //var candlesLoader = new CandlesLoader("HistoryData/EURUSD_201810.csv");
            //var candles = candlesLoader.LoadAll();
            var candles = CandlesAggregator.Inst.Candles["EURUSD"];
            var analyzer = new TrendStateAnalyzer();
            var candlesDto = new List<CandleDTO>();
            foreach (var candle in candles)
            {
                var candleDto = new CandleDTO(candle);
                //candlesDto.Add(candleDto);
                var volume = analyzer.AddCandle(candleDto);
                if (volume > 0.5)
                {
                    candleDto.Volume = volume;
                }
                candlesDto.Add(candleDto);
            }

            return candlesDto;
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }
    }
}
