using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceMediaManager : ManagerBase, IDeviceMediaManager
    {
        private readonly IDeviceMediaRepo _defaultMediaRepo;
        private readonly IDeviceMediaItemRepo _defaultMediaItemRepo;
        private readonly IDeviceManager _deviceManager;
        private readonly IProxyFactory _proxyFactory;

        public IDeviceMediaRepo GetMediaRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local ?
                _proxyFactory.Create<IDeviceMediaRepo>(
                    new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance.Id
                    }) :
                _defaultMediaRepo;
        }

        public IDeviceMediaItemRepo GetMediaItemRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local ?
                _proxyFactory.Create<IDeviceMediaItemRepo>(
                    new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance.Id
                    }) :
                _defaultMediaItemRepo;
        }

        public IDeviceMediaRepoRemote GetMediaRepoRemote(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local ?
                _proxyFactory.Create<IDeviceMediaRepo>(
                    new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance.Id
                    }) :
                _defaultMediaRepo;
        }

        public IDeviceMediaItemRepoRemote GetMediaItemRepoRemote(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local ?
                _proxyFactory.Create<IDeviceMediaItemRepo>(
                    new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance.Id
                    }) :
                _defaultMediaItemRepo;
        }

        public DeviceMediaManager(IDeviceMediaRepo mediaRepo, IDeviceMediaItemRepo mediaItemRepo, IDeviceManager deviceManager,
            IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security,
            IProxyFactory proxyFactory) :
            base(logger, appConfig, depmanager, security)
        {
            _defaultMediaRepo = mediaRepo;
            _defaultMediaItemRepo = mediaItemRepo;
            _deviceManager = deviceManager;
            _proxyFactory = proxyFactory;
        }

        public async Task<ListResponse<DeviceMedia>> GetMediaItemsForDeviceAsync(DeviceRepository repo, string deviceId, EntityHeader org, EntityHeader user, ListRequest request)
        {
            /* Ensure user has access to device */
            await _deviceManager.GetDeviceByIdAsync(repo, deviceId, org, user);

            await AuthorizeAsync(user.Id, org.Id, "getMediaItems", deviceId);

            return await GetMediaItemRepo(repo).GetMediaItemsForDeviceAsync(repo, deviceId, request);
        }

        public async Task<MediaItemResponse> GetMediaItemAsync(DeviceRepository repo, string deviceId, string itemId, EntityHeader org, EntityHeader user)
        {
            /* Ensure user has access to device */
            await _deviceManager.GetDeviceByIdAsync(repo, deviceId, org, user);

            var item = await GetMediaItemRepo(repo).GetMediaItemAsync(repo, deviceId, itemId);
            var imageRequestResponse = await GetMediaRepo(repo).GetMediaAsync(repo, item.FileName);

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

            if (contentType.ToLower().Contains("gif"))
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

            await GetMediaItemRepoRemote(repo).StoreMediaItemAsync(repo, new DeviceMedia()
            {
                ContentType = contentType,
                DeviceId = deviceId,
                FileName = fileName,
                TimeStamp = DateTime.UtcNow.ToJSONString()
            });

            var bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, (int)stream.Length);
            await GetMediaRepoRemote(repo).AddMediaAsync(repo, bytes, fileName, contentType);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeleteMediaItemAsync(DeviceRepository repo, string deviceId, string itemId, EntityHeader org, EntityHeader user)
        {
            /* Ensure user has access to device */
            await _deviceManager.GetDeviceByIdAsync(repo, deviceId, org, user);

            await AuthorizeAsync(user.Id, org.Id, "deleteMediaItem", itemId);

            var item = await GetMediaItemRepo(repo).GetMediaItemAsync(repo, deviceId, itemId);

            await GetMediaRepo(repo).DeleteMediaAsync(repo, item.FileName);
            await GetMediaItemRepo(repo).DeleteMediaItemAsync(repo, deviceId, itemId);

            return InvokeResult.Success;
        }
    }
}
