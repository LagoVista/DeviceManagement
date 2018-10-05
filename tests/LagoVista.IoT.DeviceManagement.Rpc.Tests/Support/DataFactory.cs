using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Rpc.Client.ServiceBus;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Utils;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
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

            TransceiverSettings.RpcReceiver.AccountId = "localrequestbus-dev";
            TransceiverSettings.RpcReceiver.UserName = "ListenAccessKey";
            TransceiverSettings.RpcReceiver.AccessKey = Environment.GetEnvironmentVariable("RpcReceiverAccessKey", EnvironmentVariableTarget.Machine);
            TransceiverSettings.RpcReceiver.ResourceName = "rpc_test";
            TransceiverSettings.RpcReceiver.Uri = "application";

            TransceiverSettings.RpcTransmitter.AccountId = "localrequestbus-dev";
            TransceiverSettings.RpcTransmitter.UserName = "SendAccessKey";
            TransceiverSettings.RpcTransmitter.AccessKey = Environment.GetEnvironmentVariable("RpcTransmitterAccessKey", EnvironmentVariableTarget.Machine);
            TransceiverSettings.RpcTransmitter.ResourceName = "rpc_test";
            TransceiverSettings.RpcTransmitter.TimeoutInSeconds = 30;

            RpcTransceiver = new ServiceBusProxyClient(TransceiverSettings, AsyncCoupler, Logger);
            RpcTransceiver.StartAsync().Wait();

            ProxySettings = new ProxySettings
            {
                OrganizationId = OrganizationId,
                InstanceId = InstanceId
            };

            ProxyFactory = new ProxyFactory(TransceiverSettings, RpcTransceiver, AsyncCoupler, Logger);

            DeviceManagementRepoProxy = ProxyFactory.Create<IDeviceManagementRepo>(ProxySettings);
            DeviceArchiveRepoProxy = ProxyFactory.Create<IDeviceArchiveRepo>(ProxySettings);
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
                DeviceType = EntityHeader.Create("fff", "ddd"),
                Status = EntityHeader<DeviceStates>.Create(DeviceStates.New)
            };
        }

        public static DeviceRepository CreateDeviceRespository()
        {
            return JsonConvert.DeserializeObject<DeviceRepository>(Properties.Resources.DeviceRepository);
            //var _user = EntityHeader.Create("3367B1522AF441F39238A85A80B94D33", "Test");
            //var _org = EntityHeader.Create("C8AD4589F26842E7A1AEFBAEFC979C9B", "Test");

            //var repo = new DeviceRepository()
            //{
            //    Id = "04419F4A084A46F0988B2B61D92F0379",
            //    CreatedBy = _user,
            //    CreationDate = DateTime.UtcNow.ToJSONString(),
            //    LastUpdatedBy = _user,
            //    LastUpdatedDate = DateTime.UtcNow.ToJSONString(),
            //    OwnerOrganization = _org,
            //    Name = "MarkTest",
            //    Key = "marktest",
            //    Subscription = EntityHeader.Create("650cf116-0ab9-41d9-817c-1a773e5769b7", "idtext"),
            //    DeviceCapacity = EntityHeader.Create("dev123", "capac"),
            //    StorageCapacity = EntityHeader.Create("storage", "storage"),
            //    RepositoryType = EntityHeader<RepositoryTypes>.Create(RepositoryTypes.NuvIoT)
            //};

            //return repo;
        }
    }
}
