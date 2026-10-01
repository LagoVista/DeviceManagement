using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Interfaces.Repos;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Interfaces.Repos.Account;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceDataTests
{
    [TestClass]
    public class DeviceOwnerManagerTests
    {
        private const string OrgId = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
        private const string OtherOrgId = "EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE";
        private const string UserId = "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB";
        private const string RepoId = "CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC";
        private const string OwnerId = "DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD";
        private const string OtherOwnerId = "11111111111111111111111111111111";
        private const string AssociationId = "22222222222222222222222222222222";
        private const string DeviceId = "33333333333333333333333333333333";
        private const string DeviceTypeId = "44444444444444444444444444444444";
        private const string ProductId = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";

        private Mock<IDeviceOwnerRepo> _ownerRepo;
        private Mock<IDeviceRepositoryManager> _repoManager;
        private Mock<IDeviceManager> _deviceManager;
        private Mock<IDeviceTypeRepo> _deviceTypeRepo;
        private DeviceOwnerManager _manager;
        private EntityHeader _org;
        private EntityHeader _user;
        private DeviceRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _ownerRepo = new Mock<IDeviceOwnerRepo>();
            _repoManager = new Mock<IDeviceRepositoryManager>();
            _deviceManager = new Mock<IDeviceManager>();
            _deviceTypeRepo = new Mock<IDeviceTypeRepo>();
            _manager = new DeviceOwnerManager(_ownerRepo.Object, _repoManager.Object, _deviceManager.Object, _deviceTypeRepo.Object);
            _org = EntityHeader.Create(OrgId, "Org One");
            _user = EntityHeader.Create(UserId, "Admin");
            _repo = new DeviceRepository { Id = RepoId, Name = "Repo One", OwnerOrganization = _org };
        }

        [TestMethod]
        public async Task CreateOwnerRejectsDifferentOrganization()
        {
            var owner = CreateOwner(OwnerId, OtherOrgId);

            var result = await _manager.CreateOwnerAsync(owner, _org, _user);

            Assert.IsFalse(result.Successful);
            _ownerRepo.Verify(x => x.AddUserAsync(It.IsAny<DeviceOwnerUser>()), Times.Never);
        }

        [TestMethod]
        public async Task GetOwnerRejectsDifferentOrganization()
        {
            _ownerRepo.Setup(x => x.FindByIdAsync(OwnerId)).ReturnsAsync(CreateOwner(OwnerId, OtherOrgId));

            var result = await _manager.GetOwnerByIdAsync(OwnerId, _org, _user);

            Assert.IsFalse(result.Successful);
        }

        [TestMethod]
        public async Task AssignOwnerAddsAssociationAndSetsDeviceOwner()
        {
            var owner = CreateOwner(OwnerId, OrgId);
            var device = CreateDevice();
            var deviceType = new DeviceType { Product = EntityHeader.Create(ProductId, "Product One") };

            _ownerRepo.Setup(x => x.FindByIdAsync(owner.Id)).ReturnsAsync(owner);
            _deviceManager.Setup(x => x.GetDeviceByIdAsync(_repo, device.Id, _org, _user, false))
                .ReturnsAsync(InvokeResult<Device>.Create(device));
            _deviceTypeRepo.Setup(x => x.GetDeviceTypeAsync(device.DeviceType.Id)).ReturnsAsync(deviceType);
            _ownerRepo.Setup(x => x.AddOwnedDeviceAsync(_org.Id, owner.Id, It.IsAny<DeviceOwnerDevices>()))
                .ReturnsAsync(owner);
            _deviceManager.Setup(x => x.UpdateDeviceAsync(_repo, device, _org, _user))
                .ReturnsAsync(InvokeResult<Device>.Create(device));

            var result = await _manager.AssignOwnerToDeviceAsync(_repo, device.Id, owner.Id, false, _org, _user);

            Assert.IsTrue(result.Successful);
            Assert.AreEqual(owner.Id.ToString(), device.DeviceOwner.Id.ToString());
            _ownerRepo.Verify(x => x.AddOwnedDeviceAsync(_org.Id, owner.Id,
                It.Is<DeviceOwnerDevices>(d => d.Device.Id == device.Id && d.DeviceId == device.DeviceId)), Times.Once);
            _deviceManager.Verify(x => x.UpdateDeviceAsync(_repo, device, _org, _user), Times.Once);
        }

        [TestMethod]
        public async Task AssignOwnerDoesNotReplaceExistingOwnerWithoutConfirmation()
        {
            var owner = CreateOwner(OwnerId, OrgId);
            var device = CreateDevice();
            device.DeviceOwner = EntityHeader.Create(OtherOwnerId, "Other Owner");

            _ownerRepo.Setup(x => x.FindByIdAsync(owner.Id)).ReturnsAsync(owner);
            _deviceManager.Setup(x => x.GetDeviceByIdAsync(_repo, device.Id, _org, _user, false))
                .ReturnsAsync(InvokeResult<Device>.Create(device));

            var result = await _manager.AssignOwnerToDeviceAsync(_repo, device.Id, owner.Id, false, _org, _user);

            Assert.IsFalse(result.Successful);
            _ownerRepo.Verify(x => x.AddOwnedDeviceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DeviceOwnerDevices>()), Times.Never);
            _deviceManager.Verify(x => x.UpdateDeviceAsync(It.IsAny<DeviceRepository>(), It.IsAny<Device>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Never);
        }

        [TestMethod]
        public async Task RemoveOwnerRemovesAssociationAndClearsDeviceOwner()
        {
            var owner = CreateOwner(OwnerId, OrgId);
            var device = CreateDevice();
            device.DeviceOwner = owner.ToEntityHeader();
            var association = new DeviceOwnerDevices
            {
                Id = AssociationId,
                Device = device.ToEntityHeader(),
                DeviceId = device.DeviceId,
                DeviceRepository = _repo.ToEntityHeader()
            };
            owner.Devices.Add(association);

            _ownerRepo.Setup(x => x.FindByIdAsync(owner.Id)).ReturnsAsync(owner);
            _deviceManager.Setup(x => x.GetDeviceByIdAsync(_repo, device.Id, _org, _user, false))
                .ReturnsAsync(InvokeResult<Device>.Create(device));
            _ownerRepo.Setup(x => x.RemoveOwnedDeviceAsync(_org.Id, owner.Id, association.Id)).ReturnsAsync(owner);
            _deviceManager.Setup(x => x.UpdateDeviceAsync(_repo, device, _org, _user))
                .ReturnsAsync(InvokeResult<Device>.Create(device));

            var result = await _manager.RemoveOwnerFromDeviceAsync(_repo, device.Id, owner.Id, _org, _user);

            Assert.IsTrue(result.Successful);
            Assert.IsTrue(EntityHeader.IsNullOrEmpty(device.DeviceOwner));
            _ownerRepo.Verify(x => x.RemoveOwnedDeviceAsync(_org.Id, owner.Id, association.Id), Times.Once);
            _deviceManager.Verify(x => x.UpdateDeviceAsync(_repo, device, _org, _user), Times.Once);
        }

        [TestMethod]
        public async Task CreateOwnerUsesCurrentOrganizationAndPersists()
        {
            var owner = CreateOwner(OwnerId, OrgId);
            _ownerRepo.Setup(x => x.AddUserAsync(owner)).ReturnsAsync(InvokeResult.Success);

            var result = await _manager.CreateOwnerAsync(owner, _org, _user);

            Assert.IsTrue(result.Successful);
            Assert.AreEqual(OrgId, owner.OwnerOrganization.Id.ToString());
            _ownerRepo.Verify(x => x.AddUserAsync(owner), Times.Once);
        }

        [TestMethod]
        public async Task UpdateOwnerPreservesStoredOrganization()
        {
            var existing = CreateOwner(OwnerId, OrgId);
            var update = CreateOwner(OwnerId, OtherOrgId);
            update.FirstName = "Updated";

            _ownerRepo.Setup(x => x.FindByIdAsync(OwnerId)).ReturnsAsync(existing);
            _ownerRepo.Setup(x => x.UpdateUserAsync(update)).ReturnsAsync(InvokeResult.Success);

            var result = await _manager.UpdateOwnerAsync(update, _org, _user);

            Assert.IsTrue(result.Successful);
            Assert.AreEqual(OrgId, update.OwnerOrganization.Id.ToString());
            _ownerRepo.Verify(x => x.UpdateUserAsync(update), Times.Once);
        }

        [TestMethod]
        public async Task DeleteOwnerWithoutDevicesDeletesOwnerRecord()
        {
            var owner = CreateOwner(OwnerId, OrgId);
            _ownerRepo.Setup(x => x.FindByIdAsync(OwnerId)).ReturnsAsync(owner);
            _ownerRepo.Setup(x => x.DeleteUserAsync(_org.Id, owner.Id)).ReturnsAsync(InvokeResult.Success);

            var result = await _manager.DeleteOwnerAsync(owner.Id, _org, _user);

            Assert.IsTrue(result.Successful);
            _ownerRepo.Verify(x => x.DeleteUserAsync(_org.Id, owner.Id), Times.Once);
        }

        private static DeviceOwnerUser CreateOwner(string id, string orgId) => new DeviceOwnerUser
        {
            Id = id,
            Key = id,
            UserName = id,
            FirstName = "Test",
            LastName = "Owner",
            OwnerOrganization = EntityHeader.Create(orgId, orgId)
        };

        private Device CreateDevice() => new Device
        {
            Id = DeviceId,
            Key = "device1",
            Name = "Device One",
            DeviceId = "serial-1",
            DeviceRepository = _repo.ToEntityHeader(),
            DeviceType = EntityHeader<DeviceType>.Create(new DeviceType { Id = DeviceTypeId, Name = "Type One" })
        };
    }
}
