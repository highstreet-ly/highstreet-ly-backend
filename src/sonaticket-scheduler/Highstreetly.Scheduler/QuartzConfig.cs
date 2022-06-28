using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;

namespace Highstreetly.Scheduler
{
    public class QuartzConfig : Dictionary<string, string>
    {
        public QuartzConfig UpdateConnectionString(string connectionString)
        {
            this["quartz.dataSource.default.connectionString"] = connectionString;
            return this;
        }

        public NameValueCollection ToNameValueCollection(IConfiguration config)
        {
            var properties = new NameValueCollection
            {
                // json serialization is the one supported under .NET Core (binary isn't)
                ["quartz.serializer.type"] = "json",
                ["quartz.dataSource.default.provider"] = "Npgsql",
                ["quartz.dataSource.default.connectionString"] =  config.GetConnectionString("SchedulerConnection"),
                ["quartz.scheduler.instanceName"] = "SonatribeScheduler",
                ["quartz.scheduler.instanceId"] = "instance_one",
                ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
                ["quartz.threadPool.threadCount"] = "10",
                ["quartz.jobStore.misfireThreshold"] = "60000",
                ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz",
                ["quartz.jobStore.useProperties"] = "false",
                ["quartz.jobStore.dataSource"] = "default",
                ["quartz.jobStore.tablePrefix"] = "QRTZ_",
            };

            foreach (var keyValuePair in this)
            {
                properties.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return properties;
        }
    }
}