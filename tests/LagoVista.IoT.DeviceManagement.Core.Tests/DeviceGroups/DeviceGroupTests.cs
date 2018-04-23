using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using LagoVista.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceGroups
{
    [TestClass]
    public class DeviceGroupTests : ValidationBase
    {
        DeviceGroupManager _groupManager;
        Mock<IDeviceGroupRepo> _deviceGroupRepo = new Mock<IDeviceGroupRepo>();
        Mock<IDeviceManagementRepo> _deviceManagementRepo = new Mock<IDeviceManagementRepo>();
        Mock<IDeviceRepositoryManager> _repoMgr = new Mock<IDeviceRepositoryManager>();
        Mock<IDeviceManagementRepo> _devMgmt = new Mock<IDeviceManagementRepo>();
        Mock<IDeviceManagementConnector> _deviceConnectorService = new Mock<IDeviceManagementConnector>();
        Mock<IDeviceConfigHelper> _deviceConfigHelper = new Mock<IDeviceConfigHelper>();
        Mock<IAdminLogger> _logger = new Mock<IAdminLogger>();
        Mock<ISecureStorage> _secureStorage = new Mock<ISecureStorage>();
        Mock<IAppConfig> _appConfig = new Mock<IAppConfig>();
        Mock<IDependencyManager> _dependencyManager = new Mock<IDependencyManager>();
        Mock<ISecurity> _security = new Mock<ISecurity>();
        Mock<DeviceRepository> _repo = new Mock<DeviceRepository>();

        const string DEVICEID = "7DB8128506A14D4FBC76A61D2AC2D769";
        const string GROUPID = "ACB8128506A14D4FBC76A61D2AC2D739";

        DeviceGroup _deviceGroup;
        Device _device;

        EntityHeader _org = new EntityHeader() { Id = "A228128506A14D4FBC76A61D2AC2D742", Text = "MY ORG" };
        EntityHeader _user = new EntityHeader() { Id = "28B8128506A14D4FBC76A61D2AC2D733", Text = "MY USER" };

        [TestInitialize]
        public void Init()
        {
            _groupManager = new DeviceGroupManager(_deviceGroupRepo.Object, _deviceManagementRepo.Object, _deviceConnectorService.Object, _logger.Object, _appConfig.Object, _dependencyManager.Object, _security.Object);

            _device = new Device()
            {
                Id = DEVICEID,
                Name = "My Device",
                DeviceId = "device001",
                DeviceType = new EntityHeader() { Id = "28B8128506A14D4FBC76A61D2AC2D733", Text = "DeviceType" },
                DeviceConfiguration = new EntityHeader() { Id = "28B8128506A14D4FBC76A61D2AC2D733", Text = "DeviceConfig" },
            };

            _deviceManagementRepo.Setup<Task<Device>>(dev => dev.GetDeviceByIdAsync(_repo.Object, DEVICEID)).ReturnsAsync(_device);

            _deviceGroup = new DeviceGroup()
            {
                Id = GROUPID,
                Name = "My Group",
                Devices = new List<DeviceGroupEntry>()
            };

            _deviceGroupRepo.Setup<Task<DeviceGroup>>(dev => dev.GetDeviceGroupAsync(_repo.Object, GROUPID)).ReturnsAsync(_deviceGroup);

        }

        public DeviceGroupTests()
        {

        }

        [TestMethod]
        public async Task DeviceGroups_Add_ToGroup_Returns_SuccessInvokeResult()
        {
            var result = await _groupManager.AddDeviceToGroupAsync(_repo.Object, GROUPID, DEVICEID, _org, _user);
            AssertSuccessful(result.ToInvokeResult());
        }


        [TestMethod]
        public async Task DeviceGroups_Add_ToGroup_AddedToDeviceList_Valid()
        {
            var result = await _groupManager.AddDeviceToGroupAsync(_repo.Object, GROUPID, DEVICEID, _org, _user);
            Assert.AreEqual(1, _deviceGroup.Devices.Count);
        }

        [TestMethod]
        public async Task DeviceGroups_Add_ToGroup_GroupExistsInDevice_Valid()
        {
            var result = await _groupManager.AddDeviceToGroupAsync(_repo.Object, GROUPID, DEVICEID, _org, _user);

            Assert.AreEqual(1, _device.DeviceGroups.Count);

            Assert.AreEqual(GROUPID, _device.DeviceGroups[0].Id);
        }


        [TestMethod]
        public async Task DeviceGroups_Add_ToGroup_ReturnsNewlyAddedDeviceGroupEntry_Valid()
        {
            var result = await _groupManager.AddDeviceToGroupAsync(_repo.Object, GROUPID, DEVICEID, _org, _user);
            AssertSuccessful(result.ToInvokeResult());
            var item = result.Result;
            Assert.AreEqual(1, _deviceGroup.Devices.Count);
            Assert.AreEqual(_user, item.AddedBy);
            Assert.AreEqual(_device.Name, item.Name);
            Assert.AreEqual(_device.Id, item.DeviceUniqueId);
            Assert.AreEqual(_device.DeviceId, item.DeviceId);
            Assert.AreEqual(_device.DeviceType, item.DeviceType);
            Assert.AreEqual(_device.DeviceConfiguration, item.DeviceConfiguration);
        }


        [TestMethod]
        public async Task DeviceGroups_Add_ToGroup_AlreadyAdded_Failed()
        {
            _deviceGroup.Devices.Add(DeviceGroupEntry.FromDevice(_device, _user));

            var result = await _groupManager.AddDeviceToGroupAsync(_repo.Object, GROUPID, DEVICEID, _org, _user);
            AssertInvalidError(result.ToInvokeResult(), "The device [device001] already belongs to this device group and can not be added again.");

            Assert.AreEqual(1, _deviceGroup.Devices.Count);
        }

        [TestMethod]
        public async Task DeviceGroups_Add_ToGroup_CreatesCorrectDevice_Invalid()
        {
            var item = DeviceGroupEntry.FromDevice(_device, _user);
            Assert.AreEqual(_user, item.AddedBy);
            Assert.AreEqual(_device.Name, item.Name);
            Assert.AreEqual(_device.Id, item.DeviceUniqueId);
            Assert.AreEqual(_device.DeviceId, item.DeviceId);
            Assert.AreEqual(_device.DeviceType, item.DeviceType);
            Assert.AreEqual(_device.DeviceConfiguration, item.DeviceConfiguration);
            
            Assert.IsTrue(TimeSpan.FromSeconds(5) > DateTime.UtcNow - item.DateAdded.ToDateTime());

            var result = await _groupManager.AddDeviceToGroupAsync(_repo.Object, GROUPID, DEVICEID, _org, _user);
            Assert.AreEqual(1, _deviceGroup.Devices.Count);
        }

        [TestMethod]
        public async Task DeviceGroups_Remove_ReturnsInvokeSuccess_Valid()
        {
            _deviceGroup.Devices.Add(DeviceGroupEntry.FromDevice(_device, _user));

            var result = await _groupManager.RemoveDeviceFromGroupAsync(_repo.Object, GROUPID, DEVICEID, _org, _user);
            AssertSuccessful(result.ToInvokeResult());
        }

        [TestMethod]
        public async Task DeviceGroups_Remove_ShoudRemoveFromGroup_Valid()
        {
            _deviceGroup.Devices.Add(DeviceGroupEntry.FromDevice(_device, _user));

            var result = await _groupManager.RemoveDeviceFromGroupAsync(_repo.Object, GROUPID, DEVICEID, _org, _user);
            Assert.AreEqual(0, _deviceGroup.Devices.Count);
        }

        [TestMethod]
        public async Task DeviceGroups_Remove_ShoudGroupFromDevice_Valid()
        {
            _deviceGroup.Devices.Add(DeviceGroupEntry.FromDevice(_device, _user));
            _device.DeviceGroups.Add(_deviceGroup.ToEntityHeader());
            Assert.AreEqual(1, _device.DeviceGroups.Count);

            var result = await _groupManager.RemoveDeviceFromGroupAsync(_repo.Object, GROUPID, DEVICEID, _org, _user);
            Assert.AreEqual(0, _device.DeviceGroups.Count);
        }

        [TestMethod]
        public async Task DeviceGroups_Remove_DoesNotExistInGroup_Fail()
        {
            var result = await _groupManager.RemoveDeviceFromGroupAsync(_repo.Object, GROUPID, DEVICEID, _org, _user);
            Assert.AreEqual(0, _deviceGroup.Devices.Count);
            AssertInvalidError(result, "The device [device001] does not belong to this device group and can not be removed again.");


        }
    }
}
