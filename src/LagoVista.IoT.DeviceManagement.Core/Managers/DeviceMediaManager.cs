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
using LagoVista.Core;

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceMediaManager : ManagerBase, IDeviceMediaManager
    {
        IDeviceMediaRepo _mediaRepo;
        IDeviceMediaItemRepo _mediaItemRepo;
        IAdminLogger _adminLogger;
        IDeviceManager _deviceManager;

        //TODO: Need to add remote connector for this.

        public DeviceMediaManager(IDeviceMediaRepo mediaRepo, IDeviceMediaItemRepo mediaItemRepo, IDeviceManager deviceManager, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) :
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

            return await _mediaItemRepo.GetMediaItemsForDeviceAsync(repo, deviceId, request);
        }

        public async Task<MediaItemResponse> GetMediaItemAsync(DeviceRepository repo, string deviceId, string itemId, EntityHeader org, EntityHeader user)
        {
            /* Ensure user has access to device */
            await _deviceManager.GetDeviceByIdAsync(repo, deviceId, org, user);

            var item = await _mediaItemRepo.GetMediaItemAsync(repo, deviceId, itemId);
            var imageRequestResponse = await _mediaRepo.GetMediaAsync(repo, item.FileName );

            await AuthorizeAsync(user, org, "getMediaItem", $"{{deviceId:'{deviceId}',mediaItemId:'{itemId}'}}");

            return new MediaItemResponse()
            {
                ContentType = item.ContentType,
                FileName = item.FileName,
                ImageBytes = imageRequestResponse.Result
            };
        }

        public async Task<InvokeResult> AddMediaItemAsync(DeviceRepository repo, string deviceId, Stream stream, string contentType, EntityHeader org, EntityHeader user)
        {
            await _deviceManager.GetDeviceByIdAsync(repo, deviceId, org, user);

            var itemId = Guid.NewGuid().ToId();

            await AuthorizeAsync(user, org, "addMediaItem", $"{{deviceId:'{deviceId}',mediaItemId:'{itemId}'}}");

            //TODO: This code is cut-and-paste reuse in the file DeviceMediaStorage in the IoT Project, as well as the models for Device Media, probably should refactor, but it's simple enough
            var fileName = $"{itemId}.media";

            if(contentType.ToLower().Contains("gif"))
            {
                fileName = $"{itemId}.gif";
            }
            else if (contentType.ToLower().Contains("png"))
            {
                fileName = $"{itemId}.png";
            }
            else if (contentType.ToLower().Contains("jpg"))
            {
                fileName = $"{itemId}.jpg";
            }
            else if (contentType.ToLower().Contains("jpeg"))
            {
                fileName = $"{itemId}.jpeg";
            }

            await _mediaItemRepo.StoreMediaItemAsync(repo, new DeviceMedia()
            {
                ContentType = contentType,
                DeviceId = deviceId,
                FileName = fileName,
                TimeStamp = DateTime.UtcNow.ToJSONString()
            });

            await _mediaRepo.AddMediaAsync(repo, stream, fileName, contentType);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeleteMediaItemAsync(DeviceRepository repo, string deviceId, string itemId, EntityHeader org, EntityHeader user)
        {
            /* Ensure user has access to device */
            await _deviceManager.GetDeviceByIdAsync(repo, deviceId, org, user);

            await AuthorizeAsync(user.Id, org.Id, "deleteMediaItem", itemId);

            var item = await _mediaItemRepo.GetMediaItemAsync(repo, deviceId, itemId);

            await _mediaRepo.DeleteMediaAsync(repo, item.FileName);
            await _mediaItemRepo.DeleteMediaItemAsync(repo, deviceId, itemId);

            return InvokeResult.Success;
        }
    }
}
