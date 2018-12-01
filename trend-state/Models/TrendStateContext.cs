using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TrendState.Models
{
    public class TrendStateContext : DbContext
    {
        public DbSet<Candle> Candles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=trendstate;Username=postgres;Password=admin");
        }
    }

    //https://rates.fxcm.com/RatesXML
    //http://webrates.truefx.com/rates/connect.html?f=html

    public class Candle
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }

        public Candle(string symbol, DateTime date, float open, float high, float low, float close)
        {
            Symbol = symbol;
            Date = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
        }

        public Candle(string symbol, DateTime date, float startPrice)
        {
            Symbol = symbol;
            Date = date;
            Open = Close = High = Low = startPrice;
        }
    }
}