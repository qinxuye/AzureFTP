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
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace FTPServerRole {
    public class WorkerRole : RoleEntryPoint {
        private FtpServer _server;

        public override void Run() {

            Trace.WriteLine("FTPRole entry point called", "Information");

            while (true) {
                if (_server.Started) {
                    Thread.Sleep(10000);
                    Trace.WriteLine("Server is alive.", "Information");
                }
                else {
                    _server.Start();
                    Trace.WriteLine("Server starting.", "Control");
                }

            }
        }

        public override bool OnStart() {

  
            ServicePointManager.DefaultConnectionLimit = 12;

            DiagnosticMonitor.Start("DiagnosticsConnectionString");

     
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
