using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Resources;
using System;
using LagoVista.Core;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public enum DeviceUnitCapacityTypes
    {
        [EnumLabel("5", DeviceManagementResources.Names.Device_Capacity_5_Units, typeof(DeviceManagementResources))]
        Units_5,
        [EnumLabel("100", DeviceManagementResources.Names.Device_Capacity_100_Units, typeof(DeviceManagementResources))]
        Units_100,
        [EnumLabel("500", DeviceManagementResources.Names.Device_Capacity_500_Units, typeof(DeviceManagementResources))]
        Units_500,
        [EnumLabel("1k", DeviceManagementResources.Names.Device_Capacity_1000_Units, typeof(DeviceManagementResources))]
        Units_1000,
        [EnumLabel("5k", DeviceManagementResources.Names.Device_Capacity_5000_Units, typeof(DeviceManagementResources))]
        Units_5000,
        [EnumLabel("10k", DeviceManagementResources.Names.Device_Capacity_10000_Units, typeof(DeviceManagementResources))]
        Units_10000,
        [EnumLabel("50k", DeviceManagementResources.Names.Device_Capacity_50000_Units, typeof(DeviceManagementResources))]
        Units_50000,
        [EnumLabel("100k", DeviceManagementResources.Names.Device_Capacity_100000_Units, typeof(DeviceManagementResources))]
        Units_100000,
        [EnumLabel("500k", DeviceManagementResources.Names.Device_Capacity_500000_Units, typeof(DeviceManagementResources))]
        Units_500000,
        [EnumLabel("1m", DeviceManagementResources.Names.Device_Capacity_1000000_Units, typeof(DeviceManagementResources))]
        Units_1000000,
        [EnumLabel("custom", DeviceManagementResources.Names.Device_Capacity_Custom, typeof(DeviceManagementResources))]
        Units_Custom,
    }

    public enum DeviceStorageCapacityTypes
    {
        [EnumLabel("20MB", DeviceManagementResources.Names.Device_Storage_20MB, typeof(DeviceManagementResources))]
        Size_20MB,
        [EnumLabel("1GB", DeviceManagementResources.Names.Device_Storage_1GB, typeof(DeviceManagementResources))]
        Size_1GB,
        [EnumLabel("20GB", DeviceManagementResources.Names.Device_Storage_20GB, typeof(DeviceManagementResources))]
        Size_20GB,
        [EnumLabel("100GB", DeviceManagementResources.Names.Device_Storage_100GB, typeof(DeviceManagementResources))]
        Size_100GB,
        [EnumLabel("500GB", DeviceManagementResources.Names.Device_Storage_500GB, typeof(DeviceManagementResources))]
        Size_500GB,
        [EnumLabel("1TB", DeviceManagementResources.Names.Device_Storage_1TB, typeof(DeviceManagementResources))]
        Size_1TB,
        [EnumLabel("5TB", DeviceManagementResources.Names.Device_Storage_5TB, typeof(DeviceManagementResources))]
        Size_5TB,
        [EnumLabel("custom", DeviceManagementResources.Names.Device_Storage_Custom, typeof(DeviceManagementResources))]
        Size_Custom,
    }


    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.Device_RepoTitle, Resources.DeviceManagementResources.Names.Device_Repo_Help, Resources.DeviceManagementResources.Names.Device_Repo_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources))]
    public class DeviceRepository : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IKeyedEntity, INoSQLEntity, IValidateable, IEntityHeaderEntity
    {
        public DeviceRepository()
        {
            AuthKey1 = Guid.NewGuid().ToId() + Guid.NewGuid().ToId() + Guid.NewGuid().ToId();
            AuthKey2 = Guid.NewGuid().ToId() + Guid.NewGuid().ToId() + Guid.NewGuid().ToId();
        }

        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        public ConnectionSettings DeviceStorageSettings { get; set; }
        public ConnectionSettings DeviceArchiveStorageSettings { get; set; }    
        public ConnectionSettings PEMStorageSettings { get; set; }

        [FormField(LabelResource: Resources.DeviceManagementResources.Names.Common_Key, HelpResource: Resources.DeviceManagementResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: Resources.DeviceManagementResources.Names.Device_Repo_Subscription, WaterMark: Resources.DeviceManagementResources.Names.Device_Repo_SubscriptionSelect, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: true)]
        public EntityHeader Subscription { get; set; }

        [FormField(LabelResource: Resources.DeviceManagementResources.Names.Device_Repo_AuthKey1, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public String AuthKey1 { get; set; }

        [FormField(LabelResource: Resources.DeviceManagementResources.Names.Device_Repo_AuthKey1, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public String AuthKey2 { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_StorageCapacity, EnumType: (typeof(DeviceStorageCapacityTypes)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Repo_StorageCapacity_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<DeviceStorageCapacityTypes> StorageCapacity { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_UnitCapacity, EnumType: (typeof(DeviceUnitCapacityTypes)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Repo_UnitCapacity_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<DeviceUnitCapacityTypes> DeviceCapacity { get; set; }

        public IEntityHeader ToEntityHeader()
        {
            return new EntityHeader()
            {
                Id = Id,
                Text = Name,
            };
        }

        public DeviceRepositorySummary CreateSummary()
        {
            return new DeviceRepositorySummary()
            {
                Id = Id,
                IsPublic = IsPublic,
                Key = Key,
                Name = Name,
                Description = Description
            };
        }
    }

    public class DeviceRepositorySummary : SummaryData
    {

    }
}
