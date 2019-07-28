using System;
using System.Collections.Generic;
using System.Text;

namespace Coherent.Docstore.Entity
{
    public class DocstoreApiResponse
    {
        public DocstoreApiResponse() {
            this.Status = ApiStatus.Success;
            this.Response = new DocstoreResponse();
        }

        public ApiStatus Status { get; set; }

        public string Message { get; set; }

        public DocstoreResponse Response { get; set; }
    }
}
