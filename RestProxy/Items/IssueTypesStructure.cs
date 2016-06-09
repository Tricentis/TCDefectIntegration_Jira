using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCDefectIntegration.RestProxy.Items
{

    /// <summary>
    /// Representing a class with all required fields when calling 
    /// https://tricentisapac.atlassian.net/rest/api/2/issue/createmeta?projectKeys=TOSCA&issuetypeNames=Bug&expand=projects.issuetypes.fields
    /// </summary>
    /// 
    //  {
    //"expand": "projects",
    //"projects": [
    //  {
    //    "expand": "issuetypes",
    //    "self": "https:\/\/tricentisapac.atlassian.net\/rest\/api\/2\/project\/10000",
    //    "id": "10000",
    //    "key": "TOSCA",
    //    "name": "alain  [Administrator]'s 1st Project",
    //    "avatarUrls": {
    //      "48x48": "https:\/\/tricentisapac.atlassian.net\/secure\/projectavatar?avatarId=10324",
    //      "24x24": "https:\/\/tricentisapac.atlassian.net\/secure\/projectavatar?size=small&avatarId=10324",
    //      "16x16": "https:\/\/tricentisapac.atlassian.net\/secure\/projectavatar?size=xsmall&avatarId=10324",
    //      "32x32": "https:\/\/tricentisapac.atlassian.net\/secure\/projectavatar?size=medium&avatarId=10324"
    //    },
    //    "issuetypes": [
    //      {
    //        "self": "https:\/\/tricentisapac.atlassian.net\/rest\/api\/2\/issuetype\/10004",
    //        "id": "10004",
    //        "description": "A problem which impairs or prevents the functions of the product.",
    //        "iconUrl": "https:\/\/tricentisapac.atlassian.net\/secure\/viewavatar?size=xsmall&avatarId=10303&avatarType=issuetype",
    //        "name": "Bug",
    //        "subtask": false,
    //        "expand": "fields",
    //        "fields": {
    //          "summary": {
    //            "required": true,
    //            "schema": {
    //              "type": "string",
    //              "system": "summary"
    //            },
    //            "name": "Summary",
    //            "hasDefaultValue": false,
    //            "operations": [
    //              "set"
    //            ]
    //          },
    //          "issuetype": {
    //            "required": true,
    //            "schema": {
    //              "type": "issuetype",
    //              "system": "issuetype"
    //            },
    //            "name": "Issue Type",
    //            "hasDefaultValue": false,
    //            "operations": [

    public class IssueTypesStructure
    {
        [JsonProperty("self")]
        public string Self { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("fields")]
        public Fields Fields { get; set; }

    }

    public class Fields
    {
        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("hasDefaultValue")]
        public bool HasDefaultValue { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("allowedValues")]
        public List<AllowedValues> AllowedValues { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> additionalFields;

    }

    public class AllowedValues
    {
        [JsonProperty("id")]
        public int Id{ get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> additionalFields;
    }
}
