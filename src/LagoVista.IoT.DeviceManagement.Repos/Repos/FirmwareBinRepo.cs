using LagoVista.CloudStorage.Storage;
using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Validation;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class FirmwareBinRepo : CloudFileStorage
    {
        public FirmwareBinRepo(IAdminLogger adminLogger, IConnectionSettings connectionSettings) : base(connectionSettings.AccountId, connectionSettings.AccessKey, adminLogger)
        {
        }

        public async Task<InvokeResult> AddBinAsync(byte[] data, string fileName)
        {

            var containerName = "firmware";
            var result = await AddFileAsync(containerName, fileName, data, "application/octet-stream");
            return result.ToInvokeResult();    
        }

        public async Task<InvokeResult<byte[]>> GetFirmwareBinaryAsync(string fileName)
        {
            var containerName = "firmware";
            return await GetFileAsync(containerName, fileName);
        }
    }
}
