
TCDefectIntegration_Jira (Soap)
===========================
-----------
**Table of Content**

[TOC]

-----------
The TCDefectIntegration_Jira (Soap) is an additional component for the Tosca Testsuite, which enables you to integrate Jira into Tosca. It allows Jira Tickets to be opened with Tosca Commander, new tickets can be created and existing tickets can be synchronized.

----------

<i class="icon-file">Architecture
-------------
The TCDefectIntegration_Jira uses Tosca's built-in SimpleDefectTracking interface, which provides the tasks for **create/open/update** defects on ExecutionEntries, and creates an xml file within the workspace directory including additional information for the Integration.  The integration uses the following interface: https://developer.atlassian.com/jiradev/support/archive/jira-rpc-services/creating-a-jira-soap-client


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
<i class="icon-folder-open"></i>Installation
-------------

 - Jira's Webservice must  be enabled and accessible, and the ToscaSimple Defect Tracking AddIn must be installed.
 - The TCDefectIntegration.exe and TCDefectIntegration.exe.config have to be deployed to **%TRICENTIS_HOME%\ToscaCommander**.
 

----------

<i class="icon-cog"></i>Configuration
-------------

**Mandatory properties**

The following settings must be specified in the TCDefectIntegration.exe.config file in the **&lt;userSettings&gt;**;and **&lt;applicationSettings&gt;** sections prior to using Tosca Jira Direct Integration:

Setting     | Description
--------    | --------------------------------------------------------------
Url         | A Url to open a Defect is specified here.<br>Example:<br>http://www.mycompany.com/jira/browse
SoapURL      | A Url to access the Webservice is specified here.<br>Example:<br>http://www.mycompany.com/jira/rpc/soap/jirasoapservice-v2?wsdl
User|The user name to be used to sign up to Jira is specified here.
Password|Jira password
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

----------