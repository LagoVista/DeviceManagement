// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0a726cedbe2c3449a8b57f40f6a89e8f1c6b72fe6051973f90d16876877cff9a
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using System;
using System.Collections.Generic;
using LagoVista.Core.Interfaces;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public enum FirmwareRevisionStatus
    {
        [EnumLabel(FirmwareRevision.Prototype, DeviceManagementResources.Names.FirmwareRevision_Status_Prototype, typeof(DeviceManagementResources))]
        Prototype,

        [EnumLabel(FirmwareRevision.Alpha, DeviceManagementResources.Names.FirmwareRevision_Status_Alpha, typeof(DeviceManagementResources))]
        Alpha,

        [EnumLabel(FirmwareRevision.Beta, DeviceManagementResources.Names.FirmwareRevision_Status_Beta, typeof(DeviceManagementResources))]
        Beta,

        [EnumLabel(FirmwareRevision.Production, DeviceManagementResources.Names.FirmwareRevision_Status_Production, typeof(DeviceManagementResources))]
        Production,

        [EnumLabel(FirmwareRevision.Obsolete, DeviceManagementResources.Names.FirmwareRevision_Status_Obsolete, typeof(DeviceManagementResources))]
        Obsolete,
    }

    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.FirmwareRevision_Title, DeviceManagementResources.Names.FirmwareRevision_Help, 
        DeviceManagementResources.Names.FirmwareRevision_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources), Icon: "icon-fo-firmware", 
        FactoryUrl: "/api/firmware/revision/factory")]
    public class FirmwareRevision : IFormDescriptor
    {
        public FirmwareRevision()
        {
            Status = EntityHeader<FirmwareRevisionStatus>.Create(FirmwareRevisionStatus.Prototype);
            TimeStamp = DateTime.UtcNow.ToJSONString();
            Id = Guid.NewGuid().ToId();
        }

        public const string Prototype = "prototype";
        public const string Alpha = "alpha";
        public const string Beta = "beta";
        public const string Production = "production";
        public const string Obsolete = "obsolete";

        public String Id { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.FirmwareRevision_Version, FieldType: FieldTypes.Text, ValidationRegEx: @"^\d+\.\d+\.\d+$", RegExValidationMessageResource: DeviceManagementResources.Names.FirmwareRevision_VersionCodeRegEx, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string VersionCode { get; set; }

        public string TimeStamp { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.FirmwareRevision_Notes, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeviceManagementResources))]
        public string Notes { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.FirmwareRevision_Status, EnumType: (typeof(FirmwareRevisionStatus)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.FirmwareRevision_Status_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<FirmwareRevisionStatus> Status { get; set; }

        public string File { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.FirmwareRevision_File, FieldType: FieldTypes.FileUpload, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.FirmwareRevision_Status_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader BinaryFile { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.FirmwareRevision_OtaFile, FieldType: FieldTypes.FileUpload, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.FirmwareRevision_Status_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader OtaBinaryFile { get; set; }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(VersionCode),
                nameof(Notes),
                nameof(Status),
                nameof(BinaryFile),
                nameof(OtaBinaryFile)
            };
        }

    }
}
