using mshtml;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using TCDefectIntegration.Properties;

namespace TCDefectIntegration.BrowserWindow
{

    public partial class BrowserForm : Form
    {

        private WebBrowser webBrowser;
        private bool issueCreated = false;
        private TransparentForm formOverlay = null;
        private bool showOverlay = false;
        private Point location = new Point();
        private bool automateLogin = false;
        private bool doLoginDone = false;
        private bool doRedirectDone = false;
        string username = "";
        string password = "";

        public BrowserForm(string url, string Username, string Password, bool AutomateLogin = false)
        {
            InitializeComponent();
            InitBrowser(url);
            automateLogin = AutomateLogin;
            username = Username;
            password = Password;
        }

        public string issueId { get; set; }

        public void InitBrowser(string url)
        {
            webBrowser = new WebBrowser();
            this.panel2.Controls.Add(webBrowser);
            webBrowser.Navigate(url);
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.BringToFront();

            showAutomationInProgress();
            // save location to avoid redrawing all the time
            location = this.Location;

            // event handlers
            webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(pageCompletedLoading);
            webBrowser.DocumentTitleChanged += WebBrowser_DocumentTitleChanged;
            webBrowser.Navigating += new WebBrowserNavigatingEventHandler(webBrowser_Navigating);
            this.Move += Browser_Move;
        }


        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (automateLogin) showAutomationInProgress();
        }

        private void Browser_Move(object sender, EventArgs e)
        {
            if (formOverlay != null) formOverlay.Dispose();
        }

        private void showOverlayArea()
        {
            //if (showOverlay && this.Location != location)
            if (showOverlay)
            {
                // save old location
                location = this.Location;

                // dispose old location
                if (formOverlay != null) formOverlay.Dispose();

                // paint new area
                formOverlay = new TransparentForm();
                //formOverlay.Size = this.panel2.Size;
                //formOverlay.Location = new Point(this.Left + this.DefaultMargin.Left, this.Top+this.panel2.Bounds.Y + this.DefaultMargin.Top);   
                int titleBarHeight = SystemInformation.ToolWindowCaptionHeight;
                formOverlay.Top = this.panel2.Top + this.Location.Y + this.DefaultMargin.Top * 2 + titleBarHeight + SystemInformation.BorderSize.Width * 2;
                formOverlay.Left = this.panel2.Left + this.Location.X + this.DefaultMargin.Left * 2 + SystemInformation.BorderSize.Width;
                formOverlay.Width = this.panel2.Width;
                formOverlay.Height = this.panel2.Height;
                formOverlay.ShowWithoutActivate();
                formOverlay.BringToFront();
            }
            else
            {
                if (formOverlay != null) formOverlay.Dispose();
            }
        }


        private void Document_MouseMove(object sender, HtmlElementEventArgs e)
        {
            //nothing for now, could handle the overlay form here as well.
        }

        private void changeStyleAfterIssueCapture()
        {
            if (issueCreated)
            {
                Thread.Sleep(3000);
                this.labelRight.Text = "Captured Jira Issue, good to return";
                this.closeButton.Text = "Close";
                this.closeButton.Visible = true;
                //showOverlay = true;
                //showOverlayArea();
                this.successButton.Visible = true;
                this.successLabel.Visible = true;
                this.successPanel.Visible = true;
                this.successPanel.BringToFront();
                webBrowser.Visible = false;
            }
        }

        #region captureIssue
        // depending on the jira version one or the other works, when directly calling the create page pageCompletedLoading seems to work
        // when calling the login page first WebBrowser_DocumentTitleChanged seems to work

        private void WebBrowser_DocumentTitleChanged(object sender, EventArgs e)
        {
            // try to automatically grab the submitted issue, tested with Jira 7.            

            if (((WebBrowser)sender).Document.Body != null)
            {
                string body = ((WebBrowser)sender).Document.Body.InnerText;

                Regex regex = new Regex(@"Issue .* - .* has been successfully created.");
                Match match = regex.Match(body);
                if (match.Success)
                {
                    string issue = match.Groups[0].Value;

                    regex = new Regex(@" [a-z|A-Z]+-[0-9]+ ");
                    match = regex.Match(issue);

                    if (match.Success)
                    {
                        issue = match.Groups[0].Value;
                        issue = issue.Trim();
                        issueId = issue;
                        issueCreated = true;
                        changeStyleAfterIssueCapture();

                    }
                }
            }
        }


