using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace AreaUploader.Models
{
    public class AreaPath
    {
        public class AreaPaths
        {
            public List<Suite> Suites { get; set; }
        }
        public class Suite
        {
            public int id { get; set; }
            public string name { get; set; }

            public List<Family> Families { get; set; }
        }
        public class Family
        {
            public int id { get; set; }
            public string name { get; set; }

            public List<Products> Products { get; set; }
        }
        public class Products
        {
            public int id { get; set; }
            public string name { get; set; }

            public List<Areas> Areas { get; set; }
        }
        public class Areas
        {
            public int id { get; set; }
            public string name { get; set; }

            public List<SubArea> Subareas { get; set; }
        }
        public class SubArea
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public interface IConfiguration
        {
            string PersonalAccessToken { get; set; }
            string Project { get; set; }
            string Team { get; set; }
            string MoveToProject { get; set; }
            string UriString { get; set; }
            string Query { get; set; }
            string Identity { get; set; }
            string WorkItemIds { get; set; }
            string WorkItemId { get; set; }
            string ProcessId { get; set; }
            string PickListId { get; set; }
            string QueryId { get; set; }
            string FilePath { get; set; }
            string GitRepositoryId { get; set; }
            string VersionNumber { get; set; }
        }
        public class Configuration : IConfiguration
        {
            public string UriString { get; set; }
            public string PersonalAccessToken { get; set; }
            public string Project { get; set; }
            public string Team { get; set; }
            public string MoveToProject { get; set; }
            public string Query { get; set; }
            public string Identity { get; set; }
            public string WorkItemIds { get; set; }
            public string WorkItemId { get; set; }
            public string ProcessId { get; set; }
            public string PickListId { get; set; }
            public string QueryId { get; set; }
            public string FilePath { get; set; }
            public string GitRepositoryId { get; set; }
            public string VersionNumber { get; set; }
        }

        public enum TemplateType
        {
            Agile,
            Scrum,
            CMMI
        }

        public class CreateUpdateNodeViewModel
        {
            public class Node
            {
                public int id { get; set; }
                public string name { get; set; }

            }

        }
        public class GetNodeResponse
        {
            public class Node
            {
                public string name { get; set; }
                public int id { get; set; }
                public string Message { get; set; }
                public object innerException { get; set; }
                public string message { get; set; }
                public string typeName { get; set; }
                public string typeKey { get; set; }
                public int errorCode { get; set; }
                public int eventId { get; set; }
                public HttpStatusCode HttpStatusCode { get; internal set; }
            }
        }
    }
}