using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.IoT.DeviceManagement
{
    public enum DataStreamTypes
    {
        [EnumLabel(DataStream.StreamType_AWS_ElasticSearch, DeviceManagementResources.Names.DataStream_StreamType_AWS_ElasticSearch, typeof(DeviceManagementResources))]
        AWSElasticSearch,
        [EnumLabel(DataStream.StreamType_AWS_S3, DeviceManagementResources.Names.DataStream_StreamType_AWS_S3, typeof(DeviceManagementResources))]
        AWSS3,
        [EnumLabel(DataStream.StreamType_AzureBlob, DeviceManagementResources.Names.DataStream_StreamType_AzureBlob, typeof(DeviceManagementResources))]
        AzureBlob,
        [EnumLabel(DataStream.StreamType_AzureBlob_Managed, DeviceManagementResources.Names.DataStream_StreamType_AzureBlob_Managed, typeof(DeviceManagementResources))]
        AzureBlob_Managed,
        [EnumLabel(DataStream.StreamType_AzureEventHub, DeviceManagementResources.Names.DataStream_StreamType_AzureEventHub, typeof(DeviceManagementResources))]
        AzureEventHub,
        [EnumLabel(DataStream.StreamType_AzureEventHub_Managed, DeviceManagementResources.Names.DataStream_StreamType_AzureEventHub_Managegd, typeof(DeviceManagementResources))]
        EventEventHub_Managed,
        [EnumLabel(DataStream.StreamType_AzureTableStorage, DeviceManagementResources.Names.DataStream_StreamType_TableStorage, typeof(DeviceManagementResources))]
        AzureTableStorage,
        [EnumLabel(DataStream.StreamType_AzureTableStorage_Managed, DeviceManagementResources.Names.DataStream_StreamType_TableStorage_Managed, typeof(DeviceManagementResources))]
        AzureTableStorage_Managed,
        [EnumLabel(DataStream.StreamType_DataLake, DeviceManagementResources.Names.DataStream_StreamType_DataLake, typeof(DeviceManagementResources))]
        AzureDataLake,
        [EnumLabel(DataStream.StreamType_SQLServer, DeviceManagementResources.Names.DataStream_StreamType_SQLServer, typeof(DeviceManagementResources))]
        SQLServer
    }

    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DataStream_Title, DeviceManagementResources.Names.DataStream_Help, DeviceManagementResources.Names.DataStream_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeviceManagementResources))]
    public class DataStream : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IKeyedEntity, INoSQLEntity, IValidateable, IFormDescriptor
    {
        public const string StreamType_AWS_S3 = "awss3";
        public const string StreamType_AWS_ElasticSearch = "awselasticsearch";
        public const string StreamType_AzureBlob = "azureblob";
        public const string StreamType_AzureBlob_Managed = "azureblobmanaged";
        public const string StreamType_AzureTableStorage = "azuretablestorage";
        public const string StreamType_AzureTableStorage_Managed = "azuretablestoragemanaged";
        public const string StreamType_DataLake = "azuredatalake";
        public const string StreamType_AzureEventHub = "azureeventhub";
        public const string StreamType_AzureEventHub_Managed = "azureeventhubmanaged";
        public const string StreamType_SQLServer = "sqlserver";

        public DataStream()
        {
            Fields = new List<DataStreamField>();
        }

        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Key, HelpResource: DeviceManagementResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Key { get; set; }

        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStream_StreamType, EnumType: typeof(DataStreamTypes), FieldType: FieldTypes.Picker, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.DataStream_StreamType_Select, IsRequired: true)]
        public EntityHeader<DataStreamTypes> StreamType { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStream_ConnectionString, HelpResource: DeviceManagementResources.Names.DataStream_ConnectionString_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string ConnectionString { get; set; }

        public string SecureConnectionStringId { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.DataStream_Fields, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeviceManagementResources))]
        public List<DataStreamField> Fields { get; set; }

        public DataStreamSummary CreateSummary()
        {
            return new DataStreamSummary()
            {
                Description = Description,
                Id = Id,
                Name = Name,
                IsPublic = IsPublic,
                Key = Key
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(DataStream.Name),
                nameof(DataStream.Key),
                nameof(DataStream.Description),
            };
        }

        [CustomValidator]
        public void Validate(ValidationResult result)
        {
            if (Fields.Select(fld => fld.Key).Distinct().Count() != Fields.Count) result.Errors.Add(new ErrorMessage("Keys on fields must be unique"));
            if (Fields.Select(fld => fld.FieldName).Distinct().Count() != Fields.Count) result.Errors.Add(new ErrorMessage("Field Names on fields must be unique"));
        }
    }

    public class DataStreamSummary : SummaryData
    {
        public string StreamType { get; set; }
        public string StreamTypeKey { get; set; }
    }
}
