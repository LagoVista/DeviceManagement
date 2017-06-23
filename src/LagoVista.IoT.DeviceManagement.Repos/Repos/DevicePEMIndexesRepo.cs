using LagoVista.IoT.DeviceManagement.Core.Repos;
using System;
using System.Collections.Generic;
using System.Text;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DevicePEMIndexesRepo : LagoVista.CloudStorage.Storage.TableStorageBase<DevicePEMIndex>, IDevicePEMIndexesRepo
    {
        protected override string GetTableName()
        {
            /* The actual table name is PEMIndex but that defined in a dependent project */
            return "PEMIndex";
        }

        public DevicePEMIndexesRepo(IDeviceManagementSettings settings, IAdminLogger logger) : base(settings.PEMStorage.AccountId, settings.PEMStorage.AccessKey, logger)
        {

        }

        public Task<IEnumerable<Core.Models.DevicePEMIndex>> GetPEMIndexForDeviceAsync(string deviceId, int take, string dateStampAfter)
        {
            return GetByParitionIdAsync(deviceId);
        }
    }
}
