using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Models.Resources;

namespace LagoVista.IoT.DeviceManagement
{
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DataStreamField_Title, DeviceManagementResources.Names.DataStreamField_Help, DeviceManagementResources.Names.DataStreamField_Help, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources))]
    public class DataStreamField : IValidateable
    {
        [FormField(LabelResource: DeviceManagementResources.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Name { get; set;}

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Key, HelpResource: DeviceManagementResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Description, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string Description { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Notes, FieldType: FieldTypes.MultiLineText,  ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string Notes { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStreamField_FieldName, HelpResource: DeviceManagementResources.Names.DataStreamField_FieldName_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string FieldName { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStreamField_DataType, EnumType: (typeof(ParameterTypes)), HelpResource: DeviceManagementResources.Names.DataStreamField_DataType_Help, FieldType: FieldTypes.Picker, WaterMark:DeviceManagementResources.Names.DataStreamField_DataType_Select, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public EntityHeader<ParameterTypes> FieldType { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStreamField_UnitSet, FieldType: FieldTypes.EntityHeaderPicker, WaterMark: DeviceManagementResources.Names.DataStreamField_UnitSet_Watermark,  ResourceType: typeof(DeviceManagementResources))]
        public EntityHeader<UnitSet> UnitSet { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStreamField_StateSet, FieldType: FieldTypes.EntityHeaderPicker, WaterMark: DeviceManagementResources.Names.DataStreamField_StateSet_Watermark, ResourceType: typeof(DeviceManagementResources))]
        public EntityHeader<StateSet> StateSet { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStreamField_IsRequired, HelpResource: DeviceManagementResources.Names.DataStreamField_IsRequired_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool IsRequired { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStreamField_NumberDecimalPoints, HelpResource: DeviceManagementResources.Names.DataStreamField_NumberDecimalPoints_Help, FieldType: FieldTypes.Integer,ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public int? NumberDecimalPoint { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStreamField_MinValue, HelpResource: DeviceManagementResources.Names.DataStreamField_MinValue_Help, FieldType: FieldTypes.Decimal,  ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public double? MinValue { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DataStreamField_MaxValue, HelpResource: DeviceManagementResources.Names.DataStreamField_MaxValue_Help, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public double? MaxValue { get; set; }

        [CustomValidator]
        public void Validate(ValidationResult result)
        {
            /* If field type isn't specified, it won't be valid so can't check the rest */
            if(!EntityHeader.IsNullOrEmpty(FieldType))
            {
               switch(FieldType.Value)
                {
                    case ParameterTypes.ValueWithUnit:
                        if (EntityHeader.IsNullOrEmpty(UnitSet)) result.Errors.Add(new ErrorMessage($"Unit Set is required on field {Name}"));
                        break;
                    case ParameterTypes.State:
                        if (EntityHeader.IsNullOrEmpty(StateSet)) result.Errors.Add(new ErrorMessage($"State Set is required on field {Name}"));
                        break;
                }
            }
        }

    }
}
