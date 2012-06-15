using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.WindowsAzure;
using FTPServerRole.AzureMngmntRESTLib;

namespace FTPServerRole
{
    class InstanceCountDecider
    {
        private const int MAX_INSTANCE_COUNT = 2;
        private enum ScaleAction { ScaleOut, ScaleIn };

        private const double CPU_WGHT = (double)1 / 2;
        private const double MEMORY_WGHT = (double)1 / 4;
        private const double TCP_CONN_FAIL_WGHT = (double)1 / 4;

        private const double CPU_UPPER = 80.00;
        private const double CPU_LOWER = 10.00;

        private const double REST_MEMORY_UPPER = 100.00;
        private const double REST_MEMORY_LOWER = 950.00;

        private const double TCP_CONN_FAIL_UPPER = 10.00;
        private const double TCP_CONN_FAIL_LOWER = 50.00;

        private static double getAverage(string perfCountType)
        {
            var account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            var context = new PerformanceDataContext(account.TableEndpoint.ToString(), account.Credentials);
            var data = context.PerfData;

            TimeSpan span = TimeSpan.FromMinutes(5.0);
            //TimeSpan span = TimeSpan.FromHours(1.0);
            long start = DateTime.UtcNow.Subtract(span).Ticks;

            var results = (from d in data
                           where d.CounterName == perfCountType && d.EventTickCount > start
                           select d);

            int count = 0;
            double sum = 0.0;

            foreach (var result in results)
            {
                sum += result.CounterValue;
                count++;
            }

            if (count == 0)
                return 0.0;

            //Trace.TraceInformation("Get " + perfCountType + ": " + (sum / count).ToString());
            return sum / count;
        }

        private static double getCPUAverage()
        {
            return getAverage(@"\Processor(_Total)\% Processor Time");
        }

        private static double getMemoryAvailableAverage()
        {
            return getAverage(@"\Memory\Available Mbytes");
        }

        private static double getTcpConnFailAverage()
        {
            return getAverage(@"\TCPv4\Connection Failures");
        }

        private static void ActionOnInstanceCount(ScaleAction action)
        {
            string deploymentInfo = AzureMngmntHandler.GetDeploymentInfo();
            string svcconfig = AzureMngmntHandler.GetServiceConfig(deploymentInfo);
            int InstanceCount = System.Convert.ToInt32(AzureMngmntHandler.GetInstanceCount(svcconfig, "FTPServerRole"));

            if (action == ScaleAction.ScaleIn && InstanceCount > 1)
            {
                InstanceCount--;
                string UpdatedSvcConfig = AzureMngmntHandler.ChangeInstanceCount(svcconfig, "FTPServerRole", InstanceCount.ToString());

                AzureMngmntHandler.ChangeConfigFile(UpdatedSvcConfig);
            }
            else if (action == ScaleAction.ScaleOut && InstanceCount < MAX_INSTANCE_COUNT)
            {
                InstanceCount++;
                string UpdatedSvcConfig = AzureMngmntHandler.ChangeInstanceCount(svcconfig, "FTPServerRole", InstanceCount.ToString());

                AzureMngmntHandler.ChangeConfigFile(UpdatedSvcConfig);
            }
        }

        public static void EvaluatePerfData()
        {
            Trace.TraceInformation("Starting EvaluatePerfData");

            double cpuAverage = getCPUAverage();
            double memoryAvailableAverage = getMemoryAvailableAverage();
            double tcpConnFailAverage = getTcpConnFailAverage();

            if (cpuAverage >= CPU_UPPER || memoryAvailableAverage <= REST_MEMORY_UPPER ||
                tcpConnFailAverage >= TCP_CONN_FAIL_UPPER)
            {
                ActionOnInstanceCount(ScaleAction.ScaleOut);
                Trace.TraceInformation("Try scale out");
                return;
            }
            else if (cpuAverage <= CPU_LOWER && memoryAvailableAverage >= REST_MEMORY_LOWER &&
                tcpConnFailAverage <= TCP_CONN_FAIL_LOWER)
            {
                ActionOnInstanceCount(ScaleAction.ScaleIn);
                Trace.TraceInformation("Try scale in");
                return;
            }

            double cpuProb = 0.00;
            if (cpuAverage >= CPU_UPPER) cpuProb = 1.00;
            else if (cpuAverage <= CPU_LOWER) cpuProb = 0.00;
            else cpuProb = (cpuAverage - CPU_LOWER) / (CPU_UPPER - CPU_LOWER);

            double memoryProb = 0.00;
            if (memoryAvailableAverage <= REST_MEMORY_UPPER) memoryProb = 1.00;
            else if (memoryAvailableAverage >= REST_MEMORY_LOWER) memoryProb = 0.00;
            else memoryProb = (memoryAvailableAverage - REST_MEMORY_UPPER) / (REST_MEMORY_LOWER - REST_MEMORY_UPPER);

            double tcpFailProb = 0.00;
            if (tcpConnFailAverage <= TCP_CONN_FAIL_LOWER) tcpFailProb = 0.00;
            else if (tcpConnFailAverage >= TCP_CONN_FAIL_UPPER) tcpFailProb = 1.00;
            else tcpFailProb = (tcpConnFailAverage - TCP_CONN_FAIL_LOWER) / (TCP_CONN_FAIL_UPPER - TCP_CONN_FAIL_LOWER);

            double prob = CPU_WGHT * cpuProb + MEMORY_WGHT * memoryProb + TCP_CONN_FAIL_WGHT * tcpFailProb;
            Trace.TraceInformation("Scale probability is: " + prob.ToString());
            if (prob > 0.6)
            {
                ActionOnInstanceCount(ScaleAction.ScaleOut);
                Trace.TraceInformation("Try scale out");
            }
            else if (prob < 0.2)
            {
                ActionOnInstanceCount(ScaleAction.ScaleIn);
                Trace.TraceInformation("Try scale in");
            }
            ActionOnInstanceCount(ScaleAction.ScaleOut);
        }


    }
}
