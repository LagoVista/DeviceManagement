using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceMediaManager : ManagerBase, IDeviceMediaManager
    {
        IDeviceMediaItemRepo _mediaItemRepo;
        IDeviceMediaRepo _mediaRepo;
        IAdminLogger _adminLogger;
        IDeviceManager _deviceManager;

        //TODO: Need to add remote connector for this.

        public DeviceMediaManager(IDeviceMediaItemRepo mediaItemRepo, IDeviceMediaRepo mediaRepo, IDeviceManager deviceManager, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) :
            base(logger, appConfig, depmanager, security)
        {
            _adminLogger = logger;
            _mediaRepo = mediaRepo;
            _mediaItemRepo = mediaItemRepo;
            _deviceManager = deviceManager;
        }

        public async Task<ListResponse<DeviceMedia>> GetMediaItemsForDeviceAsync(DeviceRepository repo, string deviceId, EntityHeader org, EntityHeader user, ListRequest request)
        {
            /* Ensure user has access to device */
            await _deviceManager.GetDeviceByIdAsync(repo, deviceId, org, user);

            await AuthorizeAsync(user.Id, org.Id, "getMediaItems", deviceId);

            return await _mediaRepo.GetMediaItemsForDeviceAsync(repo, deviceId, request);
        }

        public async Task<MediaItemResponse> GetMediaItemAsync(DeviceRepository repo, string deviceId, string itemId, EntityHeader org, EntityHeader user)
        {
            /* Ensure user has access to device */
            await _deviceManager.GetDeviceByIdAsync(repo, deviceId, org, user);

            var item = await _mediaRepo.GetMediaItemAsync(repo, deviceId, itemId);
            var image = await _mediaItemRepo.GetMediaItemAsync(repo, deviceId, itemId);

            await AuthorizeAsync(user, org, "getMediaItem", $"{{deviceId:'{deviceId}',mediaItemId:'{itemId}'}}");

            return new MediaItemResponse()
            {
                ContentType = item.ContentType,
                FileType = item.FileType,
                ImageBytes = image
            };
        }

        public async Task<InvokeResult> DeleteMediaItemAsync(DeviceRepository repo, string deviceId, string itemId, EntityHeader org, EntityHeader user)
        {
            /* Ensure user has access to device */
            await _deviceManager.GetDeviceByIdAsync(repo, deviceId, org, user);

            await AuthorizeAsync(user.Id, org.Id, "deleteMediaItem", itemId);

            await _mediaItemRepo.DeleteMediaItemAsync(repo, deviceId, itemId);
            await _mediaRepo.DeleteMediaItemAsync(repo, deviceId, itemId);

            return InvokeResult.Success;
        }
    }
}
