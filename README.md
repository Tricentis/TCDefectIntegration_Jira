Tosca Commander DefectIntegration for Jira (using Soap & Restful)
===========================

The Defect Integration for Jira is an additional component for the Tosca Testsuite, which enables you to integrate Jira into Tosca using Jira's Soap or Rest interface. It allows Jira Tickets to be opened with Tosca Commander, new tickets can be created and existing tickets can be synchronized.

----------
Version History
-------------
Added support for Restful interface and attachments. Restful interface is required for Jira 7 and later.
New configuration parameters UseRestfulConnection,UseManualDefectCreationWindow,RestfulURL,JiraCreateIssueURL


----------
Architecture
-------------
This integration uses Tosca's built-in SimpleDefectTracking interface, which provides the tasks for **create/open/update** defects on ExecutionEntries, and creates an xml file within the workspace directory including additional information for the integration. 
The integration uses the following interfaces from jira: https://developer.atlassian.com/jiradev/support/archive/jira-rpc-services/creating-a-jira-soap-client (soap)
														 https://developer.atlassian.com/jiradev/jira-apis/jira-rest-apis (rest)


```sequence
Tosca->SimpleDefectIntegrationAddin: create/open/update JiraIssue
SimpleDefectIntegrationAddin->TCDefectIntegration_Jira:call TCDefectIntegration.exe with parameters for create/open/update JiraIssue
TCDefectIntegration_Jira-->SimpleDefectIntegrationAddin: uses TCDefectIntegration.xml
TCDefectIntegration_Jira-->Jira:uses Jira's Soap interface
TCDefectIntegration_Jira->Jira:call create/open/update JiraIssue on Jira
Jira->Jira:create/open/update JiraIssue
Jira->TCDefectIntegration_Jira:return Id/update result
TCDefectIntegration_Jira->SimpleDefectIntegrationAddin:return Id/update result
SimpleDefectIntegrationAddin->Tosca:return Id/update result
```
----------
Installation
-------------

 - Jira's Webservice must  be enabled and accessible, and the ToscaSimple Defect Tracking AddIn must be installed.
 - The TCDefectIntegration.exe and TCDefectIntegration.exe.config have to be deployed to **%TRICENTIS_HOME%\ToscaCommander**.
 - There is a 3rd party dependency, Newtonsoft.Json.dll must be in **%TRICENTIS_HOME%\libs**, which is the case with the default Tosca installation (version 9.2).

----------

Configuration
-------------

**Mandatory properties**

The following settings must be specified in the TCDefectIntegration.exe.config file in the **&lt;userSettings&gt;**;and **&lt;applicationSettings&gt;** sections prior to using Tosca Jira Direct Integration:

Setting     | Description
--------    | --------------------------------------------------------------
Url         | A Url to open a Defect is specified here.<br>Example:<br>http://www.mycompany.com/jira/browse
SoapURL     | A Url to access the Webservice is specified here.<br>Example:<br>http://www.mycompany.com/jira/rpc/soap/jirasoapservice-v2?wsdl
User        |The user name to be used to sign up to Jira is specified here.
Password    |Jira password
UseManualDefectCreationWindow | Manual browser window is launched upon issue creation. Standard behaviour when using UseRestfulConnection.
UseRestfulConnection          | With Jira greater than 7 the SOAP is not supported anymore and RestFulConnection must be used
RestfulURL                    | Url to the restful service version https://mycompany.atlassian.net/rest/api/2/
JiraCreateIssueURL            | Url that is loaded when the manual browser window is launched, default is https://mycompany.atlassian.net/secure/CreateIssue!default.jspa
<br>
**Using a Proxy**

When using Jira via a proxy server, the following settings must be specified in the TCDefectIntegration.exe.config file in the **&lt;userSettings&gt;** and **&lt;applicationSettings&gt;** 
sections:            

Setting     | Description
--------    | --------------------------------------------------------------
UserProxy         | If the value of this setting is True, proxy settings are used.
ProxyURL|Proxy server URL
ProxyUser|Valid proxy user
ProxyPassword|Valid proxy password
<br>
**Encryption of proxy credentials**

Proxy access data can be saved encrypted. If the settings User and Password are not defined, a log in dialog box is opened when using the option Create new ticket.
After the access data has been entered it is saved encrypted in Windows user.config. The file can be found at **%USERPROFILE%\AppData\Local\TRICENTIS_Technology_&_Co\TCDefectIntegration.exe_Url_tfddctec4a1cgkl1dlottk3ndiimiu\.**
The saved access data is used for future log-ins and the log in dialog box is no longer displayed.

Properties to be saved with a ticket must be defined either via the file TCDefectIntegration.exe.config in the section **&lt;applicationSettings&gt;** or in Tosca Commanderâ„¢ via properties which can be freely defined via ExecutionLists in the Execution section (see Tosca Commander Manual - chapter "Specifying properties"). Freely definable properties overwrite properties of the same name in the TCDefectIntegration.exe.config file. The following properties can be defined:

Property     | Description
--------    | --------------------------------------------------------------
DefectType         | Type of ticket, e.g. Bug
DefectAssignee|User to whom the ticket is assigned
DefectPriority|Ticket priority, e.g. Minor
DefectStatus|Ticket status, e.g. New
DefectComponents| List of components. Individual elements are entered separated by commas.
DefectProject|Project to which the ticket should be assigned
<br>
**CustomDefectProperties**

Customer-specific fields which have been added in Jira can be linked to or filled
with values. Individual properties are entered separated by semicolons.
> **Syntax:**
> &lt;Property name>:&lt;ID&gt;=&lt;Value&gt;
> The property name can be optionally entered:
> &lt;ID&gt;=&lt;Value&gt;


**Manual creation**
Depending on the complexity of the custom properties and fields configuration during Issue creation this integration can be configured to create the issue within a browser window.
This mode is also used when the restful api is chosen.

**Debugging**
For easier debugging 3 testfiles have been attached to the project to simulate, create/open/update functinoality.
----------
