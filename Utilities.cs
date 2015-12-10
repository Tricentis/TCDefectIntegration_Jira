using System.Configuration;

namespace TCDefectIntegration {
    public static class Utilities {
        public static void ProtectUserSettings() {
            ToggleUserSettingsProtection(true);
        }

        public static void UnprotectUserSettings() {
            ToggleUserSettingsProtection(false);
        }

        private static void ToggleUserSettingsProtection( bool protect ) {
            const string StrProvider = "DataProtectionConfigurationProvider";

            var oConfiguration = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.PerUserRoamingAndLocal);

            if (oConfiguration != null) {
                bool blnChanged = false;

                var oSection = oConfiguration.GetSection("userSettings/TCDefectIntegration.Properties.Settings");

                if (oSection != null) {
                    if ((!(oSection.ElementInformation.IsLocked)) && (!(oSection.SectionInformation.IsLocked))) {
                        if (protect) {
                            if (!(oSection.SectionInformation.IsProtected)) {
                                blnChanged = true;

                                oSection.SectionInformation.ProtectSection(StrProvider);
                            }
                        }
                        else {
                            if (oSection.SectionInformation.IsProtected) {
                                blnChanged = true;

                                oSection.SectionInformation.UnprotectSection();
                            }
                        }
                    }

                    if (blnChanged) {
                        oSection.SectionInformation.ForceSave = true;

                        oConfiguration.Save(ConfigurationSaveMode.Full);
                    }
                }
            }
        }
    }
}