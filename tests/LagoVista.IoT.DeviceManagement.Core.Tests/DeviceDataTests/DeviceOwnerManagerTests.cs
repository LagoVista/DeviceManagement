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
            _org = EntityHeader.Create("org1", "Org One");
            _user = EntityHeader.Create("user1", "Admin");
            _repo = new DeviceRepository { Id = "repo1", Name = "Repo One", OwnerOrganization = _org };
        }

        [TestMethod]
        public async Task CreateOwnerRejectsDifferentOrganization()
        {
            var owner = CreateOwner("owner1", "org2");

            var result = await _manager.CreateOwnerAsync(owner, _org, _user);

            Assert.IsFalse(result.Successful);
            _ownerRepo.Verify(x => x.AddUserAsync(It.IsAny<DeviceOwnerUser>()), Times.Never);
        }

        [TestMethod]
        public async Task GetOwnerRejectsDifferentOrganization()
        {
            _ownerRepo.Setup(x => x.FindByIdAsync("owner1")).ReturnsAsync(CreateOwner("owner1", "org2"));

            var result = await _manager.GetOwnerByIdAsync("owner1", _org, _user);

            Assert.IsFalse(result.Successful);
        }

        [TestMethod]
        public async Task AssignOwnerAddsAssociationAndSetsDeviceOwner()
        {
            var owner = CreateOwner("owner1", "org1");
            var device = CreateDevice();
            var deviceType = new DeviceType { Product = EntityHeader.Create("product1", "Product One") };

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
            var owner = CreateOwner("owner1", "org1");
            var device = CreateDevice();
            device.DeviceOwner = EntityHeader.Create("owner2", "Other Owner");

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
            var owner = CreateOwner("owner1", "org1");
            var device = CreateDevice();
            device.DeviceOwner = owner.ToEntityHeader();
            var association = new DeviceOwnerDevices
            {
                Id = "assoc1",
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
            Id = "device1",
            Key = "device1",
            Name = "Device One",
            DeviceId = "serial-1",
            DeviceRepository = _repo.ToEntityHeader(),
            DeviceType = EntityHeader<DeviceType>.Create(new DeviceType { Id = "type1", Name = "Type One" })
        };
    }
}
