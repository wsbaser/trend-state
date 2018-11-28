using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TrendState.Models
{
    public class TrendStateContext : DbContext
    {
        public DbSet<Candle> Users { get; set; }

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
        public DateTime Date { get; set; }
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
    }
}