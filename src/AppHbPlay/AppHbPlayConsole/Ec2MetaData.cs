using System;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;

namespace AppHbPlayConsole
{
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
}