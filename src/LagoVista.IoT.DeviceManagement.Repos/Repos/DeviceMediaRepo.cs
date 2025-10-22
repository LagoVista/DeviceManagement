// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 5b7afd62fe78f1bf102cf3ca487ea53efb02a2612e9b2f9b06733caebf14cc7f
// IndexVersion: 0
// --- END CODE INDEX META ---
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
    public class DeviceMediaRepo : CloudFileStorage, IDeviceMediaRepo
    {
        ILogger _logger;

        public DeviceMediaRepo(IAdminLogger adminLogger) : base(adminLogger)
        {
            _logger = adminLogger;
        } 

        public async Task<InvokeResult> AddMediaAsync(DeviceRepository repo, byte[] data, string fileName, string contentType)
        {
            InitConnectionSettings(repo.DeviceArchiveStorageSettings.AccountId, repo.DeviceArchiveStorageSettings.AccessKey);

            var containerName = repo.GetDeviceMediaStorageName();

            var result = await AddFileAsync(containerName, fileName, data, contentType);
            return result.ToInvokeResult();
        }

        public async Task<InvokeResult> DeleteMediaAsync(DeviceRepository repo, string fileName)
        {
            InitConnectionSettings(repo.DeviceArchiveStorageSettings.AccountId, repo.DeviceArchiveStorageSettings.AccessKey);

            var containerName = repo.GetDeviceMediaStorageName();
            return await DeleteFileAsync(containerName, fileName);
        }

        public async Task<InvokeResult<byte[]>> GetMediaAsync(DeviceRepository repo, string fileName)
        {
            InitConnectionSettings(repo.DeviceArchiveStorageSettings.AccountId, repo.DeviceArchiveStorageSettings.AccessKey);

            var containerName = repo.GetDeviceMediaStorageName();
            return await GetFileAsync(containerName, fileName);
        }
    }
}
