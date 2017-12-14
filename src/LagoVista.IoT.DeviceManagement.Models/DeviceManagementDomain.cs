using LagoVista.Core.Attributes;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Core
{
    [DomainDescriptor]
    public class DeviceManagementDomain
    {        
        public const string DeviceManagement = "Device Managent";

        [DomainDescription(DeviceManagement)]
        public static DomainDescription DeploymentAdminDescription
        {
            get
            {
                return new DomainDescription()
                {
                    Description = "Models, Managers and Business Logics for the Management of Devices.",
                    DomainType = DomainDescription.DomainTypes.BusinessObject,
                    Name = "Device Management",
                    CurrentVersion = new LagoVista.Core.Models.VersionInfo()
                    {
                        Major = 0,
                        Minor = 8,
                        Build = 001,
                        DateStamp = new DateTime(2016, 12, 20),
                        Revision = 1,
                        ReleaseNotes = "Initial unstable preview"
                    }
                };
            }
        }
    }
}
