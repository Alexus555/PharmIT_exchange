using System;
using Newtonsoft.Json;
using System.IO;
using PharmITExchange.Common;

namespace PharmITExchange
{
    static class Program
    {
        static void Main(string[] args)
        {
            Logger.InitLogger();


            Logger.Log.Info("Start application");

            string appUri = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

            UriBuilder uri = new UriBuilder(appUri);
            string appPath =
                Path.GetDirectoryName(
                    Uri.UnescapeDataString(uri.Path));
            string dataPath = appPath + @"\Data\";
            string backupPath = appPath + @"\Backup\";

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

                string filesMask = "1c*.json";

                var files = Directory.GetFiles(dataPath, filesMask);

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
                    var backupFile = new FileInfo(backupPath + file.Name);

                    if (backupFile.Exists)
                    {
                        backupFile.Delete();
                    }

                    file.MoveTo(backupPath + file.Name);
                }

                var missedData = dataSender.GetMissedData();
                var missedDataFile = dataPath + @"MissedData.json";

                using (StreamWriter sr = new StreamWriter(missedDataFile, false))
                {

                    sr.Write(missedData);

                    sr.Flush();

                }

            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message, e);
            }

            Logger.Log.Info("End application");
        }

        private static void GetSettings(string appPath, out string apiUrl, out string apiAuthString)
        {
            FileInfo settingsFile = new FileInfo(appPath + @"\settings.json");
            if (!settingsFile.Exists)
            {
                throw new PharmITException("File 'settings.json' was not found!");
            }

            var settingsDefinition = new { ApiUrl = "", ApiAuthString = "" };

            using (StreamReader sr = new StreamReader(settingsFile.FullName))
            {

                string jsonData = sr.ReadToEnd();
                var settings = JsonConvert.DeserializeAnonymousType(jsonData, settingsDefinition);

                if (settings is null)
                {
                    throw new PharmITException("Settings was not found!");
                }

                apiUrl = settings.ApiUrl;
                apiAuthString = settings.ApiAuthString;
            }
        }

    }
}
