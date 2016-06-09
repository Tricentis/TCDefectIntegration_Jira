using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCDefectIntegration.RestProxy.Items;
using System.Reflection;

namespace RestProxy
{
    public class JiraRestManager
    {
        private string m_BaseUrl;
        private string m_Username;
        private string m_Password;
        private WebProxy m_Proxy;

        public JiraRestManager(string username, string password, string baseUrl, WebProxy proxy = null)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveNewtonsoftJson);

            m_BaseUrl = baseUrl;
            m_Username = username;
            m_Password = password;
            m_Proxy = proxy;
        }

        public bool checkCredentials()
        {
            string queryResult = RunQuery("/myself");

            JObject json = JObject.Parse(queryResult);
            return (json["key"].ToString() == m_Username);
        }

        public string getIssueStatus(string issueKey)
        {
            //retrieve creation data            
            string url = "issue";
            string argument = issueKey;

            try
            {
                string queryResult = RunQuery(url, argument);

                string status = JObject.Parse(queryResult).SelectToken("fields").SelectToken("status").ToString();
                IssueStatus issueStatus = JsonConvert.DeserializeObject<IssueStatus>(status);

                return issueStatus.Name;
            }
            catch { return "Couldn't retrieve status"; }
        }

        public bool doesIssueExist(string issueKey)
        {
            try
            {
                string queryResult = RunQuery("issue", issueKey);

                return (JObject.Parse(queryResult).SelectToken("fields").SelectToken("issuetype").ToString().Length > 0);
            }
            catch (Exception) { return false; }

        }

        internal string addAttachment(string defectId, string filePath, string fileNameDescription, string contentType)
        {
            string url = string.Format("{0}{1}", m_BaseUrl, "issue/" + defectId + "/attachments");

            // define boundaries
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            // request
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.Headers.Add("X-Atlassian-Token", "no-check");

            Stream rs = null;

            try
            {
                rs = request.GetRequestStream();

                rs.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, "file", fileNameDescription, contentType);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                FileStream fileStream = null;
                try
                {
                    fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        rs.Write(buffer, 0, bytesRead);
                    }
                }
                finally
                {
                    if (fileStream != null) fileStream.Close();
                }

                byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
            }
            finally
            {
                if (rs != null)
                {
                    rs.Close();
                }
            }

            string base64Credentials = GetEncodedCredentials();
            request.Headers.Add("Authorization", "Basic " + base64Credentials);

            //proxy
            if (m_Proxy != null)
            {
                request.Proxy = m_Proxy;
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            string result = string.Empty;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// Runs a query towards the JIRA REST api
        /// </summary>
        /// <param name="resource">The kind of resource to ask for</param>
        /// <param name="argument">Any argument that needs to be passed, such as a project key</param>
        /// <param name="data">More advanced data sent in POST requests</param>
        /// <param name="method">Either GET or POST</param>
        /// <returns></returns>
        protected string RunQuery(
            string resource,
            string argument = null,
            string data = null,
            string method = "GET")
        {
            string url = string.Format("{0}{1}/", m_BaseUrl, resource);

            if (argument != null)
            {
                url = string.Format("{0}{1}", url, argument);
            }

            // clean up url
            url = url.Replace("//", "/").Replace(":/", "://");

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = method;

            if (data != null)
            {
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(data);
                }
            }

            string base64Credentials = GetEncodedCredentials();
            request.Headers.Add("Authorization", "Basic " + base64Credentials);

            //proxy
            if (m_Proxy != null)
            {
                request.Proxy = m_Proxy;
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            string result = string.Empty;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        private string GetEncodedCredentials()
        {
            string mergedCredentials = string.Format("{0}:{1}", m_Username, m_Password);
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }

        public List<Fields> getRequiredIssues(string projectKey)
        {
            // Option 2 createmeta with parameters
            // api/2/issue/createmeta?projectKeys=QA&issuetypeNames=Bug&expand=projects.issuetypes.fields

            List<string> fields = new List<string> { "projectKeys", "issuetypeNames", "expand" };

            string issueType = "Bug";
            string argument = "createmeta?projectKeys=" + projectKey + "&issuetypeNames=" + issueType + "&expand=projects.issuetypes.fields";

            string queryResult = RunQuery("issue", argument);
            JObject json = JObject.Parse(queryResult);

            //    List<JToken> categories = json["projects"][0]["issuetypes"][0]["fields"].ToList();
            IssueTypesStructure response = JsonConvert.DeserializeObject<IssueTypesStructure>(json.SelectToken("projects")[0].SelectToken("issuetypes")[0].ToString());

            List<Fields> requiredFields = new List<Fields>();
            foreach (JToken key in response.Fields.additionalFields.Values)
            {
                Fields field = JsonConvert.DeserializeObject<Fields>(key.ToString());
                if (field.Required.Equals(true))
                {
                    requiredFields.Add(field);
                }
            }

            return requiredFields;
        }
        private static Assembly ResolveNewtonsoftJson(object sender, ResolveEventArgs args)
        {
            string tricentisHome = Environment.GetEnvironmentVariable("TRICENTIS_HOME");
            string zipDllPath = string.Empty;
            if (tricentisHome != null)
            {
                zipDllPath = Path.Combine(tricentisHome, @"libs\Newtonsoft.Json.dll");
            }
            else
            {
                throw new DirectoryNotFoundException("TRICENTIS_HOME not found.");
            }

            string[] quallifiedNameChunks = args.Name.Split(',');
            FileInfo fi = new FileInfo(zipDllPath);
            if (quallifiedNameChunks[0] == fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length))
            {
                return Assembly.LoadFrom(fi.FullName);
            }
            return null;
        }
    }


}
