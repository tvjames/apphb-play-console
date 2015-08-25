using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

    class Ec2MetaData
    {
        private static readonly ILogger Logger = Log.ForContext<Ec2MetaData>();

        public static Ec2MetaData Create(string url = "http://169.254.169.254/latest/meta-data/")
        {
            var result = new Ec2MetaData();
            var client = new HttpClient();
            try
            {
                Task.WaitAll(
                    client.GetAsync(url + "ami-id").ContinueWith(task =>
                    {
                        if (task.IsCompleted)
                            result.AmiId = task.Result.Content.ReadAsStringAsync().Result;
                    }),
                    client.GetAsync(url + "hostname").ContinueWith(task =>
                    {
                        if (task.IsCompleted)
                            result.Hostname = task.Result.Content.ReadAsStringAsync().Result;
                    }),
                    client.GetAsync(url + "instance-id").ContinueWith(task =>
                    {
                        if (task.IsCompleted)
                            result.InstanceId = task.Result.Content.ReadAsStringAsync().Result;
                    }),
                    client.GetAsync(url + "instance-type").ContinueWith(task =>
                    {
                        if (task.IsCompleted)
                            result.InstanceType = task.Result.Content.ReadAsStringAsync().Result;
                    })
                    );
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Oops");
            }
            return result;
        }

        public string AmiId { get; private set; }
        public string Hostname { get; private set; }
        public string InstanceId { get; private set; }
        public string InstanceType { get; private set; }
    }

    class WithPropertyInit
    {
        public string FirstProp { get; } = "First Prop Value";
        public string SecondProp { get; } = "Second Prop Value";

        public string ConstructorSetProp { get; }

        public WithPropertyInit()
        {
            this.ConstructorSetProp = $"Using string interpolation {Program.Random.Next()}";
        }

        public string NameOfProp() => nameof(this.ConstructorSetProp);
    }
}
