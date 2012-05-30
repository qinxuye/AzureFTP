using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using FTPServerRole.AzureMngmntRESTLib;

namespace FTPServerRole
{
    class InstanceCountDecider
    {
        private const int MAX_INSTANCE_COUNT = 2;
        private enum ScaleAction { ScaleOut, ScaleIn };

        private static double getCPUAverage()
        {
            return (from d in PerfCountDataHandler.GetPerfCountByType(@"\Processor(_Total)\% Processor Time")
                    select d.CounterValue).Average();
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

            // Put strategy here to decide.
            ActionOnInstanceCount(ScaleAction.ScaleOut);
        }
    }
}
