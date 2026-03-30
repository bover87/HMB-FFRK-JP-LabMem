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
        public const string RUNNING = "Running";
        public const string STOPPED = "Stopped";
        public const string PAUSED = "Paused";
        public const string STOPPING = "Stopping";
        public const string STARTING = "Starting";
        public const string NOT_DETECTED = "not detected";

        static String HVStatus;
        private static String CheckHVStatus()
        {
            ServiceController sc = new ServiceController("HvHost");

            switch (sc.Status)
            {
                case ServiceControllerStatus.Running:
                    return RUNNING;
                case ServiceControllerStatus.Stopped:
                    return STOPPED;
                case ServiceControllerStatus.Paused:
                    return PAUSED;
                case ServiceControllerStatus.StopPending:
                    return STOPPING;
                case ServiceControllerStatus.StartPending:
                    return STARTING;
                default:
                    return NOT_DETECTED;
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
