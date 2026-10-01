using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Interfaces.Repos;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Interfaces.Repos.Account;
using LagoVista.UserAdmin.Models.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceOwnerManager : IDeviceOwnerManager
    {
        private readonly IDeviceOwnerRepo _deviceOwnerRepo;
        private readonly IDeviceRepositoryManager _deviceRepositoryManager;
        private readonly IDeviceManager _deviceManager;
        private readonly IDeviceTypeRepo _deviceTypeRepo;

        public DeviceOwnerManager(IDeviceOwnerRepo deviceOwnerRepo,
                                  IDeviceRepositoryManager deviceRepositoryManager,
                                  IDeviceManager deviceManager,
                                  IDeviceTypeRepo deviceTypeRepo)
        {
            _deviceOwnerRepo = deviceOwnerRepo ?? throw new ArgumentNullException(nameof(deviceOwnerRepo));
            _deviceRepositoryManager = deviceRepositoryManager ?? throw new ArgumentNullException(nameof(deviceRepositoryManager));
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _deviceTypeRepo = deviceTypeRepo ?? throw new ArgumentNullException(nameof(deviceTypeRepo));
        }

        public async Task<InvokeResult> CreateOwnerAsync(DeviceOwnerUser owner, EntityHeader org, EntityHeader user)
        {
            if (owner == null)
                return InvokeResult.FromError("Device owner is required.");

            if (EntityHeader.IsNullOrEmpty(org))
                return InvokeResult.FromError("Organization is required.");

            if (!EntityHeader.IsNullOrEmpty(owner.OwnerOrganization) && owner.OwnerOrganization.Id != org.Id)
                return InvokeResult.FromError("Device owner organization does not match the current organization.");

            owner.OwnerOrganization = org;
            return await _deviceOwnerRepo.AddUserAsync(owner);
        }

        public Task<InvokeResult<DeviceOwnerUser>> GetOwnerByIdAsync(string ownerId, EntityHeader org, EntityHeader user)
        {
            return GetAuthorizedOwnerAsync(() => _deviceOwnerRepo.FindByIdAsync(ownerId), org);
        }

        public Task<InvokeResult<DeviceOwnerUser>> GetOwnerByPhoneAsync(string phoneNumber, EntityHeader org, EntityHeader user)
        {
            return GetAuthorizedOwnerAsync(() => _deviceOwnerRepo.FindByPhoneNumberAsync(phoneNumber), org);
        }

        public Task<InvokeResult<DeviceOwnerUser>> GetOwnerByEmailAsync(string email, EntityHeader org, EntityHeader user)
        {
            return GetAuthorizedOwnerAsync(() => _deviceOwnerRepo.FindByEmailAsync(email), org);
        }

        public Task<InvokeResult<DeviceOwnerUser>> GetOwnerByNameAsync(string userName, EntityHeader org, EntityHeader user)
        {
            return GetAuthorizedOwnerAsync(() => _deviceOwnerRepo.FindByNameAsync(userName), org);
        }

        public async Task<InvokeResult> UpdateOwnerAsync(DeviceOwnerUser owner, EntityHeader org, EntityHeader user)
        {
            if (owner == null)
                return InvokeResult.FromError("Device owner is required.");

            var existing = await GetOwnerByIdAsync(owner.Id, org, user);
            if (!existing.Successful)
                return existing.ToInvokeResult();

            owner.OwnerOrganization = existing.Result.OwnerOrganization;
            return await _deviceOwnerRepo.UpdateUserAsync(owner);
        }

        public Task<ListResponse<DeviceOwnerUserSummary>> GetOwnersAsync(ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            return _deviceOwnerRepo.GetAllForOrgAsync(org.Id, listRequest);
        }

        public async Task<ListResponse<DeviceOwnerUser>> GetOwnersForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var device = await _deviceManager.GetDeviceByIdAsync(deviceRepo, deviceId, org, user);
            if (!device.Successful)
                return ListResponse<DeviceOwnerUser>.FromInvokeResult(device.ToInvokeResult());

            return await _deviceOwnerRepo.GetDeviceOwnersForDeviceAsync(deviceId, listRequest);
        }

        public async Task<InvokeResult<DeviceOwnerDevices[]>> GetDevicesForOwnerAsync(string ownerId, EntityHeader org, EntityHeader user)
        {
            var owner = await GetOwnerByIdAsync(ownerId, org, user);
            if (!owner.Successful)
                return InvokeResult<DeviceOwnerDevices[]>.FromInvokeResult(owner.ToInvokeResult());

            return InvokeResult<DeviceOwnerDevices[]>.Create(owner.Result.Devices?.ToArray() ?? Array.Empty<DeviceOwnerDevices>());
        }

        public async Task<InvokeResult> AssignOwnerToDeviceAsync(DeviceRepository deviceRepo, string deviceId, string ownerId, bool replaceExisting, EntityHeader org, EntityHeader user)
        {
            var ownerResult = await GetOwnerByIdAsync(ownerId, org, user);
            if (!ownerResult.Successful)
                return ownerResult.ToInvokeResult();

            var deviceResult = await _deviceManager.GetDeviceByIdAsync(deviceRepo, deviceId, org, user);
            if (!deviceResult.Successful)
                return deviceResult.ToInvokeResult();

            var device = deviceResult.Result;
            var owner = ownerResult.Result;

            if (!EntityHeader.IsNullOrEmpty(device.DeviceOwner) && device.DeviceOwner.Id != owner.Id)
            {
                if (!replaceExisting)
                    return InvokeResult.FromError("Device is already owned.");

                var previousOwner = await _deviceOwnerRepo.FindByIdAsync(device.DeviceOwner.Id);
                if (previousOwner != null && previousOwner.OwnerOrganization?.Id == org.Id)
                {
                    var previousAssociation = previousOwner.Devices?.FirstOrDefault(item => item.Device?.Id == device.Id);
                    if (previousAssociation != null)
                        await _deviceOwnerRepo.RemoveOwnedDeviceAsync(org.Id, previousOwner.Id, previousAssociation.Id);
                }
            }

            var association = owner.Devices?.FirstOrDefault(item => item.Device?.Id == device.Id);
            if (association == null)
            {
                var deviceType = await _deviceTypeRepo.GetDeviceTypeAsync(device.DeviceType.Id);
                association = new DeviceOwnerDevices
                {
                    Device = device.ToEntityHeader(),
                    DeviceId = device.DeviceId,
                    DeviceRepository = device.DeviceRepository,
                    Product = deviceType.Product
                };

                await _deviceOwnerRepo.AddOwnedDeviceAsync(org.Id, owner.Id, association);
            }
            else
            {
                association.Device = device.ToEntityHeader();
                association.DeviceId = device.DeviceId;
                association.DeviceRepository = device.DeviceRepository;
                await _deviceOwnerRepo.UpdateOwnedDeviceAsync(org.Id, owner.Id, association);
            }

            device.DeviceOwner = owner.ToEntityHeader();
            return (await _deviceManager.UpdateDeviceAsync(deviceRepo, device, org, user)).ToInvokeResult();
        }

        public async Task<InvokeResult> RemoveOwnerFromDeviceAsync(DeviceRepository deviceRepo, string deviceId, string ownerId, EntityHeader org, EntityHeader user)
        {
            var ownerResult = await GetOwnerByIdAsync(ownerId, org, user);
            if (!ownerResult.Successful)
                return ownerResult.ToInvokeResult();

            var deviceResult = await _deviceManager.GetDeviceByIdAsync(deviceRepo, deviceId, org, user);
            if (!deviceResult.Successful)
                return deviceResult.ToInvokeResult();

            var owner = ownerResult.Result;
            var device = deviceResult.Result;
            var association = owner.Devices?.FirstOrDefault(item => item.Device?.Id == device.Id);
            if (association != null)
                await _deviceOwnerRepo.RemoveOwnedDeviceAsync(org.Id, owner.Id, association.Id);

            if (!EntityHeader.IsNullOrEmpty(device.DeviceOwner) && device.DeviceOwner.Id == owner.Id)
            {
                device.DeviceOwner = null;
                return (await _deviceManager.UpdateDeviceAsync(deviceRepo, device, org, user)).ToInvokeResult();
            }

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeleteOwnerAsync(string ownerId, EntityHeader org, EntityHeader user)
        {
            var ownerResult = await GetOwnerByIdAsync(ownerId, org, user);
            if (!ownerResult.Successful)
                return ownerResult.ToInvokeResult();

            var owner = ownerResult.Result;
            foreach (var association in owner.Devices?.ToArray() ?? Array.Empty<DeviceOwnerDevices>())
            {
                if (association.DeviceRepository == null || association.Device == null)
                    continue;

                var deviceRepo = await _deviceRepositoryManager.GetDeviceRepositoryWithSecretsAsync(association.DeviceRepository.Id, org, user);
                var removeResult = await RemoveOwnerFromDeviceAsync(deviceRepo, association.Device.Id, owner.Id, org, user);
                if (!removeResult.Successful)
                    return removeResult;
            }

            return await _deviceOwnerRepo.DeleteUserAsync(org.Id, owner.Id);
        }

        private async Task<InvokeResult<DeviceOwnerUser>> GetAuthorizedOwnerAsync(Func<Task<DeviceOwnerUser>> loader, EntityHeader org)
        {
            var owner = await loader();
            if (owner == null)
                return InvokeResult<DeviceOwnerUser>.FromError("Device owner could not be found.");

            if (EntityHeader.IsNullOrEmpty(owner.OwnerOrganization) || owner.OwnerOrganization.Id != org.Id)
                return InvokeResult<DeviceOwnerUser>.FromError("Device owner organization does not match the current organization.");

            return InvokeResult<DeviceOwnerUser>.Create(owner);
        }
    }
}
