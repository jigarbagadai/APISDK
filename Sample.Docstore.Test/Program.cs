using Coherent.Docstore.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
namespace Coherent.Docstore.Test {
    class Program {
        static void Main(string[] args) {
            ILoggerFactory loggerFactory = new LoggerFactory();
            string userToken = "eyJraWQiOiJhWW0ya2xQOHFWMzFiSndkd1JSVDVVTkRCZjlUVEpnTE5hQUViVGo3QVFJPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiI0MGViY2VkYS1kN2QwLTRkNjgtOWE1MS1mOWMzN2E4ZmYyNzQiLCJhdWQiOiJxcG5kaGNyZDd1ZzRqZnFkaWRidmtuNTVnIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImV2ZW50X2lkIjoiZWUyMWRkZmEtZGE2Yy0xMWU4LWI3YWMtMmJkMTc3YTYwY2U1IiwidG9rZW5fdXNlIjoiaWQiLCJhdXRoX3RpbWUiOjE1NDA3MDIyMDIsImlzcyI6Imh0dHBzOlwvXC9jb2duaXRvLWlkcC51cy13ZXN0LTIuYW1hem9uYXdzLmNvbVwvdXMtd2VzdC0yX01Ocm5TRmxNYSIsImNvZ25pdG86dXNlcm5hbWUiOiJleGNlbGFkbWluIiwiZXhwIjoxNTQwNzA1ODAyLCJpYXQiOjE1NDA3MDIyMDIsImVtYWlsIjoidmlqYXlkYmFnYWRhaUBnbWFpbC5jb20ifQ.MFjDHWhPGFYuPTZkPp_EMASIvgB6h9CmTMvmqW-DN2L85WIaVbq3fquk0VAiXCp1VeyLVeh0RhpOc86FQgtstDSMMPTxPMfOT0QDa73rHsrqA86RBRJImJ7L9bOxdqawe3_d1OpdQpOOluNPj-FOy-Sg0birDXdduH8Bmvq_a9-YxXiEhUKOStKS6OuYFW1PQRIGeGNObBbi5sX4aNH92Bpi4iqh9vE2A1CbSz9MrePBUtPZqB5WIyuFfbRTP7nD-8uZbrLpixii_MQ51pZ8pWfhPX0HzWMZZWp3hNHmmh5tf5yv3OGyNoVHKQygbL6D1zq7uXcxcu2WRiWBmuNU9g";
            DocstoreClient docstoreClient = new DocstoreClient(userToken, "https://docs.dev.darkdonny.pw/docstore", loggerFactory);
            byte[] bytecontet = System.IO.File.ReadAllBytes("documents/0e7d10a8-b41b-41ff-8945-f91b027d0393.xlsx");

            byte[] gb = Guid.NewGuid().ToByteArray();
            int i = BitConverter.ToInt32(gb, 0);

            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("test", "test1");
            metadata.Add("test1", "test2");

            string documentName = string.Format("agreement{0}", Math.Abs(i)) + ".xlsx";
            DocstoreApiResponse response = docstoreClient.CreateDocument(documentName, "jigartest", bytecontet, metadata).Result;
            Console.WriteLine("Create Document: {0}", response.Response.Name);

            documentName = string.Format("agreement{0}", Math.Abs(i)) + ".xlsx";
            response = docstoreClient.UpsertDocument(documentName, "jigartest", bytecontet, metadata).Result;
            Console.WriteLine("Upsert Document: {0}", response.Response.Name);

            response = docstoreClient.GetDocument(documentName, "ExcelEngine/Services").Result;
            Console.WriteLine("Get Latest Version Document: {0}", response.Response.File);
            
            DocumentDownloadManager downloadManager = new DocumentDownloadManager();
           // byte[] content = downloadManager.DownloadFileContent(response.Response.File);

            //System.IO.File.WriteAllBytes("documents/test.xlsx", content);
//            response = docstoreClient.DeleteDocument(documentName, "jigartest").Result;
  //          Console.WriteLine("Document Deleted sucessfully");

            DocstoreListApiResponse result = docstoreClient.ListDocuments("ExcelEngine/Services").Result;
            Console.WriteLine("Total Count:" + result.Response.Count);

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}
