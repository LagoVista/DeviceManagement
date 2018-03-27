using LagoVista.Core.Attributes;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Models.Resources;

namespace LagoVista.IoT.DeviceManagement
{
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DataStreamField_Title, DeviceManagementResources.Names.DataStreamField_Help, DeviceManagementResources.Names.DataStreamField_Help, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources))]
    public class DataStreamField
    {
        [FormField(LabelResource: DeviceManagementResources.Names.Common_Name, FieldType: FieldTypes.Text, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Name { get; set;}

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Key, HelpResource: DeviceManagementResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Description, FieldType: FieldTypes.Text, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string Description { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Notes, FieldType: FieldTypes.Text, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string Notes { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStreamField_FieldName, HelpResource: DeviceManagementResources.Names.DataStreamField_FieldName_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string FieldName { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStreamField_IsRequired, HelpResource: DeviceManagementResources.Names.DataStreamField_IsRequired_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool IsRequired { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStreamField_NumberDecimalPoints, HelpResource: DeviceManagementResources.Names.DataStreamField_NumberDecimalPoints_Help, FieldType: FieldTypes.Integer,ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public int? NumberDecimalPoint { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStreamField_MinValue, HelpResource: DeviceManagementResources.Names.DataStreamField_MinValue_Help, FieldType: FieldTypes.Decimal,  ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public double? MinValue { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStreamField_MaxValue, HelpResource: DeviceManagementResources.Names.DataStreamField_MaxValue_Help, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public double? MaxValue { get; set; }
    }
}
