using Coherent.Docstore.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coherent.Docstore.Client {
    public interface IDocStoreClient {
        Task<DocstoreApiResponse> CreateDocument(string documentName, string path, byte[] filecontent, Dictionary<string, string> metadata);

        Task<DocstoreApiResponse> UpsertDocument(string documentName, string path, byte[] filecontent, Dictionary<string, string> metadata);

        Task<DocstoreApiResponse> GetDocument(string documentName, string path);

        Task<DocstoreApiResponse> DeleteDocument(string documentName, string path);

        Task<DocstoreListApiResponse> ListDocuments(string path);

        Task<byte[]> GetFileContent(string documentName, string path);

        Task<DocstoreApiResponse> CreateTempDocumeent(string documentName, string path, byte[] filecontent, Dictionary<string, string> metadata, int timetoLive = 5);

        Task<byte[]> GetFileContent(string documentUrl);
    }
}
