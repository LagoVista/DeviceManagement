// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8e2bb056485e094c74e70fd63cb1aa6b6f0a4d1f5c26f624813e3986bf439c22
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Rpc.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests
{
    [TestClass]
    public class Rpc_IDeviceManagementRepo_Tests
    {
        [ClassInitialize]
        static public void ClassInitialize(TestContext testContext)
        {
            DataFactory.Initialize();
        }

        [TestMethod]
        public async Task Echo()
        {
            var requestValue = "Hello, world";
            var responseValue = await DataFactory.DeviceManagementRepoProxy.Echo(requestValue);
            Assert.AreEqual(requestValue, responseValue);
        }

        [TestMethod]
        public async Task AddDeviceAsync()
        {
            var device = DataFactory.CreateDevice();
            var responseValue = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(responseValue.Successful);
        }

        [TestMethod]
        public async Task DeleteDeviceAsync()
        {
            var device = DataFactory.CreateDevice();
            var addDeviceResponse = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(addDeviceResponse.Successful);

            await DataFactory.DeviceManagementRepoProxy.DeleteDeviceAsync(DataFactory.DeviceRepo, device.Id);
        }

        [TestMethod]
        public async Task UpdateDeviceAsync()
        {
            var device = DataFactory.CreateDevice();
            var deviceName = device.Name;
            var addDeviceResponse = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(addDeviceResponse.Successful);

            var getDeviceResponse = await DataFactory.DeviceManagementRepoProxy.GetDeviceByIdAsync(DataFactory.DeviceRepo, device.Id);
            Assert.AreEqual(deviceName, getDeviceResponse.Name);

            device.Name = device.Name + " changed";
            await DataFactory.DeviceManagementRepoProxy.UpdateDeviceAsync(DataFactory.DeviceRepo, device);

            getDeviceResponse = await DataFactory.DeviceManagementRepoProxy.GetDeviceByIdAsync(DataFactory.DeviceRepo, device.Id);
            Assert.AreNotEqual(deviceName, getDeviceResponse.Name);
            Assert.AreEqual(deviceName + " changed", getDeviceResponse.Name);
        }

        [TestMethod]
        public async Task DeleteDeviceByIdAsync()
        {
            var deviceId = Guid.NewGuid().ToId();
            var device = DataFactory.CreateDevice(deviceId);
            var deviceName = device.Name;
            var addDeviceResponse = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(addDeviceResponse.Successful);

            var getDeviceResponse = await DataFactory.DeviceManagementRepoProxy.GetDeviceByIdAsync(DataFactory.DeviceRepo, device.Id);
            Assert.AreEqual(deviceName, getDeviceResponse.Name);

            await DataFactory.DeviceManagementRepoProxy.DeleteDeviceByIdAsync(DataFactory.DeviceRepo, deviceId);

            getDeviceResponse = await DataFactory.DeviceManagementRepoProxy.GetDeviceByIdAsync(DataFactory.DeviceRepo, device.Id);
            Assert.IsNull(getDeviceResponse);
        }

        [TestMethod]
        public async Task GetDevicesForRepositoryAsync()
        {
            var device = DataFactory.CreateDevice();
            var deviceName = device.Name;
            var addDeviceResponse = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(addDeviceResponse.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var deviceListResponse = await DataFactory.DeviceManagementRepoProxy.GetDevicesForRepositoryAsync(DataFactory.DeviceRepo, DataFactory.OrganizationId, listRequest);
            Assert.IsTrue(deviceListResponse.Successful);
            Assert.IsTrue(deviceListResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task GetDevicesForLocationIdAsync()
        {
            var device = DataFactory.CreateDevice();
            var addDeviceResponse = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(addDeviceResponse.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var deviceListResponse = await DataFactory.DeviceManagementRepoProxy.GetDevicesForLocationIdAsync(DataFactory.DeviceRepo, DataFactory.LocationId, listRequest);
            Assert.IsTrue(deviceListResponse.Successful);
            Assert.IsTrue(deviceListResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task GetDeviceByDeviceIdAsync()
        {
            var deviceId = Guid.NewGuid().ToId();
            var device = DataFactory.CreateDevice(deviceId);
            var deviceName = device.Name;
            var addDeviceResponse = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(addDeviceResponse.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var deviceResponse = await DataFactory.DeviceManagementRepoProxy.GetDeviceByDeviceIdAsync(DataFactory.DeviceRepo, deviceId);
            Assert.IsNotNull(deviceResponse);
        }

        [TestMethod]
        public async Task CheckIfDeviceIdInUse()
        {
            var deviceId = Guid.NewGuid().ToId();
            var device = DataFactory.CreateDevice(deviceId);
            
            var addDeviceResponse = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(addDeviceResponse.Successful);

            var deviceInUse = await DataFactory.DeviceManagementRepoProxy.CheckIfDeviceIdInUse(DataFactory.DeviceRepo, deviceId, DataFactory.OrganizationId);
            Assert.IsTrue(deviceInUse);

            deviceId = Guid.NewGuid().ToId();
            deviceInUse = await DataFactory.DeviceManagementRepoProxy.CheckIfDeviceIdInUse(DataFactory.DeviceRepo, deviceId, DataFactory.OrganizationId);
            Assert.IsFalse(deviceInUse);
        }

        [TestMethod]
        public async Task GetDeviceByIdAsync()
        {
            var device = DataFactory.CreateDevice();
            device.Name = Guid.NewGuid().ToId();
            var responseValue = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var deviceResponse = await DataFactory.DeviceManagementRepoProxy.GetDeviceByIdAsync(DataFactory.DeviceRepo, device.Id);
            Assert.AreEqual(device.Name, deviceResponse.Name);
        }

        [TestMethod]
        public async Task GetDevicesInStatusAsync()
        {
            var device = DataFactory.CreateDevice();
            device.Status = EntityHeader<DeviceStates>.Create(DeviceStates.Ready);
            var responseValue = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var status = DeviceStates.Ready.ToString().ToLower();
            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var getDevicesResponse = await DataFactory.DeviceManagementRepoProxy.GetDevicesInStatusAsync(DataFactory.DeviceRepo, status, listRequest);
            Assert.IsTrue(getDevicesResponse.Successful);
            Assert.IsTrue(getDevicesResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task GetDevicesInCustomStatusAsync()
        {
            var device = DataFactory.CreateDevice();
            var customStatus = "freakout";
            device.CustomStatus = EntityHeader.Create(customStatus, customStatus);
            var responseValue = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var getDevicesResponse = await DataFactory.DeviceManagementRepoProxy.GetDevicesInCustomStatusAsync(DataFactory.DeviceRepo, customStatus, listRequest);
            Assert.IsTrue(getDevicesResponse.Successful);
            Assert.IsTrue(getDevicesResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task GetDevicesWithConfigurationAsync()
        {
            var device = DataFactory.CreateDevice();
            var configurationId = Guid.NewGuid().ToId();
            device.DeviceConfiguration = EntityHeader.Create(configurationId, configurationId);
            var responseValue = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var getDevicesResponse = await DataFactory.DeviceManagementRepoProxy.GetDevicesWithConfigurationAsync(DataFactory.DeviceRepo, configurationId, listRequest);
            Assert.IsTrue(getDevicesResponse.Successful);
            Assert.IsTrue(getDevicesResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task GetFullDevicesWithConfigurationAsync()
        {
            var device = DataFactory.CreateDevice();
            var configurationId = Guid.NewGuid().ToId();
            device.DeviceConfiguration = EntityHeader.Create(configurationId, configurationId);
            var responseValue = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var getDevicesResponse = await DataFactory.DeviceManagementRepoProxy.GetFullDevicesWithConfigurationAsync(DataFactory.DeviceRepo, configurationId, listRequest);
            Assert.IsTrue(getDevicesResponse.Successful);
            Assert.IsTrue(getDevicesResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task GetDevicesWithDeviceTypeAsync()
        {
            var device = DataFactory.CreateDevice();
            var deviceTypeId = Guid.NewGuid().ToId();
            device.DeviceType = EntityHeader<DeviceType>.Create(new DeviceType()
            {
                Id = deviceTypeId,
                Name = "abc",
            });
            var responseValue = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var getDevicesResponse = await DataFactory.DeviceManagementRepoProxy.GetDevicesWithDeviceTypeAsync(DataFactory.DeviceRepo, deviceTypeId, listRequest);
            Assert.IsTrue(getDevicesResponse.Successful);
            Assert.IsTrue(getDevicesResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task SearchByDeviceIdAsync()
        {
            var deviceId = Guid.NewGuid().ToId();
            var device = DataFactory.CreateDevice(deviceId);
            var responseValue = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var search = device.DeviceId.Substring(5, 10);
            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var getDevicesResponse = await DataFactory.DeviceManagementRepoProxy.SearchByDeviceIdAsync(DataFactory.DeviceRepo, search, listRequest);
            Assert.IsTrue(getDevicesResponse.Successful);
            Assert.IsTrue(getDevicesResponse.Model.Count() > 0);
        }

        [TestMethod]
        public Task GetDeviceGroupSummaryDataAsync(string groupId, ListRequest listRequest)
        {
            //var response = DataFactory._deviceManagementRepoProxy.GetDeviceGroupSummaryDataAsync(DataFactory._deviceRepo, groupId, listRequest);
            return Task.FromResult<object>(null);
        }
    }
}
