using LagoVista.Core;
using LagoVista.Core.Validation;
using LagoVista.Core.Attributes;
using System;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using LagoVista.Core.Models;
using System.Collections.Generic;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    // TODO: Not implemented, need to really think through how to associate users with devices.

    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DeviceOwner_Title, DeviceManagementResources.Names.DeviceOwner_Description,
        DeviceManagementResources.Names.DeviceOwner_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeviceManagementResources), Icon: "icon-ae-error-1")]
    public class DeviceOwner : IValidateable
    {
        public DeviceOwner()
        {
            Id = Guid.NewGuid().ToId();
        }

        public string Id { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceOwner_FirstName, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true)]
        public string FirstName { get; set; }
        [FormField(LabelResource: DeviceManagementResources.Names.DeviceOwner_FirstName, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true)]
        public string LastName { get; set; }
        [FormField(LabelResource: DeviceManagementResources.Names.DeviceOwner_EmailAddress, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true)]
        public string EmailAddress { get; set; }
        
        [FormField(LabelResource: DeviceManagementResources.Names.DeviceOwner_PhoneNumber, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true)]
        public string PhoneNumber { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceOwner_Password, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true)]
        public string Password { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceOwner_Password, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true)]
        public string PasswordConfirm { get; set; }
        
        public string PasswordHash { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceOwner_Devices, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true)]
        public List<DeviceOwnerDevices> Devices { get; set; } = new List<DeviceOwnerDevices>();

        [CustomValidator]
        public void Validte(ValidationResult result, Actions action)
        {
            if (String.IsNullOrEmpty(FirstName))
                result.AddUserError("Please provide your name.");

            if (String.IsNullOrEmpty(LastName))
                result.AddUserError("Please provide your name.");
        }
    }

    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DeviceOwnersDevices_Title, DeviceManagementResources.Names.DeviceOwnersDevices_Description,
        DeviceManagementResources.Names.DeviceOwner_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeviceManagementResources), Icon: "icon-ae-error-1")]
    public class DeviceOwnerDevices
    {


        [FormField(LabelResource: DeviceManagementResources.Names.DeviceOwner_Password, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false, IsRequired: true)]
        public EntityHeader DeviceRepository { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceOwner_Password, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false, IsRequired: true)]
        public EntityHeader Device { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceOwner_Password, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false, IsRequired: true)]
        public EntityHeader Organization { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceOwner_Password, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false, IsRequired: true)]
        public string DeviceId { get; set; }
    }
}
