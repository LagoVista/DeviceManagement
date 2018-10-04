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
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests
{
    [TestClass]
    public class RpcRepoTests
    {
        private static string _hostId = "test_host";
        private static string _orgId = "c8ad4589f26842e7a1aefbaefc979c9b";
        private static string _instanceId = "9e88c7f6b5894dbfb3bc09d20736705e";
        private static string _pipelineModuleId = "0559c89ca08f40ef997ad1e03ed93ef1";
        private static ITransceiverConnectionSettings _transceiverSettings = new TransceiverConnectionSettings();
        private static ILogger _logger;
        private static ITransceiver _rpcTransceiver;
        private static Support.AsyncCoupler<IMessage> _asyncCoupler;
        private static IProxyFactory _proxyFactory;
        private static ProxySettings _proxySettings;
        private static DeviceRepository _deviceRepo = DataFactory.CreateDeviceRespository();

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
                OrganizationId = _orgId,
                InstanceId = _instanceId
            };

            _proxyFactory = new ProxyFactory(_transceiverSettings, _rpcTransceiver, _asyncCoupler, _logger);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_Echo()
        {
            var proxy = _proxyFactory.Create<IDeviceManagementRepo>(_proxySettings);
            var requestValue = "Hello, world";
            var responseValue = await proxy.Echo(requestValue);
            Assert.AreEqual(requestValue, responseValue);
        }

        [TestMethod]
        public async Task IDeviceManagementRepo_AddDeviceAsync()
        {
            var proxy = _proxyFactory.Create<IDeviceManagementRepo>(_proxySettings);
            var device = DataFactory.CreateDevice();
            var responseValue = await proxy.AddDeviceAsync(_deviceRepo, device);
            Assert.IsTrue(responseValue.Successful);
        }
    }
}
