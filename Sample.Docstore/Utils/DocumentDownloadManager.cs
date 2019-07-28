using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Coherent.Docstore {
    public class DocumentDownloadManager {
        public byte[] DownloadFileContent(string fileUrl) {
            byte[] contentArray = null;
            using(var wc = new System.Net.WebClient()) {
                contentArray = wc.DownloadDataTaskAsync(new Uri(fileUrl)).Result;
            }
            return contentArray;
        }

    }
}
