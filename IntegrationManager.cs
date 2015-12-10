using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TCDefectIntegration {
    public class IntegrationManager {
        private Integrator integrator = IntegratorFactory.GetIntegrator();

        protected virtual Integrator Integrator {
            get {
                return integrator;
            }
        }

        #region Cout

        private readonly TextWriter cout = Console.Out;

        private TextWriter Cout {
            get {
                return cout;
            }
        }

        #endregion

        #region  Create, Open and GetStates

        private XmlReaderSettings xmlReaderSettings = null;

        private XmlReaderSettings XmlReaderSettings {
            get {
                if (xmlReaderSettings == null) {
                    xmlReaderSettings = new XmlReaderSettings();
                    xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
                    xmlReaderSettings.IgnoreWhitespace = true;
                    xmlReaderSettings.IgnoreComments = true;
                    xmlReaderSettings.CheckCharacters = false;
                }
                return xmlReaderSettings;
            }
        }

        private const string TagDefectIntegration = "TCDefectIntegration";

        private const string TagChangeRequestId = "Log-ChangeRequestId";

        public virtual int CreateDefect( string dataFileName ) {
            XmlReader reader = null;

            try {
                reader = XmlReader.Create(dataFileName, this.XmlReaderSettings);
                reader.Read(); //move to first line
                if (reader.Name != "xml") {
                    throw new XmlException("Invalid XML");
                }
                reader.Read();
                Dictionary<string, string> defectInfos = new Dictionary<string, string>();

                reader.ReadStartElement(TagDefectIntegration);
                while(reader.NodeType != XmlNodeType.None) {
                    if (reader.NodeType == XmlNodeType.EndElement) {
                        reader.ReadEndElement();
                    }
                    else {
                        string name = reader.GetAttribute("Name");
                        string key;

                        if (String.IsNullOrEmpty(name)) {
                            key = reader.Name;
                        }
                        else {
                            key = reader.Name + " " + name;
                        }
                        string value = reader.ReadElementString();

                        defectInfos[key] = value;
                    }
                }
                return CreateDefectNow(defectInfos);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return -1;
            }
            finally {
                if (reader != null) {
                    reader.Close();
                }
            }
        }

        public virtual int OpenDefect( string dataFileName ) {
            try {
                string changeRequestId;
                XmlReader reader;
                using (reader = XmlReader.Create(dataFileName, this.XmlReaderSettings)) {
                    reader.Read(); //move to first line
                    if (reader.Name != "xml") {
                        throw new XmlException("Invalid XML");
                    }
                    reader.Read();
                    reader.ReadStartElement(TagDefectIntegration);
                    changeRequestId = reader.ReadElementString(TagChangeRequestId);

                    reader.ReadEndElement();
                }
                return OpenDefectNow(changeRequestId);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        public virtual int GetStatesForDefects( string dataFileName ) {
            try {
                List<string> defectIds = GetDefectIds(dataFileName);

                return GetStatesForDefectsNow(defectIds);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        public virtual int GetInfosForDefects( string dataFileName ) {
            try {
                List<string> defectIds = GetDefectIds(dataFileName);

                return GetInfosForDefectsNow(defectIds);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        private static string Encode( string s ) {
            return s.Replace("\\", "\\\\").Replace(":", "\\:");
        }

        private List<string> GetDefectIds( string fileName ) {
            XmlReader reader = null;

            try {
                reader = XmlReader.Create(fileName, XmlReaderSettings);
                reader.Read();
                if (!reader.Name.Equals("xml")) {
                    throw new XmlException("Invalid XML");
                }
                reader.Read();
                List<string> defectIds = new List<string>();

                reader.ReadStartElement(TagDefectIntegration);
                while(reader.NodeType != XmlNodeType.None) {
                    if (reader.NodeType == XmlNodeType.EndElement) {
                        reader.ReadEndElement();
                    }
                    else {
                        string defectId = reader.ReadElementString(TagChangeRequestId);

                        defectIds.Add(defectId);
                    }
                }
                return defectIds;
            }
            finally {
                if (reader != null) {
                    reader.Close();
                }
            }
        }

        #endregion

        #region Integrator-Methods

        protected virtual int CreateDefectNow( Dictionary<string, string> defectInfos ) {
            string newDefectId = Integrator.CreateDefect(defectInfos);
            Cout.Write(newDefectId);
            return 0;
        }

        protected virtual int OpenDefectNow( string crqId ) {
            Integrator.OpenDefect(crqId);
            return 0;
        }

        protected virtual int GetStatesForDefectsNow( List<string> crqIds ) {
            Dictionary<string, string> defectStates = Integrator.GetStatesForDefects(crqIds);
            foreach (KeyValuePair<string, string> kvPair in defectStates) {
                string crqId = kvPair.Key;
                string crqState = kvPair.Value;
                Cout.WriteLine(Encode(crqId) + ":" + Encode(crqState));
            }
            return 0;
        }

        protected virtual int GetInfosForDefectsNow( List<string> crqIds ) {
            Dictionary<string, string> defectStates = Integrator.GetInfosForDefects(crqIds);
            foreach (KeyValuePair<string, string> kvPair in defectStates) {
                string crqId = kvPair.Key;
                string crqInfo = kvPair.Value;
                Cout.WriteLine(Encode(crqId) + ":" + Encode(crqInfo));
            }
            return 0;
        }

        #endregion
    }
}