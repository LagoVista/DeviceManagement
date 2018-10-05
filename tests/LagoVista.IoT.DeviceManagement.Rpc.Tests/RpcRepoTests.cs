using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Rpc.Client.ServiceBus;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Rpc.Tests.Support;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests
{
    [TestClass]
    public class Rpc_IDeviceManagementRepo_Tests
    {
        #region fields
        private static readonly string _hostId = "test_host";
        private static readonly string _instanceId = "9e88c7f6b5894dbfb3bc09d20736705e";
        private static readonly string _pipelineModuleId = "0559c89ca08f40ef997ad1e03ed93ef1";
        private static readonly ITransceiverConnectionSettings _transceiverSettings = new TransceiverConnectionSettings();
        private static readonly DeviceRepository _deviceRepo = DataFactory.CreateDeviceRespository();
        private static ILogger _logger;
        private static ITransceiver _rpcTransceiver;
        private static Support.AsyncCoupler<IMessage> _asyncCoupler;
        private static IProxyFactory _proxyFactory;
        private static ProxySettings _proxySettings;
        private static IDeviceManagementRepo _deviceManagementRepoProxy;
        #endregion

        [ClassInitialize]
        static public void ClassInitialize(TestContext testContext)
        {
            _logger = new AdminLogger(new ConsoleLogWriter(), _hostId);

            _asyncCoupler = new Support.AsyncCoupler<IMessage>(_logger, new UsageMetrics(_hostId, _instanceId, _pipelineModuleId));

            _transceiverSettings.RpcReceiver.AccountId = "localrequestbus-dev";
            _transceiverSettings.RpcReceiver.UserName = "ListenAccessKey";
            _transceiverSettings.RpcReceiver.AccessKey = Environment.GetEnvironmentVariable("RpcReceiverAccessKey", EnvironmentVariableTarget.Machine);
            _transceiverSettings.RpcReceiver.ResourceName = "rpc_test";
            _transceiverSettings.RpcReceiver.Uri = "application";

            _transceiverSettings.RpcTransmitter.AccountId = "localrequestbus-dev";
            _transceiverSettings.RpcTransmitter.UserName = "SendAccessKey";
            _transceiverSettings.RpcTransmitter.AccessKey = Environment.GetEnvironmentVariable("RpcTransmitterAccessKey", EnvironmentVariableTarget.Machine);
            _transceiverSettings.RpcTransmitter.ResourceName = "rpc_test";
            _transceiverSettings.RpcTransmitter.TimeoutInSeconds = 30;

            _rpcTransceiver = new ServiceBusProxyClient(_transceiverSettings, _asyncCoupler, _logger);
            _rpcTransceiver.StartAsync().Wait();

            _proxySettings = new ProxySettings
            {
                OrganizationId = DataFactory.OrganizationId,
                InstanceId = _instanceId
            };

            _proxyFactory = new ProxyFactory(_transceiverSettings, _rpcTransceiver, _asyncCoupler, _logger);

            _deviceManagementRepoProxy = _proxyFactory.Create<IDeviceManagementRepo>(_proxySettings);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_Echo()
        {
            var requestValue = "Hello, world";
            var responseValue = await _deviceManagementRepoProxy.Echo(requestValue);
            Assert.AreEqual(requestValue, responseValue);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_AddDeviceAsync()
        {
            var device = DataFactory.CreateDevice();
            var responseValue = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(responseValue.Successful);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_DeleteDeviceAsync()
        {
            var device = DataFactory.CreateDevice();
            var addDeviceResponse = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(addDeviceResponse.Successful);

            await _deviceManagementRepoProxy.DeleteDeviceAsync(_deviceRepo, device.Id);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_UpdateDeviceAsync()
        {
            var device = DataFactory.CreateDevice();
            var deviceName = device.Name;
            var addDeviceResponse = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(addDeviceResponse.Successful);

            var getDeviceResponse = await _deviceManagementRepoProxy.GetDeviceByIdAsync(_deviceRepo, device.Id);
            Assert.AreEqual(deviceName, getDeviceResponse.Name);

            device.Name = device.Name + " changed";
            await _deviceManagementRepoProxy.UpdateDeviceAsync(_deviceRepo, device);

            getDeviceResponse = await _deviceManagementRepoProxy.GetDeviceByIdAsync(_deviceRepo, device.Id);
            Assert.AreNotEqual(deviceName, getDeviceResponse.Name);
            Assert.AreEqual(deviceName + " changed", getDeviceResponse.Name);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_DeleteDeviceByIdAsync()
        {
            var deviceId = Guid.NewGuid().ToId();
            var device = DataFactory.CreateDevice(deviceId);
            var deviceName = device.Name;
            var addDeviceResponse = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(addDeviceResponse.Successful);

            var getDeviceResponse = await _deviceManagementRepoProxy.GetDeviceByIdAsync(_deviceRepo, device.Id);
            Assert.AreEqual(deviceName, getDeviceResponse.Name);

            await _deviceManagementRepoProxy.DeleteDeviceByIdAsync(_deviceRepo, deviceId);

            getDeviceResponse = await _deviceManagementRepoProxy.GetDeviceByIdAsync(_deviceRepo, device.Id);
            Assert.IsNull(getDeviceResponse);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_GetDevicesForRepositoryAsync()
        {
            var device = DataFactory.CreateDevice();
            var deviceName = device.Name;
            var addDeviceResponse = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(addDeviceResponse.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var deviceListResponse = await _deviceManagementRepoProxy.GetDevicesForRepositoryAsync(_deviceRepo, DataFactory.OrganizationId, listRequest);
            Assert.IsTrue(deviceListResponse.Successful);
            Assert.IsTrue(deviceListResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_GetDevicesForLocationIdAsync()
        {
            var device = DataFactory.CreateDevice();
            var addDeviceResponse = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(addDeviceResponse.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var deviceListResponse = await _deviceManagementRepoProxy.GetDevicesForLocationIdAsync(_deviceRepo, DataFactory.LocationId, listRequest);
            Assert.IsTrue(deviceListResponse.Successful);
            Assert.IsTrue(deviceListResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_GetDeviceByDeviceIdAsync()
        {
            var deviceId = Guid.NewGuid().ToId();
            var device = DataFactory.CreateDevice(deviceId);
            var deviceName = device.Name;
            var addDeviceResponse = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(addDeviceResponse.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var deviceResponse = await _deviceManagementRepoProxy.GetDeviceByDeviceIdAsync(_deviceRepo, deviceId);
            Assert.IsNotNull(deviceResponse);
        }

        [TestMethod]
        public Task IDeviceManagementRepo_CheckIfDeviceIdInUse()
        {
            //var deviceId = string.Empty;
            // await _deviceManagementRepoProxy.CheckIfDeviceIdInUse(_deviceRepo, deviceId, DataFactory.OrganizationId);
            return Task.FromResult<object>(null);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_GetDeviceByIdAsync()
        {
            var device = DataFactory.CreateDevice();
            device.Name = Guid.NewGuid().ToId();
            var responseValue = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var deviceResponse = await _deviceManagementRepoProxy.GetDeviceByIdAsync(_deviceRepo, device.Id);
            Assert.AreEqual(device.Name, deviceResponse.Name);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_GetDevicesInStatusAsync()
        {
            var device = DataFactory.CreateDevice();
            device.Status = EntityHeader<DeviceStates>.Create(DeviceStates.Ready);
            var responseValue = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var status = DeviceStates.Ready.ToString().ToLower();
            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var getDevicesResponse = await _deviceManagementRepoProxy.GetDevicesInStatusAsync(_deviceRepo, status, listRequest);
            Assert.IsTrue(getDevicesResponse.Successful);
            Assert.IsTrue(getDevicesResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_GetDevicesInCustomStatusAsync()
        {
            var device = DataFactory.CreateDevice();
            var customStatus = "freakout";
            device.CustomStatus = EntityHeader.Create(customStatus, customStatus);
            var responseValue = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var getDevicesResponse = await _deviceManagementRepoProxy.GetDevicesInCustomStatusAsync(_deviceRepo, customStatus, listRequest);
            Assert.IsTrue(getDevicesResponse.Successful);
            Assert.IsTrue(getDevicesResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_GetDevicesWithConfigurationAsync()
        {
            var device = DataFactory.CreateDevice();
            var configurationId = Guid.NewGuid().ToId();
            device.DeviceConfiguration = EntityHeader.Create(configurationId, configurationId);
            var responseValue = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var getDevicesResponse = await _deviceManagementRepoProxy.GetDevicesWithConfigurationAsync(_deviceRepo, configurationId, listRequest);
            Assert.IsTrue(getDevicesResponse.Successful);
            Assert.IsTrue(getDevicesResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_GetFullDevicesWithConfigurationAsync()
        {
            var device = DataFactory.CreateDevice();
            var configurationId = Guid.NewGuid().ToId();
            device.DeviceConfiguration = EntityHeader.Create(configurationId, configurationId);
            var responseValue = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var getDevicesResponse = await _deviceManagementRepoProxy.GetFullDevicesWithConfigurationAsync(_deviceRepo, configurationId, listRequest);
            Assert.IsTrue(getDevicesResponse.Successful);
            Assert.IsTrue(getDevicesResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_GetDevicesWithDeviceTypeAsync()
        {
            var device = DataFactory.CreateDevice();
            var deviceTypeId = Guid.NewGuid().ToId();
            device.DeviceType = EntityHeader.Create(deviceTypeId, deviceTypeId);
            var responseValue = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var getDevicesResponse = await _deviceManagementRepoProxy.GetDevicesWithDeviceTypeAsync(_deviceRepo, deviceTypeId, listRequest);
            Assert.IsTrue(getDevicesResponse.Successful);
            Assert.IsTrue(getDevicesResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_SearchByDeviceIdAsync()
        {
            var deviceId = Guid.NewGuid().ToId();
            var device = DataFactory.CreateDevice(deviceId);
            var responseValue = await _deviceManagementRepoProxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(responseValue.Successful);

            var search = device.DeviceId.Substring(5, 10);
            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var getDevicesResponse = await _deviceManagementRepoProxy.SearchByDeviceIdAsync(_deviceRepo, search, listRequest);
            Assert.IsTrue(getDevicesResponse.Successful);
            Assert.IsTrue(getDevicesResponse.Model.Count() > 0);
        }

        [TestMethod]
        public Task IDeviceManagementRepo_GetDeviceGroupSummaryDataAsync(string groupId, ListRequest listRequest)
        {
            //var response = _deviceManagementRepoProxy.GetDeviceGroupSummaryDataAsync(_deviceRepo, groupId, listRequest);
            return Task.FromResult<object>(null);
        }
    }
}
