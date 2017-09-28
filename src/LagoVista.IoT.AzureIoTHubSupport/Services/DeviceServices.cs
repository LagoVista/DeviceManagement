using LagoVista.IoT.AzureIoTHubSupport.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.AzureIoTHubSupport.Services
{
    public class DeviceServices
    {
        private string _iotHubName;

        const string API_VERSION = "2016-11-14";

        private AuthenticationHeaderValue _authHeader;

        public DeviceServices(string keyName, string key, string iotHubName, int daysTTL = 1)
        {
            var sasGenerator = new Utils.SharedAccessSignatureBuilder()
            {
                KeyName = keyName,
                Key = key,
                Target = $"{iotHubName}.azure-devices.net",
                TimeToLive = TimeSpan.FromDays(daysTTL)
            };

            _authHeader = sasGenerator.GetAuthHeader();
            _iotHubName = iotHubName;
        }

        /// <summary>
        /// Get All Devices for IoT Hub, no pagination, will return up to 1000 devices
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Models.AzureIoTHubDevice>> GetDevicesAsync()
        {
            var uri = $"https://{_iotHubName}.azure-devices.net/devices?api-version={API_VERSION}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = _authHeader;
            var queryContent = new StringContent(""); ;

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ObservableCollection<AzureIoTHubDevice>>(json);
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Create a new Device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<AzureIoTHubDevice> AddDeviceAsync(String deviceId)
        {
            var device = new AzureIoTHubDevice() { DeviceId = deviceId };
            device.Authentication.SymmetricKey.PrimaryKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            device.Authentication.SymmetricKey.SecondaryKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var uri = $"https://{_iotHubName}.azure-devices.net/devices/{deviceId}?api-version={API_VERSION}";
            var json = JsonConvert.SerializeObject(device);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = _authHeader;
            var response = await client.PutAsync(uri, new StringContent(json, Encoding.ASCII, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                var outputJSON = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AzureIoTHubDevice>(outputJSON);
            }
            else
            {
                var failedRespnoseJSON = await response.Content.ReadAsStringAsync();
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<AzureIoTHubDevice> UpdateDeviceAsync(AzureIoTHubDevice device)
        {
            device.Authentication.SymmetricKey.PrimaryKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            device.Authentication.SymmetricKey.SecondaryKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var uri = $"https://{_iotHubName}.azure-devices.net/devices/{device.DeviceId}?api-version={API_VERSION}";
            var json = JsonConvert.SerializeObject(device);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = _authHeader;
            client.DefaultRequestHeaders.IfMatch.Add(new EntityTagHeaderValue($@"""{device.ETag}"""));
            var response = await client.PutAsync(uri, new StringContent(json, Encoding.ASCII, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                var outputJSON = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AzureIoTHubDevice>(outputJSON);
            }
            else
            {
                var failedResponsejSON = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(failedResponsejSON))
                {
                    throw new Exception(response.ReasonPhrase);
                }
                else
                {
                    var error = JsonConvert.DeserializeObject<ErrorMessage>(failedResponsejSON);
                }
            }
        }

        public async Task<AzureIoTHubDevice> GetDeviceAsync(String deviceId)
        {
            var uri = $"https://{_iotHubName}.azure-devices.net/devices/{deviceId}?api-version={API_VERSION}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = _authHeader;
            var queryContent = new StringContent(""); ;

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AzureIoTHubDevice>(json);
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<AzureIoTHubDevice> SearchDevices(String deviceId)
        {
            // Eventual Query Language
            //https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-query-language

            var uri = $"https://{_iotHubName}.azure-devices.net/devices/query?api-version={API_VERSION}";
            var query = "SELECT * FROM DEVICeS";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = _authHeader;
            var queryContent = new StringContent(""); ;

            var response = await client.PutAsync(uri, new StringContent(query, Encoding.ASCII, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AzureIoTHubDevice>(json);
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

    }
}
