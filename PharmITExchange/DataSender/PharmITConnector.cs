using System;
using Newtonsoft.Json;
using PharmITExchange.Common;
using RestSharp;

namespace PharmITExchange
{
    public class PharmITConnector
    {
        readonly RestClient Client;
        readonly RestRequest Request;
        readonly string ApiUrl;
        readonly string ApiAuthString;

        public PharmITConnector(string apiUrl, string apiAuthString)
        {
            Client = new RestClient();
            Request = new RestRequest();

            ApiUrl = apiUrl;
            ApiAuthString = apiAuthString;

            SetRequestHeaders();
        }

        private void SetRequestHeaders()
        {
            Request.Parameters.Clear();
            Request.AddHeader("Cache-Control", "no-cache");
            Request.AddHeader("Authorization", $"{ApiAuthString}");
            Request.AddHeader("Content-Type", "application/json");
        }

        private string GetFileID(DateTime date)
        {
            Client.BaseUrl = new Uri($"{ApiUrl}/loadhistories");

            SetRequestHeaders();
            Request.Method = Method.POST;
            Request.AddJsonBody(new { DateSale = date.ToString("yyyyMMdd") });
            IRestResponse response = Client.Execute(Request);

            if (!response.IsSuccessful)
            {
                throw new PharmITException($"{(int)response.StatusCode} - {response.StatusDescription}");
            }

            Logger.Log.Info($"Response Content: {response.Content}");

            return response.Content;

        }

        private void SendDataPortion(string data)
        {
            Client.BaseUrl = new Uri($"{ApiUrl}/values");
            SetRequestHeaders();
            Request.Method = Method.POST;
            Request.AddJsonBody(data);
            IRestResponse response = Client.Execute(Request);

            if (!response.IsSuccessful)
            {
                throw new PharmITException($"{(int)response.StatusCode} - {response.StatusDescription}");
            }

            Logger.Log.Info($"Data was sent successfully!");
        }

        public string GetMissedData()
        {
            Client.BaseUrl = new Uri($"{ApiUrl}/values");
            SetRequestHeaders();
            Request.Method = Method.GET;
            IRestResponse response = Client.Execute(Request);

            if (!response.IsSuccessful)
            {
                throw new PharmITException($"{(int)response.StatusCode} - {response.StatusDescription}");
            }

            Logger.Log.Info($"Missed data was received successfully!");

            return response.Content;
        }

        public void SendData(DataFile dataFile)
        {
            string fileId = GetFileID(dataFile.Date);
            int rowCount = dataFile.Count;


            foreach (var page in dataFile.Pages)
            {
                foreach (var obj in page.Page)
                {
                    obj.FileId = fileId;
                }

                string data = JsonConvert.SerializeObject(page.Page);

                SendDataPortion(data);
            }

            CommitData(fileId, rowCount);
        }

        private void CommitData(string fileId, int rowCount)
        {
            bool result = false;

            Client.BaseUrl = new Uri($"{ApiUrl}/loadhistories/{fileId}_{rowCount}");
            SetRequestHeaders();
            Request.Method = Method.PUT;
            IRestResponse response = Client.Execute(Request);

            if (!response.IsSuccessful)
            {
                throw new PharmITException($"{(int)response.StatusCode} - {response.StatusDescription}");
            }

            int rowCountCommited = 0;

            if (int.TryParse(response.Content, out rowCountCommited))
            {
                result = rowCount - rowCountCommited < 5;
            }

            if (result)
            {
                Logger.
                    Log.
                    Info($"Data commited with success! Actual row count: {rowCount}, row count commited: {rowCountCommited}");
            }
            else
            {
                Logger.
                    Log.
                    Warn($"Data must be resended! Actual row count: {rowCount}, row count commited: {rowCountCommited}");
            }

        }

    }
}
