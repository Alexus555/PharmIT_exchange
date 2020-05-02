using System;
using Newtonsoft.Json;
using System.IO;

namespace PharmITExchange
{
    public class PharmITDataSender
    {
        private readonly string ApiUrl;
        private readonly string ApiAuthString;

        public PharmITDataSender(string apiUrl, string apiAuthString)
        {
            ApiUrl = apiUrl;
            ApiAuthString = apiAuthString;
        }


        private bool SendFile(string file)
        {

            bool result = false;

            Logger.Log.Info($"Read file {file}");


            using (StreamReader sr = new StreamReader(file))
            {

                string jsonData = sr.ReadToEnd();

                DataFile dataFile = JsonConvert.DeserializeObject<DataFile>(jsonData);


                if (dataFile is null)
                {
                    throw new Exception("No data in file");
                }

                PharmITConnector connector = new PharmITConnector(ApiUrl, ApiAuthString);

                connector.SendData(dataFile);

            }

            
            return result;
        }

        public void SendFiles(string filesMask)
        {

            var files = Directory.GetFiles(filesMask);

            SendFiles(files);


        }

        public void SendFiles(string[] files)
        {

            foreach (var file in files)
            {
                try
                {
                    if (SendFile(file))
                    {
                        Logger.Log.Info($"File {file} was sent successfully!");
                    }
                }
                catch (Exception e)
                {
                    Logger.Log.Error($"File {file} wasn't sent due to error: {e.Message}");
                }
            }


        }

        public string GetMissedData()
        {
            PharmITConnector connector = new PharmITConnector(ApiUrl, ApiAuthString);
            return connector.GetMissedData();
        }
    }
}
