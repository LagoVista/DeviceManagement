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
using LagoVista.IoT.DeviceManagement.Core.Interfaces;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceArchiveManager : ManagerBase, IDeviceArchiveManager
    {
        IDeviceArchiveRepo _archiveRepo;
        IDeviceArchiveConnector _archiveConnector;

        public DeviceArchiveManager(IDeviceArchiveRepo archiveRepo, IDeviceArchiveConnector archiveConnector, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _archiveRepo = archiveRepo;
            _archiveConnector = archiveConnector;
        }

        public async Task<InvokeResult> AddArchiveAsync(DeviceRepository deviceRepo, DeviceArchive logEntry, EntityHeader org, EntityHeader user)
        {
            if(deviceRepo.RepositoryType.Value == RepositoryTypes.Distributed)
            {
                await _archiveConnector.AddArchiveAsync(deviceRepo.Instance.Id, logEntry, org, user);
            }
            else
            {
                await _archiveRepo.AddArchiveAsync(deviceRepo, logEntry);
            }
            return InvokeResult.Success;
        }       

        public async Task<ListResponse<List<object>>> GetDeviceArchivesAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceArchive), LagoVista.Core.Validation.Actions.Read);
            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Distributed)
            {
                return await _archiveConnector.GetForDateRangeAsync(deviceRepo.Instance.Id, deviceId, listRequest, org, user);
            }
            else
            {
                return await _archiveRepo.GetForDateRangeAsync(deviceRepo, deviceId, listRequest);
            }
        }
    }
}
