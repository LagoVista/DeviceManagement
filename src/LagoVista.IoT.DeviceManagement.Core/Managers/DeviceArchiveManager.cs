using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.Managers;
using LagoVista.Core.Interfaces;
using System.Threading.Tasks;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System.Collections.Generic;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceArchiveManager : ManagerBase, IDeviceArchiveManager
    {
        IDeviceArchiveRepo _archiveRepo;

        public DeviceArchiveManager(IDeviceArchiveRepo archiveRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _archiveRepo = archiveRepo;
        }

        public async Task<InvokeResult> AddArchiveAsync(DeviceRepository deviceRepo, DeviceArchive logEntry, EntityHeader org, EntityHeader user)
        {
            await _archiveRepo.AddArchiveAsync(deviceRepo, logEntry);
            return InvokeResult.Success;
        }       

        public async Task<ListResponse<List<object>>> GetDeviceArchivesAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {            
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceArchive), LagoVista.Core.Validation.Actions.Read);
            return await _archiveRepo.GetForDateRangeAsync(deviceRepo, deviceId, listRequest);
        }
    }
}
