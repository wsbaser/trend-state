using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using trend_state.Controllers;

namespace TrendState.Analyzers
{
    public class TrendStateAnalyzer
    {
        public List<Candle> _candles;
        private Candle LastCandle => _candles.Last();

        private EMACalculator _ma8;
        private EMACalculator _ma21;
        private Stake _ma8Stake;
        private Stake _ma21Stake;
        private List<Stake> _closedStakes;

        public Lazy<float> OnePip;

        public TrendStateAnalyzer()
        {
            _ma8 = new EMACalculator(8);
            _ma21 = new EMACalculator(21);
            _candles = new List<Candle>();
            _closedStakes = new List<Stake>();
            OverallQuality = new EMACalculator(5);
            OnePip = new Lazy<float>(CalculateOnePip);
        }

        private float CalculateOnePip()
        {
            decimal argument = (decimal)_candles.First().Close;
            int count = BitConverter.GetBytes(decimal.GetBits(argument)[3])[2];
            return (float) Math.Pow(0.1, count);
        }

        public EMACalculator OverallQuality;

        public Direction StakeDirection => TrendDirection;

        public Direction TrendDirection => _ma8.Value > _ma21.Value ? Direction.Up : Direction.Down;

        public float AddCandle(Candle candle)
        {
            _candles.Add(candle);
            if (CalculateEMAs(candle))
            {
                CheckActiveStakes();
                AddStakes();
            }

            return OverallQuality.Value;
        }
        
        private bool CalculateEMAs(Candle candle)
        {
            _ma8.AddValue(candle.Close);
            _ma21.AddValue(candle.Close);
            return _candles.Count >= _ma21.MinReadyCount;
        }

        private void CheckActiveStakes()
        {
            var lastCandle = _candles.Last();
            if (_ma8Stake != null)
            {
                _ma8Stake.AddCandle(lastCandle);
                if (_ma8Stake.Closed)
                {
                    _closedStakes.Add(_ma8Stake);
                    //OverallQuality.AddValue(_ma8Stake.Quality);
                    _ma8Stake = null;
                }
            }

            if (_ma21Stake != null)
            {
                _ma21Stake.AddCandle(lastCandle);
                if (_ma21Stake.Closed)
                {
                    _closedStakes.Add(_ma21Stake);
                    OverallQuality.AddValue(_ma21Stake.Quality);
                    _ma21Stake = null;
                }
            }
        }

        private void AddStakes()
        {
            if (_ma21Stake == null)
            {
                var touchPrice = GetTouchPrice(_ma21);
                if (touchPrice.HasValue)
                {
                    _ma21Stake = new Stake(touchPrice.Value, 7, StakeDirection, LastCandle, OnePip.Value);
                    if (_ma8Stake != null)
                    {
                        _ma8Stake = null;
                    }
                }
                else if (_ma8Stake == null)
                {
                    touchPrice = GetTouchPrice(_ma8);
                    if (touchPrice.HasValue)
                    {
                        _ma8Stake = new Stake(touchPrice.Value, 7, StakeDirection, LastCandle, OnePip.Value);
                    }
                }
            }
        }

        private float? GetTouchPrice(EMACalculator ema)
        {
            if (TrendDirection == Direction.Up)
            {
                if (LastCandle.Low - ema.Value <= OnePip.Value*2)
                {
                    return ema.Value;
                }
            }
            else
            {
                if (ema.Value - LastCandle.High <= OnePip.Value*2)
                {
                    return ema.Value;
                }
            }

            return null;
        }
    }

    public enum Direction
    {
        Up,
        Down
    }

    internal class Stake
    {
        private List<Candle> _candles;
        public float Price;
        public int Time;
        private Direction Direction;
        private float _onePip { get; set; }

        public Stake(float price, int time, Direction direction, Candle firstCandle, float onePip)
        {
            Price = price;
            Time = time;
            Direction = direction;
            _onePip = onePip;
            _candles = new List<Candle> { firstCandle };
        }

        public float Quality { get; set; }
        public bool Closed { get; private set; }

        public void AddCandle(Candle candle)
        {
            _candles.Add(candle);
            if (_candles.Count == Time)
            {
                _candles.Add(candle);
                CalculateQuality();
                Closed = true;
            }
        }

        private void CalculateQuality()
        {
            int direction = Direction == Direction.Up ? 1 : -1;
            var diff = (_candles.Last().Close - Price) * direction;
            if (diff < 0)
            {
                Quality = -1;
            }
            else if (diff < _onePip * 5)
            {
                Quality = (float) 0.1;
            }
            else
            {
                Quality = 1;
            }
        }
    }

    public class EMACalculator
    {
        private readonly int _timeFrame;
        private readonly Queue<float> _values;
        private readonly float _multiplier;
        private float _sum;
        public float SMA => _sum / _timeFrame;
        public float Value;
        public int MinReadyCount => _timeFrame + 1;

        public EMACalculator(int timeframe)
        {
            _timeFrame = timeframe;
            _multiplier = 2 / (float) (timeframe + 1);
            _values = new Queue<float>();
        }

        public void AddValue(float newValue)
        {
            _sum += newValue;
            _values.Enqueue(newValue);

            if (_values.Count == _timeFrame)
            {
                // .first calculation - use SMA as first EMA value
                Value = SMA;
            }
            else if (_values.Count > _timeFrame)
            {
                _sum -= _values.Dequeue();
                Value = (newValue - Value) * _multiplier + Value;
            }
        }
    }

    [TestFixture]
    public class TrendStateAnalyzerTest
    {
        [Test]
        public void CalculateTrendQuality()
        {
            // .Arrange
            var candlesLoader = new CandlesLoader("HistoryData/EURUSD_201810.csv");
            var historicalData = candlesLoader.LoadAll().Take(100);
            var analyzer = new TrendStateAnalyzer();

            // .Act
            foreach (var candle in historicalData.Take(100))
            {
                analyzer.AddCandle(candle);
            }

            // .Assert
            Assert.NotZero(analyzer.OverallQuality.Value);
        }
    }
}
