using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.IO;

namespace PharmITExchange
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.InitLogger();


            Logger.Log.Info("Start application");

            string appUri = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

            UriBuilder uri = new UriBuilder(appUri);
            string appPath = Uri.UnescapeDataString(uri.Path);
            string dataPath = Path.GetDirectoryName(appPath) + @"\Data\";
            string backupPath = Path.GetDirectoryName(appPath) + @"\Backup\";

            string apiUrl = "";
            string apiAuthString = "";

            try
            {
                GetSettings(appPath, out apiUrl, out apiAuthString);

                DirectoryInfo dirInfo = new DirectoryInfo(dataPath);

                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }

                string filesMask = dataPath + "1c*.json";

                var files = Directory.GetFiles(filesMask);

                var dataSender = new PharmITDataSender(apiUrl, apiAuthString);

                dataSender.SendFiles(files);

                DirectoryInfo backupInfo = new DirectoryInfo(backupPath);

                if (!backupInfo.Exists)
                {
                    backupInfo.Create();
                }

                foreach (var fileName in files)
                {
                    var file = new FileInfo(fileName);
                    file.MoveTo(backupPath + file.Name);
                }

            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message, e);
            }

            //Console.ReadKey();

            Logger.Log.Info("End application");
        }

        private static void GetSettings(string appPath, out string apiUrl, out string apiAuthString)
        {
            FileInfo settingsFile = new FileInfo(appPath + @"\settings.json");
            if (!settingsFile.Exists)
                throw new Exception("File 'settings.json' was not found!");

            var settingsDefinition = new { ApiUrl = "", ApiAuthString = "" };

            using (StreamReader sr = new StreamReader(settingsFile.FullName))
            {

                string jsonData = sr.ReadToEnd();
                var settings = JsonConvert.DeserializeAnonymousType(jsonData, settingsDefinition);

                if (settings is null)
                {
                    throw new Exception("Settings was not found!");
                }

                apiUrl = settings.ApiUrl;
                apiAuthString = settings.ApiAuthString;
            }
        }

    }
}
