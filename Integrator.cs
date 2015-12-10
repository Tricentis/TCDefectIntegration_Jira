using System;
using System.Collections.Generic;
using System.Reflection;

using TCDefectIntegration.Properties;

namespace TCDefectIntegration {
    public abstract class Integrator {
        private const string ExecutionListProperty = "ExecutionList-Property ";

        /// <summary>
        /// Creates a new Defect in the integrated ChangeRequest-System
        /// fills the fields by the supplied defectInfos and returns the new defect-id
        /// </summary>
        /// <param name="defectInfos">Dictionary of all available Infos (Key=Info-Label, Value = Info-Value)</param>
        /// <returns>the defect-id of the created defect</returns>
        public abstract string CreateDefect( Dictionary<string, string> defectInfos );

        /// <summary>
        /// Opens the Defect in the integrated ChangeRequest-Tool
        /// </summary>
        /// <param name="defectId">unique id of the defect to open</param>
        public abstract void OpenDefect( string defectId );

        /// <summary>
        /// Returns the State of the supplied defects
        /// </summary>
        /// <param name="defectIds">List of all defect-ids to return defect-states</param>
        /// <returns>Dictionary of strings, Key is defect-id, Value is defect-state</returns>
        public abstract Dictionary<string, string> GetStatesForDefects( List<string> defectIds );

        /// <summary>
        /// Returns Info of the supplied defects
        /// </summary>
        /// <param name="defectIds">List of all defect-ids to return defect-infos</param>
        /// <returns>Dictionary of strings, Key is defect-id, Value is defect-info</returns>
        public abstract Dictionary<string, string> GetInfosForDefects( List<string> defectIds );

        /// <summary>
        /// Returns the default value for a defect property.
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="defectInfos">List of defect infos</param>
        /// <returns>Default value of the defect property</returns>
        /// <remarks>The values are retrieved in the following order:
        /// <list type="bullet">
        /// <item>ExecutionList-Property in defectInfos</item>
        /// <item>TCDefectIntegration.exe Settings</item>
        /// </list>
        /// The first value found is returned.
        /// </remarks>
        protected string GetDefectIntegrationSetting( string name, Dictionary<string, string> defectInfos ) {
            if (defectInfos.ContainsKey(ExecutionListProperty + name)) {
                return defectInfos[ExecutionListProperty + name];
            }
            PropertyInfo[] props = Settings.Default.GetType().GetProperties();
            foreach (PropertyInfo property in props) {
                if (String.Compare(property.Name, name) == 0) {
                    return property.GetValue(Settings.Default, null).ToString();
                }
            }
            throw new ApplicationException("No configuration setting with name »" + name + "« found");
        }

        /// <summary>
        /// Returns a list of custom defect properties.
        /// </summary>
        /// <returns>List of custom properties</returns>
        protected List<CustomDefectProperty> GetCustomDefectProperties( Dictionary<string, string> defectInfos ) {
            List<CustomDefectProperty> result = new List<CustomDefectProperty>();

            string[] customProperties =
                GetDefectIntegrationSetting("CustomDefectProperties", defectInfos)
                    .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string customProperty in customProperties) {
                CustomDefectProperty property = new CustomDefectProperty();

                string propertyNameID;

                int position = customProperty.IndexOf('=');
                if (position > 0) {
                    property.value = customProperty.Substring(position + 1);
                    propertyNameID = customProperty.Substring(0, position);
                }
                else {
                    propertyNameID = customProperty;
                }
                position = propertyNameID.IndexOf(':');
                if (position > 0) {
                    property.name = propertyNameID.Substring(0, position);
                    property.id = propertyNameID.Substring(position + 1);
                }
                else {
                    property.name = property.id = propertyNameID;
                }

                if (defectInfos.ContainsKey(ExecutionListProperty + property.name)) {
                    property.value = defectInfos[ExecutionListProperty + property.name];
                }

                if (property.name != string.Empty && property.value != null) {
                    result.Add(property);
                }
            }
            return result;
        }

        protected struct CustomDefectProperty {
            public string id;

            public string name;

            public string value;
        }
    }
}