using System;

namespace TrendState.Models
{

    public class CandleDTO
    {
        public DateTime Date;
        public float Open;
        public float High;
        public float Low;
        public float Close;
        public float Volume;

        public CandleDTO(){
        }

        public CandleDTO(Candle candle)
        {
            Date = candle.Date;
            Open = candle.Open;
            High = candle.High;
            Low = candle.Low;
            Close = candle.Close;
        }
    }
}