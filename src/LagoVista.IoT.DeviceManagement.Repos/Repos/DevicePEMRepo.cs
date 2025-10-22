// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3bbd693ec9b65ba47bff8d60c8b3b87615837b74e7fff018d966c98135c607e4
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using System.Threading.Tasks;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.CloudStorage.Storage;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    //TODO: Eventually we need to have this talk to remote devices for cloud version

    public class DevicePEMRepo : TableStorageBase<PEMIndexDTO>, IDevicePEMRepo
    {
        IAdminLogger _adminLogger;

        public DevicePEMRepo(IAdminLogger adminLogger) : base(adminLogger)
        {
            _adminLogger = adminLogger;
        }

        public async Task<string> GetPEMAsync(DeviceRepository deviceRepo, string partitionKey, string rowKey)
        {
            SetConnection(deviceRepo.PEMStorageSettings.AccountId, deviceRepo.PEMStorageSettings.AccountId);

            var result = await GetAsync(partitionKey, rowKey);
            var pemResult = result.ToPEM();
            if(pemResult.Successful) 
                return pemResult.Result;

            _adminLogger.AddError("DevicePEMRepo_GetPEMAsync", pemResult.Errors.First().Message);
            return null;
        }

        public async Task<ListResponse<PEMIndex>> GetPEMIndexForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            SetConnection(deviceRepo.PEMStorageSettings.AccountId, deviceRepo.PEMStorageSettings.AccountId);

            var result = await GetByParitionIdAsync(deviceId, request.PageSize, request.PageIndex * request.PageSize);
            return ListResponse<PEMIndex>.Create(result.Select(pem => pem.ToPEMIndex()).ToList(), request);
        }

        public async Task<ListResponse<PEMIndex>> GetPEMIndexForErrorReasonAsync(DeviceRepository deviceRepo, string errorReason, ListRequest request)
        {
            SetConnection(deviceRepo.PEMStorageSettings.AccountId, deviceRepo.PEMStorageSettings.AccountId);
            var result = await GetPagedResultsAsync(errorReason, request);
            var records = result.Model.Select(result => result.ToPEMIndex()).ToList();
            return ListResponse<PEMIndex>.Create(records, result); 
        }
    }
}
