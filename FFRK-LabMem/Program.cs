using System;
using FFRK_LabMem.Config;
using FFRK_LabMem.Config.UI;
using FFRK_LabMem.Data.UI;
using FFRK_LabMem.Machines;
using FFRK_LabMem.Services;
using FFRK_Machines;
using FFRK_Machines.Services;
using System.Linq;
using FFRK_Machines.Services.Adb;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Runtime.ConstrainedExecution;

namespace FFRK_LabMem
{
    class Program
    {
        static void Main(string[] args)
        {

            // Listen for console exit
            ConsoleTasks.ListenForExit(OnConsoleExit);
            ConsoleTasks.DisableQuickEditMode();
            Console.TreatControlCAsInput = true;

            // Enable visual styles in forms
            System.Windows.Forms.Application.EnableVisualStyles();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Get Configuration
            var configFile = (args.Length > 0) ? args[0] : null;
            var config = new ConfigHelper(configFile);

            // Console and Translation
            ColorConsole.Timestamps = config.GetBool("console.timestamps", true);
            ColorConsole.DebugCategories = (ColorConsole.DebugCategory)config.GetInt("console.debugCategories", 0);
            ColorConsole.LogBuffer.Enabled = config.GetBool("console.logging", false);
            ColorConsole.Translate = config.GetBool("console.translate", true);
            ColorConsole.LogBuffer.UpdateFolderOrDefault(config.GetString("console.logFolder", ""));
            ColorConsole.LogBuffer.BufferSize = config.GetInt("console.logBuffer", 10);

            // Config arg switch
            if (args.Contains("-c"))
            {
                ConfigForm.CreateAndShow(config, LabController.Create(config).Result);
                Tray.Hide();
                return;
            }

            // Version check
            var versionCode = Updates.GetVersionCode("jp");
            var versionTitle = String.Format("{0} {1}", Updates.GetName(), versionCode);
            ColorConsole.WriteLine(versionTitle);
            Console.Title = versionTitle;
            if (config.GetBool("updates.checkForUpdates", false))
                _ = Updates.Check(config.GetBool("updates.includePrerelease", false));
            SoulbreakSync.APIKEY = config.GetString("soulbreak.api", "");

            // Hyper-V check
            ColorConsole.Debug(ColorConsole.DebugCategory.Notification, "Hyper-V is " + HyperV.GetHVStatus());

            LabController controller = null;
            try
            {
                // Controller
                controller = LabController.CreateAndStart(config).Result;

                // Instructions for Japanese output (Windows 10 and earlier)
                if (HyperV.GetHVStatus() != "Running" && ColorConsole.Translate == false)
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Japanese output: click treasure chest in top left then Properties and change Font to MS Gothic");
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "See Readme.md on GitHub for more information");
                }

                // Soulbreak Synchronization Prompts
                if (SoulbreakSync.APIKEY.Length < 1)
                    Console.WriteLine("Enter an API Key from https://ffrk.gigaforge.com to begin tracking Soulbreaks.");
                else
                    Console.WriteLine("Soulbreaks will synchronize with https://ffrk.gigaforge.com when you enter the Party menu.");

                // Ad-hoc command loop
                Console.WriteLine("Press 'D' to Disable, 'E' to Enable, 'C' for Config, 'S' for Stats, 'Ctrl+X' to Exit");
                Console.WriteLine("Type ? for help");
                Tyro.Register(controller);

                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.X && key.Modifiers == ConsoleModifiers.Control) break;
                    if (key.Key == ConsoleKey.E) controller.Enable();
                    if (key.Key == ConsoleKey.D) controller.Disable();
                    if (key.Key == ConsoleKey.H) Tray.MinimizeTo(key.Modifiers, controller);
                    if (key.Key == ConsoleKey.C && key.Modifiers == 0) ConfigForm.CreateAndShow(config, controller);
                    if (key.Key == ConsoleKey.C && key.Modifiers == ConsoleModifiers.Control) Console.Clear();
                    if (key.Key == ConsoleKey.S) CountersForm.CreateAndShow(controller);
                    if (key.Key == ConsoleKey.U && key.Modifiers == ConsoleModifiers.Alt) if (Updates.DownloadInstallerAndRun(config.GetBool("updates.includePrerelease", false)).Result) break;
                    if (key.Key == ConsoleKey.O && key.Modifiers == ConsoleModifiers.Alt) controller.AutoDetectOffsets(config);
                    if (key.Key == ConsoleKey.B && key.Modifiers == ConsoleModifiers.Control) Clipboard.CopyProxyBypassToClipboard();
                    if (key.Key == ConsoleKey.R && key.Modifiers == ConsoleModifiers.Alt) controller.ManualFFRKRestart();
                    if (key.Key == ConsoleKey.Q) controller.QuickExplore();
                    if (key.Key >= ConsoleKey.D0 && key.Key <= ConsoleKey.D9) controller.SetRestartCount(int.Parse(key.KeyChar.ToString()));
                    if (key.Key == ConsoleKey.OemMinus) controller.SetRestartCount(-1);
                    if (key.KeyChar == '?') Console.WriteLine(Properties.Resources.HelpString);
                    if (key.Key == ConsoleKey.B) Benchmark.FrameCapture(controller);
                    
                    // This was intended to allow Win11 users to manually run cert install script with Hyper-V ignored, to force-install the cert
                    // It would have skipped the "Press ENTER" prompt and immediately proceeded to activate the bot's functions
                    // This does not work however, so this code is commented out
                    // if (key.Key == ConsoleKey.I)
                    //{
                    //    controller.Disable();
                    //    HyperV.ResetHVStatus();
                    //    controller = LabController.CreateAndStart(config).Result;
                    //    ColorConsole.WriteLine(ConsoleColor.Red, "**PLEASE RESTART LABMEM AFTER CERTIFICATE INSTALL**");
                    //}
                    Tyro.ReadConsole(key);
                }
            }
            catch(Exception ex)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
                ColorConsole.WriteLine("Unhandled exception occured, press any key to exit");
                ColorConsole.LogBuffer.Flush();
                Console.ReadKey();
            }
            finally
            {
                // Stop
                controller?.Stop();
                OnConsoleExit();
            }

        }

        private static void OnConsoleExit()
        {
            // Kill adb option
            if (new ConfigHelper().GetBool("adb.closeOnExit", false)) Adb.KillAdb();

            // Flush buffers
            ColorConsole.LogBuffer.Flush();
            Data.Counters.Flush().Wait();
        }

    }
}
