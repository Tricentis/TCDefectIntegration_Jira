using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Microsoft.Win32;

using TCDefectIntegration.Properties;

namespace TCDefectIntegration {
    public class JiraIntegrator : Integrator {
        private const String XmlObjectTagName = "XmlObject";

        private const String LogInfoAttributeName = "LogInfo";

        private frmLogin frmLogin;

        private JiraSoapServiceService jira;

        private string jiraToken;

        private string password = Settings.Default.Password;

        private string proxyPassword = Settings.Default.ProxyPassword;

        private string proxyUserName = Settings.Default.ProxyUser;

        private string userName = Settings.Default.User;

        private bool ShowLoginDialog {
            get {
                if (Settings.Default.UseProxy) {
                    if (proxyPassword == String.Empty) {
                        if (proxyUserName == String.Empty) {
                            return true;
                        }
                    }
                }

                if (password == String.Empty) {
                    if (userName == String.Empty) {
                        return true;
                    }
                }
                return false;
            }
        }

        public override string CreateDefect( Dictionary<string, string> defectInfos ) {
            Connect();

            RemoteIssue issue = new RemoteIssue();
            RemotePriority[] priorities = jira.getPriorities(jiraToken);
            RemoteIssueType[] issueTypes = jira.getIssueTypes(jiraToken);
            jira.getProjectRoles(jiraToken);

            issue.created = DateTime.Now;
            issue.summary = "TOSCA[" + defectInfos["Workspace-User"] + "]: Error in Testcase \""
                            + defectInfos["TestCase-Name"] + "\"";
            string description = defectInfos["Log-Description"];
            if (!String.IsNullOrEmpty(description)) {
                issue.description = description;
            }
            else {
                String compressedLog = defectInfos["CompressedLogDetails"];
                issue.description = String.IsNullOrEmpty(compressedLog)
                    ? String.Empty
                    : GetLogInfoFromCompressedLogDetails(compressedLog);
            }
            issue.assignee = GetDefectIntegrationSetting("DefectAssignee", defectInfos);
            if (issue.assignee == string.Empty) {
                issue.assignee = "-1";
            }

            issue.project = GetDefectIntegrationSetting("DefectProject", defectInfos);
            issue.priority = GetDefectIntegrationSetting("DefectPriority", defectInfos);
            foreach (RemotePriority priority in priorities) {
                if (priority.name == issue.priority) {
                    issue.priority = priority.id;
                    break;
                }
            }
            issue.status = GetDefectIntegrationSetting("DefectStatus", defectInfos);
            issue.type = GetDefectIntegrationSetting("DefectType", defectInfos);
            foreach (RemoteIssueType issueType in issueTypes) {
                if (issueType.name == issue.type) {
                    issue.type = issueType.id;
                    break;
                }
            }
            issue.components = GetComponents(GetDefectIntegrationSetting("DefectComponents", defectInfos));

            issue.customFieldValues = GetCustomDefectProperties(defectInfos).Select(property => new RemoteCustomFieldValue {
                                                                                                                               customfieldId = "customfield_" + property.id, values = property.value.Split(',')
                                                                                                                           }).ToArray();
            issue = jira.createIssue(jiraToken, issue);
            Disconnect();

            return issue != null ? issue.key : string.Empty;
        }

        public override void OpenDefect( string defectId ) {
            string url = Settings.Default.URL;
            if (!url.EndsWith("/")) {
                url += "/";
            }

            string defaultBrowserPath = GetDefaultBrowserPath();
            Process.Start(defaultBrowserPath, url + defectId);
        }

        public override Dictionary<string, string> GetStatesForDefects( List<string> defectIds ) {
            Dictionary<string, string> result = new Dictionary<string, string>();
            List<string> failedIds = new List<string>();

            Connect();

            RemoteStatus[] remoteStatuses = jira.getStatuses(jiraToken);
            Dictionary<string, string> statusNames = remoteStatuses.ToDictionary(remoteStatus => remoteStatus.id, remoteStatus => remoteStatus.name);

            foreach (string defectId in defectIds) {
                try {
                    RemoteIssue issue = jira.getIssue(jiraToken, defectId);
                    if (statusNames.ContainsKey(issue.status)) {
                        result.Add(defectId, statusNames[issue.status]);
                    }
                }
                catch {
                    failedIds.Add(defectId);
                }
            }

            Disconnect();

            if (failedIds.Count > 0) {
                MessageBox.Show(
                    "The status of the following Defects could not be retrieved:" + Environment.NewLine
                    + string.Join(",", failedIds.ToArray()),
                    "Synchronize Defect States");
            }

            return result;
        }

        public override Dictionary<string, string> GetInfosForDefects( List<string> defectIds ) {
            Dictionary<string, string> result = new Dictionary<string, string>();

            Connect();

            RemoteStatus[] remoteStatuses = jira.getStatuses(jiraToken);
            Dictionary<string, string> statusNames = remoteStatuses.ToDictionary(remoteStatus => remoteStatus.id, remoteStatus => remoteStatus.name);

            foreach (string defectId in defectIds) {
                try {
                    RemoteIssue issue = jira.getIssue(jiraToken, defectId);
                    if (statusNames.ContainsKey(issue.status)) {
                        result.Add(defectId, issue.created + " (" + statusNames[issue.status] + ")");
                    }
                }
                catch {
                    // ignored
                }
            }

            Disconnect();

            return result;
        }