        private void pageCompletedLoading(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            string url = ((WebBrowser)sender).Url.ToString();

            if (automateLogin && url.IndexOf("/login") != -1 && !doLoginDone)
            {
                doLoginDone = true;
                showAutomationInProgress();
                doLogin((WebBrowser)sender);
                resetAutomationInProgress();
                return;
            }

            // Sometimes an error is displayed and the login page is not loaded directly
            if (automateLogin && url.Equals(Settings.Default.JiraCreateIssueURL) && !doRedirectDone)
            {
                doRedirectDone = true;
                showAutomationInProgress();
                doRedirect((WebBrowser)sender);
                resetAutomationInProgress();
            }

            // Disable overlay on navigation once Create Issue page is reached
            HtmlDocument doc = ((WebBrowser)sender).Document;
            if (doc != null)
            {
                HtmlElementCollection titles = doc.GetElementsByTagName("TITLE");
                if(titles.Count > 0)
                {
                    foreach(HtmlElement el in titles)
                    {
                        string text = el.InnerText;
                        if (!string.IsNullOrEmpty(text) && text.ToLower().StartsWith("create issue"))
                        {
                            automateLogin = false;
                        }
                    }

                }
            }

            Regex regex = new Regex(@"browse/[a-z|A-Z]+-[0-9]+");
            Match match = regex.Match(url);
            if (match.Success)
            {
                url = match.Groups[0].Value;

                regex = new Regex(@"[a-z|A-Z]+-[0-9]+");
                match = regex.Match(url);

                if (match.Success)
                {
                    url = match.Groups[0].Value;
                    url = url.Trim();
                    issueId = url;
                    issueCreated = true;
                    changeStyleAfterIssueCapture();

                }
            }

            // assigned here as document is null when initializated
            webBrowser.Document.MouseMove += Document_MouseMove;
        }

        private void showAutomationInProgress()
        {
            showOverlay = true;
            showOverlayArea();
        }

        private void resetAutomationInProgress()
        {
            showOverlay = false;
            showOverlayArea();
        }

        private void doRedirect(WebBrowser browser)
        {
            try
            {
                HtmlDocument doc = browser.Document;
                if (doc != null)
                {
                    string bodyText = doc.Body.InnerText;
                    HtmlElementCollection links = doc.GetElementsByTagName("A");
                    if (bodyText.IndexOf("You are not logged in") != -1)
                    {
                        foreach (HtmlElement link in links)
                        {
                            string innerText = link.InnerText;
                            if (innerText != null && innerText.ToLower().Equals("log in"))
                            {
                                link.InvokeMember("click");
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        private void doLogin(WebBrowser browser)
        {
            try
            {
                HtmlDocument doc = browser.Document;
                if (doc != null)
                {

                    HtmlElement usernameElement = null;
                    usernameElement = doc.GetElementById("username");
                    if (usernameElement == null)
                    {
                        usernameElement = doc.GetElementById("login-form-username");
                    }
                    if(usernameElement != null)
                    {
                        usernameElement.SetAttribute("value", username);
                    }
                    else
                    {
                        return;
                    }


                    HtmlElement passwordElement = null;
                    passwordElement = doc.GetElementById("password");
                    if(passwordElement == null)
                    {
                        passwordElement = doc.GetElementById("login-form-password");
                    }
                    if(passwordElement != null){
                        passwordElement.SetAttribute("value", password);
                    }
                    else
                    {
                        return;
                    }                    

                    // click login button
                    doc.GetElementById("login").InvokeMember("click");
                }
            }
            catch (Exception) { }

        }

        #endregion

        private bool SetTextControlValue(HtmlDocument doc, String elementName, String value)
        {
            try
            {
                doc.GetElementById(elementName).SetAttribute("value", value);
            }
            catch { return false; }

            return true;
        }



        private void SetErrorMessage(String text)
        {
            // Show error message if required
        }


        private void closeButton_Click(object sender, EventArgs e)
        {
            if (formOverlay != null) formOverlay.Dispose();
            this.Close();
        }

        private void successButton_Click(object sender, EventArgs e)
        {
            if (formOverlay != null) formOverlay.Dispose();
            this.Close();
        }
    }
}
