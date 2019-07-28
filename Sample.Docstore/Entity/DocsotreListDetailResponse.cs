using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coherent.Docstore {
    public class DocsotreListDetailResponse {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("results")]
        public List<DocstoreListResponse> Results { get; set; }
    }
}
