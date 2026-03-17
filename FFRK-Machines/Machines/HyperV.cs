using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_Machines.Services
{
    public class HyperV
    {
        static String HVStatus;
        private static String CheckHVStatus()
        {
            ServiceController sc = new ServiceController("HvHost");

            switch (sc.Status)
            {
                case ServiceControllerStatus.Running:
                    return "Running";
                case ServiceControllerStatus.Stopped:
                    return "Stopped";
                case ServiceControllerStatus.Paused:
                    return "Paused";
                case ServiceControllerStatus.StopPending:
                    return "Stopping";
                case ServiceControllerStatus.StartPending:
                    return "Starting";
                default:
                    return "not detected";
            }
        }
        public static String GetHVStatus()
        {
            if (HVStatus == null)
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Notification, "Checking for process HvHost...");
                HVStatus = CheckHVStatus();
            }      
            return HVStatus;
        }

        // This was intended to reset Hyper-V status in the bot to null, hopefully allowing the certificate to be installed manually
        // This did not work, however, so this method is not used
        //public static String ResetHVStatus()
        //{
        //    HVStatus = null;
        //    return "Hyper-V status reset";
        //}
    }
}
