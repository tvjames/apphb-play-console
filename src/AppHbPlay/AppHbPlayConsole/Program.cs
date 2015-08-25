using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Serilog;

namespace AppHbPlayConsole
{
    class Program
    {
        internal static readonly Random Random = new Random();

        static void Main(string[] args)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .Enrich.WithMachineName()
                .MinimumLevel.Debug();
            var logger = loggerConfiguration.CreateLogger();
            Log.Logger = logger;

            Log.Information("Starting up. Testing out all things C# 6.");

            var sleepPeriodInSeconds = 60;
            if (args.Length > 0 && int.TryParse(args[0], out sleepPeriodInSeconds))
            {
                Log.Information("Sleep interval provided. Setting sleep period {SleepPeriodInSeconds}.", sleepPeriodInSeconds);
            }
            var sleepPeriod = TimeSpan.FromSeconds(sleepPeriodInSeconds);

            Log.Information("EC2 Host information, if any. {@Ec2MetaData}", Ec2MetaData.Create());

            while (true)
            {
                var coinToss = Random.Next(0, 2);
                Log.Information("Coin Toss... {CoinToss}", coinToss);

                var instance = coinToss == 0 ? new WithPropertyInit() : null;

                Log.Information("Doing Work... {NameOfProp}", instance?.NameOfProp());
                Log.Information("{@Instance}", instance);

                Thread.Sleep(sleepPeriod);
            }
        }
    }
}
