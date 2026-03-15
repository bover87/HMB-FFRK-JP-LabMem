using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FFRK_Machines;
using FFRK_Machines.Services.Adb;
using Newtonsoft.Json.Linq;
using Semver;
using Syroot.Windows.IO;

namespace FFRK_LabMem.Services
{
    class SoulbreakSync
    {

        private String Endpoint { get; set; }
        private Boolean IncludePreRelease { get; set; }
        private HttpClient httpClient;
        private const String WEB_URL = "https://ffrk.gigaforge.com/ffrk_sync.php";
        public static string APIKEY = "";
        private const string GITHUB_USER = "gigaforge";
        private const string GITHUB_REPO = "FFRK-LabMem-SBTracker";
        private static List<Int64> ownedSoulbreaks = new List<Int64>();

        public SoulbreakSync()
        {
            this.Endpoint = WEB_URL;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", String.Format("{0} {1}", GetName(), GetVersionCode("")));
        }

        public static String GetVersionCode(String preRelease = "jp")
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var suffix = (String.IsNullOrEmpty(preRelease)) ? "" : "-" + preRelease;
            return string.Format("v{0}.{1}.{2}{3}", version.Major, version.Minor, version.Build, suffix);
        }

        public static String GetName()
        {
            return Assembly.GetExecutingAssembly().GetName().Name;
        }

        public static async Task<bool> Sync(string soulbreaks)
        {
            string[] sbs = soulbreaks.Split(',');
            string newSoulbreaks = "0";
            Int64 sbCheck;
            foreach (var sb in sbs)
            {
                sbCheck = Int64.Parse(sb);
                if (ownedSoulbreaks.Contains(sbCheck))
                {
                    continue;
                }
                else
                {
                    newSoulbreaks += "," + sb;
                    ownedSoulbreaks.Add(sbCheck);
                }
            }

            if (APIKEY.Length > 1 && newSoulbreaks != "0")
            {
                var checker = new SoulbreakSync();
                var values = new Dictionary<string, string>
                {
                    { "sb_list", soulbreaks },
                    { "api_key", APIKEY }
                };
                var content = new FormUrlEncodedContent(values);
                var response = await checker.httpClient.PostAsync(WEB_URL, content);
                var result = response.Content.ReadAsStringAsync().Result;
                if (result != "")
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "{0}", response.Content.ReadAsStringAsync().Result);
            }

            return true;
        }


    }
}
