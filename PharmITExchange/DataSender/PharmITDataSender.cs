using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace PharmITExchange
{
    public class PharmITDataSender
    {
        string ApiUrl;
        string ApiAuthString;

        public PharmITDataSender(string apiUrl, string apiAuthString)
        {
            ApiUrl = apiUrl;
            ApiAuthString = apiAuthString;
        }

        private IEnumerable<FileInfo> GetFiles(string filesMask)
        {
            List<FileInfo> files = new List<FileInfo>();

            var fileNames = Directory.GetFiles(filesMask);
            foreach (var fileName in fileNames)
            {
                files.Add(new FileInfo(fileName));
            }

            return files;
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
                        Logger.Log.Info($"File {file} was sent successfully!");
                }
                catch (Exception e)
                {
                    Logger.Log.Warn($"File {file} wasn't sent due to error: {e.Message}");
                }
            }


        }
    }
}
