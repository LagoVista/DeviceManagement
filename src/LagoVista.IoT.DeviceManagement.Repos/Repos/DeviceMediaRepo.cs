// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 5b7afd62fe78f1bf102cf3ca487ea53efb02a2612e9b2f9b06733caebf14cc7f
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.Interfaces;
using LagoVista.CloudStorage.Storage;
using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceMediaRepo : IDeviceMediaRepo
    {
        ILogger _logger;

        ICloudFileStorageClient _storageClient;
        public DeviceMediaRepo(IAdminLogger adminLogger, ICloudFileStorageClient storageClient) 
        {
            _logger = adminLogger;
            _storageClient = storageClient;
        } 

        public async Task<InvokeResult> AddMediaAsync(DeviceRepository repo, byte[] data, string fileName, string contentType)
        {
        
            var containerName = repo.GetDeviceMediaStorageName();

            var result = await _storageClient.AddFileAsync(containerName, fileName, data, contentType);
            return result.ToInvokeResult();
        }

        public async Task<InvokeResult> DeleteMediaAsync(DeviceRepository repo, string fileName)
        {
            var containerName = repo.GetDeviceMediaStorageName();
            return await _storageClient.DeleteFileAsync(containerName, fileName);
        }

        public async Task<InvokeResult<byte[]>> GetMediaAsync(DeviceRepository repo, string fileName)
        {
            var containerName = repo.GetDeviceMediaStorageName();
            return await _storageClient.GetFileAsync(containerName, fileName);
        }
    }
}
