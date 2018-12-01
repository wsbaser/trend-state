using TrendState.Models;

namespace TrendState.Services
{
    internal interface ICandlesRepository
    {
        void SaveCandle(Candle candle);
    }
}