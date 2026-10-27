// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f0257013c413e81b387b8cf2503e129c1fe85bccbadcdb4d7aec51e4f900705b
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Utils;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Models.Orgs;
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

            ProxySettings = new ProxySettings
            {
                OrganizationId = OrganizationId,
                InstanceId = InstanceId
            };

            //ProxyFactory = new ProxyFactory(TransceiverSettings, RpcTransceiver, AsyncCoupler, Logger);

            //DeviceManagementRepoProxy = ProxyFactory.Create<IDeviceManagementRepo>(ProxySettings);
            //DeviceArchiveRepoProxy = ProxyFactory.Create<IDeviceArchiveRepo>(ProxySettings);
            //DeviceLogRepo = ProxyFactory.Create<IDeviceLogRepo>(ProxySettings);
            //DevicePEMRepo = ProxyFactory.Create<IDevicePEMRepo>(ProxySettings);

            //DeviceGroupRepo = ProxyFactory.Create<IDeviceGroupRepo>(ProxySettings);
            //DeviceMediaRepo = ProxyFactory.Create<IDeviceMediaRepo>(ProxySettings);
            //DeviceMediaItemRepo = ProxyFactory.Create<IDeviceMediaItemRepo>(ProxySettings);
            //DeviceMediaRepoRemote = ProxyFactory.Create<IDeviceMediaRepoRemote>(ProxySettings);
            //DeviceMediaItemRepoRemote = ProxyFactory.Create<IDeviceMediaItemRepoRemote>(ProxySettings);
        }

        public static Device CreateDevice(string deviceId = "dev1234")
        {
            return new Device()
            {
                Id = Guid.NewGuid().ToId(),
                CreationDate = UtcTimestamp.Now,
                LastUpdatedDate = UtcTimestamp.Now,
                CreatedBy = EntityHeader.Create(Guid.NewGuid().ToId(), "abc123"),
                LastUpdatedBy = EntityHeader.Create(Guid.NewGuid().ToId(), "abc123"),
                OwnerOrganization = EntityHeader.Create(OrganizationId, "test organization"),
                Location = EntityHeader<OrgLocation>.Create(LocationId, "test location", null),
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
