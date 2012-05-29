using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using AzureFtpServer.Azure;
using AzureFtpServer.Ftp;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics.Management;


using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace FTPServerRole {
    public class WorkerRole : RoleEntryPoint {
        private FtpServer _server;

        public override void Run() {

            Trace.WriteLine("FTPRole entry point called", "Information");
           
            while (true) {
                if (_server.Started)
                {
                    Thread.Sleep(10000);
                    Trace.WriteLine("Server is alive.", "Information");
                }
                else
                {
                    _server.Start();
                    Trace.WriteLine("Server starting.", "Control");
                }

            }
        }

        public override bool OnStart() {

  
            ServicePointManager.DefaultConnectionLimit = 12;


            DiagnosticMonitorConfiguration diagConfig = DiagnosticMonitor.GetDefaultInitialConfiguration();
           /* diagConfig.Directories.ScheduledTransferPeriod = TimeSpan.FromSeconds(15);
            diagConfig.Logs.ScheduledTransferPeriod = TimeSpan.FromSeconds(15);
            
           */
            PerformanceCounterConfiguration pccCPU = new PerformanceCounterConfiguration();
            pccCPU.CounterSpecifier = @"\Processor(_Total)\% Processor Time";
            pccCPU.SampleRate = TimeSpan.FromSeconds(5);
            diagConfig.PerformanceCounters.DataSources.Add(pccCPU);

            PerformanceCounterConfiguration pccMemory = new PerformanceCounterConfiguration();
            pccMemory.CounterSpecifier = @"\Memory\Available Mbytes";
            pccMemory.SampleRate = TimeSpan.FromSeconds(5);
            diagConfig.PerformanceCounters.DataSources.Add(pccMemory);


            diagConfig.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromSeconds(15);

            //diagConfig.WindowsEventLog.DataSources.Add("System!*");

            DiagnosticMonitor.Start("DiagnosticsConnectionString", diagConfig);
            
            System.Diagnostics.Trace.TraceInformation("OnStart Complete");

            /*
            String myRoleInstanceName = RoleEnvironment.CurrentRoleInstance.Id;
            System.Diagnostics.Trace.TraceInformation("TraceInformation:" + myRoleInstanceName);
            CrashDumps.EnableCollection(true);
            CrashDumps.EnableCollection(false);
            */
           
           
            RoleEnvironment.Changing += RoleEnvironmentChanging;

            if (_server == null)
                _server = _server = new FtpServer(new AzureFileSystemFactory());

            _server.NewConnection += ServerNewConnection;

            return base.OnStart();
        }

        static void ServerNewConnection(int nId) {
            Trace.WriteLine(String.Format("Connection {0} accepted", nId), "Connection");
        }

        private static void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e) {
       
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange)) {
           
                e.Cancel = true;
            }
        }
    }
}
