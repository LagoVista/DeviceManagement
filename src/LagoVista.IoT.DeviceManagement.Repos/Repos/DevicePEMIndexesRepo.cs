using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DevicePEMIndexesRepo : LagoVista.CloudStorage.Storage.TableStorageBase<DevicePEMIndex>, IDevicePEMIndexesRepo
    {
        protected override string GetTableName()
        {
            /* The actual table name is PEMIndex but that defined in a dependent project */
            return "PEMIndex";
        }

        public DevicePEMIndexesRepo(IAdminLogger logger) : base(logger)
        {

        }

        public Task<ListResponse<DevicePEMIndex>> GetPEMIndexForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            SetConnection(deviceRepo.PEMStorageSettings.AccountId, deviceRepo.PEMStorageSettings.AccessKey);
            return GetPagedResultsAsync(deviceId, request);
        }
    }
}
