using TrendState.Models;

namespace TrendState.Services
{
    internal class TrendStateRepository : ICandlesRepository
    {
        private TrendStateContext _dbContext;

        public TrendStateRepository()
        {
            _dbContext = new TrendStateContext();
        }

        public void SaveCandle(Candle candle)
        {
            _dbContext.Candles.Add(candle);
            _dbContext.SaveChangesAsync();
        }
    }
}