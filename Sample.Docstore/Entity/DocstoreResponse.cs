using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coherent.Docstore {
    public class DocstoreResponse {
        [JsonProperty("create_version_action")]
        public string CreateVersionAction { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fully_qualified_name")]
        public string FullyQualifiedName { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("updated")]
        public string Updated { get; set; }

        [JsonProperty("parent")]
        public string Parent { get; set; }

        [JsonProperty("created_by")]
        public string CreatedBy { get; set; }

        [JsonProperty("last_modified_by")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("latest_version")]
        public string LatestVersion { get; set; }

        [JsonProperty("children")]
        public List<string> Childrens { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string,string> Metadata { get; set; }

        [JsonProperty("sha512")]
        public string Sha512 { get; set; }

        [JsonProperty("file")]
        public string File { get; set; }

        [JsonProperty("doc_path")]
        public string doc_path { get; set; }
    }
}
