using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;
using System.Data.Services.Client;
using Microsoft.WindowsAzure;
using System.Diagnostics;


namespace FTPWebRole
{
    public class PerformanceData : Microsoft.WindowsAzure.StorageClient.TableServiceEntity
    {
        public Int64 EventTickCount { get; set; }
        public string DeploymentId { get; set; }
        public string Role { get; set; }
        public string RoleInstance { get; set; }
        public string CounterName { get; set; }
        public double CounterValue { get; set; }

    }
    public class Performance
    {
        public Performance(PerformanceData data)
        {
            this.TimeCur = Util.getDateTime(data.EventTickCount).Hour.ToString()+":"+Util.getDateTime(data.EventTickCount).Minute.ToString() + ":" + Util.getDateTime(data.EventTickCount).Second.ToString();
            this.DeploymentId = data.DeploymentId;
            this.Role = data.Role;
            this.RoleInstance = data.RoleInstance;
            this.CounterName = data.CounterName;
            this.CounterValue = data.CounterValue;

        }
        public string TimeCur { get; set; }
        public string DeploymentId { get; set; }
        public string Role { get; set; }
        public string RoleInstance { get; set; }
        public string CounterName { get; set; }
        public double CounterValue { get; set; }

    }

    public class PerformanceDataContext : Microsoft.WindowsAzure.StorageClient.TableServiceContext
    {
        public IQueryable<PerformanceData> PerfData
        {
            get { return this.CreateQuery<PerformanceData>("WADPerformanceCountersTable"); }
        }
        public PerformanceDataContext(string baseAddress, Microsoft.WindowsAzure.StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
        }
    }

    public class queryDataProvider
    {



        public static List<Performance> GetProcessorTime()
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var context = new PerformanceDataContext(account.TableEndpoint.ToString(), account.Credentials);
            var data = context.PerfData;
            var timeFrameInMinutes = 30;
            DateTime tf =
DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(timeFrameInMinutes));


            System.Collections.Generic.List<PerformanceData> selectedData = (from d in
                                                                                 data
                                                                             where
d.CounterName == @"\Processor(_Total)\% Processor Time"
&&
(DateTime.Compare(tf, d.Timestamp) < 0)
                                                                             select
d).ToList<PerformanceData>();

            //            List<Double> CPUList = new List<Double>();
            var performacelist = new List<Performance>();
            if (selectedData.Count >= 10)
            {
                selectedData = selectedData.GetRange(selectedData.Count - 10, 10);
                foreach (PerformanceData temp in selectedData)
                    performacelist.Add(new Performance(temp));
            }


            return performacelist;
            /*      foreach(PerformanceData temp in selectedData)
                  {
                      CPUList.Add(temp.CounterValue);
                      Trace.WriteLine(temp.CounterValue);
                  }

                  return selectedData;
     /*

                 List<double> AvgCPU = (from d in selectedData
                                        where d.CounterName == @"\Processor(_Total)\% Processor Time"
                                        select d.CounterValue);
     ;


     if (AvgCPU.Count >= 10)
         return AvgCPU.GetRange(AvgCPU.Count - 10, AvgCPU.Count - 1);
     else
         return AvgCPU;
      * 
      */
        }

        public static List<Performance> GetMemoryTime()
        {

            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var context = new PerformanceDataContext(account.TableEndpoint.ToString(), account.Credentials);
            var data = context.PerfData;
            var timeFrameInMinutes = 30;
            DateTime tf =
DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(timeFrameInMinutes));


            System.Collections.Generic.List<PerformanceData> selectedData = (from d in
                                                                                 data
                                                                             where
d.CounterName == @"\Memory\Available Mbytes"
&&
(DateTime.Compare(tf, d.Timestamp) < 0)
                                                                             select
d).ToList<PerformanceData>();

            var performacelist = new List<Performance>();
            if (selectedData.Count >= 10)
            {
                selectedData = selectedData.GetRange(selectedData.Count - 10, 10);
                foreach (PerformanceData temp in selectedData)
                    performacelist.Add(new Performance(temp));
            }


            return performacelist;

        }

        public static List<Performance> GetTcpFail()
        {

            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var context = new PerformanceDataContext(account.TableEndpoint.ToString(), account.Credentials);
            var data = context.PerfData;
            var timeFrameInMinutes = 30;
            DateTime tf =
DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(timeFrameInMinutes));


            System.Collections.Generic.List<PerformanceData> selectedData = (from d in
                                                                                 data
                                                                             where
d.CounterName == @"\TCPv4\Connection Failures"
&&
(DateTime.Compare(tf, d.Timestamp) < 0)
                                                                             select
d).ToList<PerformanceData>();
            var performacelist = new List<Performance>();
            if (selectedData.Count >= 10)
            {
                selectedData = selectedData.GetRange(selectedData.Count - 10, 10);
                foreach (PerformanceData temp in selectedData)
                {
                    performacelist.Add(new Performance(temp));
                }
            }


            return performacelist;

        }

    }
    public class Util
    {
        public static DateTime getDateTime(long spantime)
        {
            //   var datetimeStart = new DateTime(1970, 1, 1,0,0,0);
            //   var timeSpan = new TimeSpan(spantime);
            //   var datetimeNow = datetimeStart + timeSpan;
            //   return datetimeNow;
            //  string timeStamp = now;
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1, 1, 1));
            //  long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(spantime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;

        }
    }
}