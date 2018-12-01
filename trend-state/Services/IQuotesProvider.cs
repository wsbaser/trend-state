using System.Collections.Generic;
using TrendState.Models;

namespace TrendState.Services
{
    public interface IQuotesProvider
    {
        List<Quote> GetQuotes();
        List<string> Symbols { get; }
        List<string> SupportedSymbols { get; }
    }
}
