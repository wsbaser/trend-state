using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using TrendState.Services;

namespace trend_state
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CandlesAggregator.Inst.Start();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
