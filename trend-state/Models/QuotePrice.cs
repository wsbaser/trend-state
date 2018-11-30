using System;

namespace TrendState.Models
{
    public class QuotePrice
    {
        public string Symbol;
        public float Price;
        public float Bid;
        public float Ask;

        private long _timestamp;
        public long Timestamp{
            get{
                return _timestamp;
            }
            set{
                _timestamp = value;
                Date = DateTimeOffset.FromUnixTimeMilliseconds(value).DateTime;
            }
        }
        public DateTime Date;

        public QuotePrice(string symbol,long timestamp, float price)
        {
            Symbol = symbol;
            Timestamp = timestamp;
            Price = price;
        }
    }
}