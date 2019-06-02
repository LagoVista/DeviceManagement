using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Rpc.Client.ServiceBus;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Utils;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests.Support
{
    public static class DataFactory
    {
        #region fields
        private static bool _initialized = false;

        public static readonly string LocationId = Guid.NewGuid().ToId();
        public static readonly string OrganizationId = "c8ad4589f26842e7a1aefbaefc979c9b";

        public static readonly string HostId = "test_host";
        public static readonly string InstanceId = "9e88c7f6b5894dbfb3bc09d20736705e";
        public static readonly string PipelineModuleId = "0559c89ca08f40ef997ad1e03ed93ef1";
        public static readonly ITransceiverConnectionSettings TransceiverSettings = new TransceiverConnectionSettings();
        public static readonly DeviceRepository DeviceRepo = CreateDeviceRespository();
        public static ILogger Logger;
        public static ITransceiver RpcTransceiver;
        public static AsyncCoupler<IMessage> AsyncCoupler;
        public static IProxyFactory ProxyFactory;
        public static ProxySettings ProxySettings;
        public static IDeviceManagementRepo DeviceManagementRepoProxy;
        public static IDeviceArchiveRepo DeviceArchiveRepoProxy;
        public static IDeviceLogRepo DeviceLogRepo;
        public static IDevicePEMRepo DevicePEMRepo;
        public static IDeviceGroupRepo DeviceGroupRepo;
        public static IDeviceMediaRepo DeviceMediaRepo;
        public static IDeviceMediaItemRepo DeviceMediaItemRepo;
        public static IDeviceMediaRepoRemote DeviceMediaRepoRemote;
        public static IDeviceMediaItemRepoRemote DeviceMediaItemRepoRemote;


        #endregion

        static public void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            _initialized = true;

            Logger = new AdminLogger(new ConsoleLogWriter(), HostId);

            AsyncCoupler = new AsyncCoupler<IMessage>(Logger, new UsageMetrics(HostId, InstanceId, PipelineModuleId));

            TransceiverSettings.RpcClientReceiver.AccountId = System.Environment.GetEnvironmentVariable("RPC_TESTS_SERVICE_BUS", EnvironmentVariableTarget.User);
            Assert.IsNotNull(TransceiverSettings.RpcClientReceiver.AccountId, "Please add environment variable for [RPC_TESTS_RECEIVE_KEY] with read acess to service bus");

            TransceiverSettings.RpcClientReceiver.UserName = "rpc_test_receive";
            TransceiverSettings.RpcClientReceiver.AccessKey = System.Environment.GetEnvironmentVariable("RPC_TESTS_RECEIVE_KEY", EnvironmentVariableTarget.User);
            Assert.IsNotNull(TransceiverSettings.RpcClientReceiver.AccessKey, "Please add environment variable for [RPC_TESTS_RECEIVE_KEY] with read acess to service bus");
            TransceiverSettings.RpcClientReceiver.ResourceName = "rpc_test";
            TransceiverSettings.RpcClientReceiver.Uri = "application";

            TransceiverSettings.RpcClientTransmitter.AccountId = TransceiverSettings.RpcClientReceiver.AccountId;
            TransceiverSettings.RpcClientTransmitter.UserName = "rpc_test_send";
            TransceiverSettings.RpcClientTransmitter.AccessKey = System.Environment.GetEnvironmentVariable("RPC_TESTS_SEND_KEY", EnvironmentVariableTarget.User);
            Assert.IsNotNull(TransceiverSettings.RpcClientReceiver.AccessKey, "Please add environment variable for [RPC_TESTS_RECEIVE_KEY] with read acess to service bus");
            TransceiverSettings.RpcClientTransmitter.ResourceName = "rpc_test";
            TransceiverSettings.RpcClientTransmitter.TimeoutInSeconds = 30;

            TransceiverSettings.RpcServerTransmitter.AccountId = TransceiverSettings.RpcClientReceiver.AccountId;
            TransceiverSettings.RpcServerTransmitter.UserName = "rpc_test_send";
            TransceiverSettings.RpcServerTransmitter.AccessKey = System.Environment.GetEnvironmentVariable("RPC_TESTS_SEND_KEY", EnvironmentVariableTarget.User);
            Assert.IsNotNull(TransceiverSettings.RpcServerTransmitter.AccessKey, "Please add environment variable for [RPC_TESTS_RECEIVE_KEY] with read acess to service bus");
            TransceiverSettings.RpcServerTransmitter.ResourceName = "rpc_test";
            TransceiverSettings.RpcServerTransmitter.TimeoutInSeconds = 30;

            TransceiverSettings.RpcServerReceiver.AccountId = System.Environment.GetEnvironmentVariable("RPC_TESTS_SERVICE_BUS", EnvironmentVariableTarget.User);
            TransceiverSettings.RpcServerReceiver.UserName = "rpc_test_receive";
            TransceiverSettings.RpcServerReceiver.AccessKey = System.Environment.GetEnvironmentVariable("RPC_TESTS_RECEIVE_KEY", EnvironmentVariableTarget.User);
            TransceiverSettings.RpcServerReceiver.ResourceName = "rpc_test";
            TransceiverSettings.RpcServerReceiver.Uri = "application";

            RpcTransceiver = new ServiceBusProxyClient(AsyncCoupler, Logger);
            RpcTransceiver.StartAsync(TransceiverSettings).Wait();

            ProxySettings = new ProxySettings
            {
                OrganizationId = OrganizationId,
                InstanceId = InstanceId
            };

            ProxyFactory = new ProxyFactory(TransceiverSettings, RpcTransceiver, AsyncCoupler, Logger);

            DeviceManagementRepoProxy = ProxyFactory.Create<IDeviceManagementRepo>(ProxySettings);
            DeviceArchiveRepoProxy = ProxyFactory.Create<IDeviceArchiveRepo>(ProxySettings);
            DeviceLogRepo = ProxyFactory.Create<IDeviceLogRepo>(ProxySettings);
            DevicePEMRepo = ProxyFactory.Create<IDevicePEMRepo>(ProxySettings);

            DeviceGroupRepo = ProxyFactory.Create<IDeviceGroupRepo>(ProxySettings);
            DeviceMediaRepo = ProxyFactory.Create<IDeviceMediaRepo>(ProxySettings);
            DeviceMediaItemRepo = ProxyFactory.Create<IDeviceMediaItemRepo>(ProxySettings);
            DeviceMediaRepoRemote = ProxyFactory.Create<IDeviceMediaRepoRemote>(ProxySettings);
            DeviceMediaItemRepoRemote = ProxyFactory.Create<IDeviceMediaItemRepoRemote>(ProxySettings);
        }

        public static Device CreateDevice(string deviceId = "dev1234")
        {
            return new Device()
            {
                Id = Guid.NewGuid().ToId(),
                CreationDate = DateTime.UtcNow.ToJSONString(),
                LastUpdatedDate = DateTime.UtcNow.ToJSONString(),
                CreatedBy = EntityHeader.Create(Guid.NewGuid().ToId(), "abc123"),
                LastUpdatedBy = EntityHeader.Create(Guid.NewGuid().ToId(), "abc123"),
                OwnerOrganization = EntityHeader.Create(OrganizationId, "test organization"),
                Location = EntityHeader.Create(LocationId, "test location"),
                DeviceId = deviceId,
                PrimaryAccessKey = "abc123",
                SecondaryAccessKey = "def45",
                Name = "tesedevice",
                DeviceConfiguration = EntityHeader.Create("fff", "ddd"),
                DeviceType = EntityHeader<DeviceType>.Create(new DeviceType()
                {
                    Id = Guid.NewGuid().ToId(),
                    Name = "abc",
                }),
                Status = EntityHeader<DeviceStates>.Create(DeviceStates.New)
            };
        }

        public static DeviceRepository CreateDeviceRespository()
        {
            return JsonConvert.DeserializeObject<DeviceRepository>(Properties.Resources.DeviceRepository);
        }
    }
}
