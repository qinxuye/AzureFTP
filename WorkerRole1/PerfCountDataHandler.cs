using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using FTPServerRole.AzureMngmntRESTLib;

namespace FTPServerRole
{
    class PerfCountDataHandler
    {
        public static IQueryable<PerformanceData> GetPerfCountByType(string perfCountType)
        {
            var account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            var context = new PerformanceDataContext(account.TableEndpoint.ToString(), account.Credentials);
            var data = context.PerfData;

            return from d in data
                   where d.CounterName == perfCountType
                   select d;
        }
    }
}
