// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 436cf8d38a146e30ba24db127127785c2d73ecbbccf832eb87f40bf6889f4439
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core.Models.ML;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Models.Resources;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.AttributeValue_Title, DeviceManagementResources.Names.AttributeValue_Help, DeviceManagementResources.Names.AttributeValue_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources))]
    public class AttributeValue
    {
        [FormField(LabelResource: DeviceManagementResources.Names.AttributeValue_LastUpdated, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public string LastUpdated { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.AttributeValue_LastUpdatedBy, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable:false)]
        public string LastUpdatedBy { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.AttributeValue_Name, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public string Name { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.AttributeValue_Key, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public string Key { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.AttributeValue_Value, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public string Value { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.AttributeValue_Inference, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public MLInference Inference { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.AttributeValue_Type, FieldType: FieldTypes.Picker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public EntityHeader<ParameterTypes> AttributeType { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.AttributeValue_Unit, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public EntityHeader<UnitSet> UnitSet {get; set;}

        [FormField(LabelResource: DeviceManagementResources.Names.AttributeValue_State, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public EntityHeader<StateSet> StateSet { get; set; }
    
    }
}