        private void Connect() {
            bool loginWasSuccessful = false;
            bool showLoginDialog = ShowLoginDialog;
            Utilities.UnprotectUserSettings();
            InitJiraToken();

            while(!loginWasSuccessful) {
                if (showLoginDialog) {
                    frmLogin = new frmLogin();
                    PreSetJiraLoginData();

                    if (Settings.Default.UseProxy) {
                        PreSetProxyLoginData();
                    }
                    else {
                        DisableProxyControls();
                    }

                    if (frmLogin.ShowDialog() == DialogResult.Cancel) {
                        Utilities.ProtectUserSettings();
                        Environment.Exit(0);
                    }
                    else {
                        SaveCredentials();
                    }
                }

                try {
                    InitProxy();
                    jiraToken = jira.login(userName, password);
                    jira.Credentials = null;
                    loginWasSuccessful = true;
                }
                catch (Exception e) {
                    showLoginDialog = true;
                    MessageBox.Show(
                        "login failed for the following reason: " + "\r\n" + e.Message,
                        "Error occured",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally {
                    Utilities.ProtectUserSettings();
                }
            }
        }

        private void InitProxy() {
            if (Settings.Default.UseProxy) {
                jira.Proxy = new WebProxy(Settings.Default.ProxyURL);
                NetworkCredential proxyAuth = new NetworkCredential(
                    Settings.Default.ProxyUser,
                    Settings.Default.ProxyPassword);
                jira.Proxy.Credentials = proxyAuth;
            }
            else {
                jira.Proxy = WebRequest.GetSystemWebProxy();
            }
            
        }

        private void InitJiraToken() {
            jira = new JiraSoapServiceService { Url = Settings.Default.SoapURL };
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
        }

        private void SaveCredentials() {
            userName = frmLogin.Controls["groupBox1"].Controls["tbLogin"].Text;
            password = frmLogin.Controls["groupBox1"].Controls["tbPassword"].Text;
            Settings.Default.User = userName;
            Settings.Default.Password = password;

            if (Settings.Default.UseProxy) {
                proxyUserName = frmLogin.Controls["groupBox2"].Controls["tb_proxylogin"].Text;
                proxyPassword = frmLogin.Controls["groupBox2"].Controls["tb_proxypassword"].Text;
                Settings.Default.ProxyUser = proxyUserName;
                Settings.Default.ProxyPassword = proxyPassword;
            }

            Settings.Default.Save();
        }

        private void DisableProxyControls() {
            frmLogin.Controls["groupBox2"].Controls["tb_proxylogin"].Enabled = false;
            frmLogin.Controls["groupBox2"].Controls["tb_proxypassword"].Enabled = false;
            frmLogin.Controls["groupBox2"].Controls["lb_proxylogin"].Enabled = false;
            frmLogin.Controls["groupBox2"].Controls["lb_proxypassword"].Enabled = false;
        }

        private void PreSetJiraLoginData() {
            frmLogin.Controls["groupBox1"].Controls["tbLogin"].Text = userName;
            frmLogin.Controls["groupBox1"].Controls["tbPassword"].Text = password;
        }

        private void PreSetProxyLoginData() {
            frmLogin.Controls["groupBox2"].Controls["tb_proxylogin"].Text = proxyUserName;
            frmLogin.Controls["groupBox2"].Controls["tb_proxypassword"].Text = proxyPassword;
        }

        private static bool CertificateValidationCallBack(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors ) {
            return true;
        }

        private
            void Disconnect() {
            jira.logout(jiraToken);
        }

        private RemoteComponent[] GetComponents( string components ) {
            List<RemoteComponent> list = components.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(val => new RemoteComponent { id = val }).ToList();

            return list.Count == 0 ? null : list.ToArray();
        }

        private static string GetDefaultBrowserPath() {
            string key = @"http\shell\open\command";
            RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(key, false);
            return ((string)registryKey.GetValue(null, null)).Split('"')[1];
        }

        /// <summary>
        /// This method reads the compressedLogDetails as XML and returns all found "LogInfo"-Attributes of the "XmlObject"-Nodes in one string
        /// </summary>
        /// <param name="compressedLogDetails">String representation of the CompressedLog</param>
        /// <returns>LogInfo-Attributes joined to one string</returns>
        private static String GetLogInfoFromCompressedLogDetails( String compressedLogDetails ) {
            if (compressedLogDetails != null) {
                try {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(compressedLogDetails);
                    XmlNodeList xmlObjectNodes = doc.GetElementsByTagName(XmlObjectTagName);
                    StringBuilder logInfos = new StringBuilder();
                    foreach (XmlNode xmlObjectNode in xmlObjectNodes) {
                        XmlAttributeCollection xmlAttributeCollection = xmlObjectNode.Attributes;
                        if (xmlAttributeCollection == null) {
                            continue;
                        }
                        XmlNode logInfoAttribute = xmlAttributeCollection.GetNamedItem(LogInfoAttributeName);
                        if (logInfoAttribute != null) {
                            String currLogInfo = logInfoAttribute.Value;
                            if (!String.IsNullOrEmpty(currLogInfo)) {
                                logInfos.AppendLine(currLogInfo);
                            }
                        }
                    }
                    return logInfos.ToString();
                }
                catch {
                    // we can't read the compressedLog for some reason. We'll return an empty String. 
                }
            }
            return String.Empty;
        }
    }
}