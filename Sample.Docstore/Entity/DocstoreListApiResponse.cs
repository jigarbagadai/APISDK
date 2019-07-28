using System;
using System.Collections.Generic;
using System.Text;

namespace Coherent.Docstore.Entity {
    public class DocstoreListApiResponse {
        public DocstoreListApiResponse() {
            this.Status = ApiStatus.Success;
            this.Response = new DocsotreListDetailResponse();
        }

        public ApiStatus Status { get; set; }

        public string Message { get; set; }

        public DocsotreListDetailResponse Response { get; set; }
    }
}
