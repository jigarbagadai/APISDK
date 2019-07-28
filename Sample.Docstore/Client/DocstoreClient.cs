using Coherent.Docstore.Client;
using Coherent.Docstore.Entity;
using Coherent.Logging.SDK;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Coherent.Docstore {
    public class DocstoreClient : BaseDocstoreClient, IDocStoreClient {
        private string docstoreServiceendPoint = null;
        private IMemoryCache memoryCache = null;
        private ILogger<DocstoreClient> logger = null;
        public DocstoreClient(string token, string servicebaseUrl, ILoggerFactory loggerFactory) : base(token, servicebaseUrl, loggerFactory) {
            this.docstoreServiceendPoint = string.Format("{0}", servicebaseUrl);
            this.logger = loggerFactory.CreateLogger<DocstoreClient>();
        }


        public DocstoreClient(string token, string servicebaseUrl, ILoggerFactory loggerFactory, IMemoryCache memoryCache) : base(token, servicebaseUrl, loggerFactory) {
            this.docstoreServiceendPoint = string.Format("{0}", servicebaseUrl);
            this.memoryCache = memoryCache;
            this.logger = loggerFactory.CreateLogger<DocstoreClient>();
        }

        public async Task<DocstoreApiResponse> CreateDocument(string documentName, string path, byte[] filecontent, Dictionary<string, string> metadata) {
            DocstoreApiResponse apiResponse = new DocstoreApiResponse();

            try {
                this.restClient.requestUrl = string.Format("{0}/{1}/", this.docstoreServiceendPoint, "documents-create");
                //this.logger.LogTrace(this.GetLogRecord("Request->Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.CreateDocument", path));
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(documentName, Encoding.UTF8), "name");
                content.Add(new StringContent(path, Encoding.UTF8), "path");
                content.Add(new ByteArrayContent(filecontent), "file", documentName);
                content.Add(new StringContent(JsonConvert.SerializeObject(metadata), Encoding.UTF8, "application/json"), "metadata");

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                HttpResponseMessage responseMessage = await this.restClient.Post(content);
                if(responseMessage.IsSuccessStatusCode) {
                    apiResponse.Status = ApiStatus.Success;
                    var responseString = await responseMessage.Content.ReadAsStringAsync();
                    apiResponse.Response = GetDocstoreResponse(responseString);
                    stopwatch.Stop();
                    this.logger.LogTrace(this.GetLogRecord("Request Success -> Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.CreateDocument", path, stopwatch.ElapsedMilliseconds));
                }
                else {
                    apiResponse.Status = ApiStatus.Error;
                    apiResponse.Message = await responseMessage.Content.ReadAsStringAsync();
                    stopwatch.Stop();
                    this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl + " documentName:" + documentName + " ErrorMessage:" + apiResponse.Message, "DocstoreClient.CreateDocument", path, stopwatch.ElapsedMilliseconds));

                }
            } catch(Exception ex) {
                apiResponse.Status = ApiStatus.Error;
                apiResponse.Message = ex.Message;
                this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.CreateDocument", path), ex);
            }

            return apiResponse;
        }

        public async Task<DocstoreApiResponse> UpsertDocument(string documentName, string path, byte[] filecontent, Dictionary<string, string> metadata) {
            DocstoreApiResponse apiResponse = new DocstoreApiResponse();

            try {
                this.restClient.requestUrl = string.Format("{0}/{1}/", this.docstoreServiceendPoint, "documents-upsert");
                //this.logger.LogTrace(this.GetLogRecord("Request->Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.UpsertDocument", path));

                var content = new MultipartFormDataContent();
                content.Add(new StringContent(documentName, Encoding.UTF8), "name");
                content.Add(new StringContent(path, Encoding.UTF8), "path");
                content.Add(new ByteArrayContent(filecontent), "file", documentName);
                content.Add(new StringContent(JsonConvert.SerializeObject(metadata), Encoding.UTF8, "application/json"), "metadata");

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                HttpResponseMessage responseMessage = await this.restClient.Post(content);
                if(responseMessage.IsSuccessStatusCode) {
                    apiResponse.Status = ApiStatus.Success;
                    var responseString = await responseMessage.Content.ReadAsStringAsync();
                    apiResponse.Response = GetDocstoreResponse(responseString);

                    stopwatch.Stop();
                    this.logger.LogTrace(this.GetLogRecord("Request Success -> Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.UpsertDocument", path, stopwatch.ElapsedMilliseconds));
                }
                else {
                    apiResponse.Status = ApiStatus.Error;
                    apiResponse.Message = await responseMessage.Content.ReadAsStringAsync();

                    stopwatch.Stop();
                    this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl + " documentName:" + documentName + " ErrorMessage:" + apiResponse.Message, "DocstoreClient.UpsertDocument", path, stopwatch.ElapsedMilliseconds));

                }
            } catch(Exception ex) {
                apiResponse.Status = ApiStatus.Error;
                apiResponse.Message = ex.Message;
                this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.UpsertDocument", path), ex);
            }

            return apiResponse;
        }

        public async Task<DocstoreApiResponse> GetDocument(string documentName, string path) {
            DocstoreApiResponse apiResponse = new DocstoreApiResponse();

            try {
                this.restClient.requestUrl = string.Format("{0}/{1}/?name={2}&path={3}", this.docstoreServiceendPoint, "documents-get-latest", documentName, path);
                //this.logger.LogTrace(this.GetLogRecord("Request->Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.GetDocument", path));

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                HttpResponseMessage responseMessage = await this.restClient.Get();
                if(responseMessage.IsSuccessStatusCode) {
                    apiResponse.Status = ApiStatus.Success;
                    var responseString = await responseMessage.Content.ReadAsStringAsync();
                    apiResponse.Response = GetDocstoreResponse(responseString);
                    stopwatch.Stop();
                    this.logger.LogTrace(this.GetLogRecord("Request Success -> Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.GetDocument", path, stopwatch.ElapsedMilliseconds));
                }
                else {
                    apiResponse.Status = ApiStatus.Error;
                    apiResponse.Message = await responseMessage.Content.ReadAsStringAsync();

                    stopwatch.Stop();
                    this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl + " documentName:" + documentName + " ErrorMessage:" + apiResponse.Message, "DocstoreClient.GetDocument", path, stopwatch.ElapsedMilliseconds));
                }
            } catch(Exception ex) {
                apiResponse.Status = ApiStatus.Error;
                apiResponse.Message = ex.Message;

                this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.GetDocument", path), ex);
            }

            return apiResponse;
        }

        public async Task<DocstoreApiResponse> DeleteDocument(string documentName, string path) {
            DocstoreApiResponse apiResponse = new DocstoreApiResponse();

            try {
                this.restClient.requestUrl = string.Format("{0}/{1}/", this.docstoreServiceendPoint, "documents-delete-by-path");
                //this.logger.LogTrace(this.GetLogRecord("Request->Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.DeleteDocument", path));
                var formContent = new FormUrlEncodedContent(new[]
                     {
                    new KeyValuePair<string, string>("name", documentName),
                    new KeyValuePair<string, string>("path", path)
                 });

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                HttpResponseMessage responseMessage = await this.restClient.Post(formContent);
                if(responseMessage.IsSuccessStatusCode) {
                    apiResponse.Status = ApiStatus.Success;
                    var responseString = await responseMessage.Content.ReadAsStringAsync();
                    apiResponse.Response = GetDocstoreResponse(responseString);

                    stopwatch.Stop();
                    this.logger.LogTrace(this.GetLogRecord("Request Success -> Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.DeleteDocument", path, stopwatch.ElapsedMilliseconds));
                }
                else {
                    apiResponse.Status = ApiStatus.Error;
                    apiResponse.Message = await responseMessage.Content.ReadAsStringAsync();

                    stopwatch.Stop();
                    this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl + " documentName:" + documentName + " ErrorMessage:" + apiResponse.Message, "DocstoreClient.DeleteDocument", path, stopwatch.ElapsedMilliseconds));
                }
            } catch(Exception ex) {
                apiResponse.Status = ApiStatus.Error;
                apiResponse.Message = ex.Message;

                this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.DeleteDocument", path), ex);
            }

            return apiResponse;
        }

        public async Task<DocstoreListApiResponse> ListDocuments(string path) {
            DocstoreListApiResponse response = new DocstoreListApiResponse();
            try {
                DocsotreListDetailResponse docstoreListResponse = new DocsotreListDetailResponse();
                docstoreListResponse.Results = new List<DocstoreListResponse>();

                this.restClient.requestUrl = string.Format("{0}/{1}?path={2}", this.docstoreServiceendPoint, "folder/docs", path);
                //this.logger.LogTrace(this.GetLogRecord("Request->Url:" + this.restClient.requestUrl, "DocstoreClient.ListDocuments", path));

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                HttpResponseMessage responseMessage = await this.restClient.Get();
                if(responseMessage.IsSuccessStatusCode) {
                    response.Status = ApiStatus.Success;

                    DocsotreListDetailResponse resultdocstoreListResponse = await responseMessage.Content.ReadAsAsync<DocsotreListDetailResponse>();
                    docstoreListResponse.Results.AddRange(resultdocstoreListResponse.Results);

                    while(!string.IsNullOrEmpty(resultdocstoreListResponse.Next)) {
                        this.restClient.requestUrl = resultdocstoreListResponse.Next;
                        responseMessage = await this.restClient.Get();
                        resultdocstoreListResponse = await responseMessage.Content.ReadAsAsync<DocsotreListDetailResponse>();
                        docstoreListResponse.Results.AddRange(resultdocstoreListResponse.Results);
                    }

                    docstoreListResponse.Count = docstoreListResponse.Results.Count;
                    response.Response = docstoreListResponse;

                    stopwatch.Stop();
                    this.logger.LogTrace(this.GetLogRecord("Request Success -> Url:" + this.restClient.requestUrl, "DocstoreClient.ListDocuments", path, stopwatch.ElapsedMilliseconds));
                }
                else {
                    response.Status = ApiStatus.Error;
                    response.Message = await responseMessage.Content.ReadAsStringAsync();

                    stopwatch.Stop();
                    this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl + " ErrorMessage:" + response.Message, "DocstoreClient.ListDocuments", path, stopwatch.ElapsedMilliseconds));
                }
            } catch(Exception ex) {
                response.Status = ApiStatus.Error;
                response.Message = ex.Message;

                this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl, "DocstoreClient.ListDocuments", path), ex);
            }

            return response;
        }

        public async Task<byte[]> GetFileContent(string documentName, string path) {
            byte[] content = null;
            if(this.memoryCache == null) {
                this.restClient.requestUrl = string.Format("{0}/{1}/?name={2}&path={3}", this.docstoreServiceendPoint, "documents-get-latest", documentName, path);
                //this.logger.LogTrace(this.GetLogRecord("CACHE NOT ENABLED. Request->Url:" + this.restClient.requestUrl + " Path=" + path, "DocstoreClient.GetFileContent", path));

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                HttpResponseMessage responseMessage = await this.restClient.Get();
                if(responseMessage.IsSuccessStatusCode) {
                    var responseString = await responseMessage.Content.ReadAsStringAsync();
                    DocstoreResponse docstoreResponse = GetDocstoreResponse(responseString);
                    string downloadfilePath = docstoreResponse.File;
                    content = new DocumentDownloadManager().DownloadFileContent(downloadfilePath);
                    stopwatch.Stop();
                    this.logger.LogTrace(this.GetLogRecord("Request Success -> Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.GetFileContent", path, stopwatch.ElapsedMilliseconds));
                }
                else {
                    string error = await responseMessage.Content.ReadAsStringAsync();
                    stopwatch.Stop();
                    this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl + " ErrorMessage:" + error, "DocstoreClient.GetFileContent", path, stopwatch.ElapsedMilliseconds));
                    if(responseMessage.StatusCode == System.Net.HttpStatusCode.Forbidden) {
                        throw new ApplicationException("Token Expired. Please enter valid token.");
                    }
                }
            }
            else {
                this.restClient.requestUrl = string.Format("{0}/{1}/?name={2}&path={3}", this.docstoreServiceendPoint, "documents-get-latest", documentName, path);
                //this.logger.LogTrace(this.GetLogRecord("CACHE ENABLED. Request->Url:" + this.restClient.requestUrl + " Path=" + path, "DocstoreClient.GetFileContent", path));
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                HttpResponseMessage responseMessage = await this.restClient.Get();
                if(responseMessage.IsSuccessStatusCode) {
                    DocstoreResponse olddocstoreApiResponse = this.memoryCache.Get<DocstoreResponse>("OLD_" + documentName);
                    var responseString = await responseMessage.Content.ReadAsStringAsync();
                    DocstoreResponse docstoreResponse = GetDocstoreResponse(responseString);
                    this.memoryCache.Set<DocstoreResponse>("OLD_" + documentName, docstoreResponse);
                    stopwatch.Stop();
                    this.logger.LogTrace(this.GetLogRecord("Request Success -> Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.GetFileContent", path, stopwatch.ElapsedMilliseconds));

                    if(olddocstoreApiResponse != null) {
                        DateTime olddocstoreDateTime = Convert.ToDateTime(olddocstoreApiResponse.Updated);
                        DateTime newDocstoreDateTime = Convert.ToDateTime(docstoreResponse.Updated);

                        if(olddocstoreDateTime == newDocstoreDateTime) {
                            byte[] cacheContent = this.memoryCache.Get<byte[]>("CONTENT_" + documentName);

                            if(cacheContent == null) {
                                string downloadfilePath = docstoreResponse.File;
                                content = new DocumentDownloadManager().DownloadFileContent(downloadfilePath);
                                this.memoryCache.Set("CONTENT_" + documentName, content);
                                this.logger.LogTrace(this.GetLogRecord("Not found in cache, Retrive from docstore. Request->Url:" + this.restClient.requestUrl + " Path=" + path, "DocstoreClient.GetFileContent", path));
                            }
                            else {
                                content = cacheContent;
                                //this.logger.LogTrace(this.GetLogRecord("Retrived From Cache. Request->Url:" + this.restClient.requestUrl + " Path=" + path, "DocstoreClient.GetFileContent", path));
                            }
                        }
                        else {
                            string downloadfilePath = docstoreResponse.File;
                            content = new DocumentDownloadManager().DownloadFileContent(downloadfilePath);
                            this.memoryCache.Set("CONTENT_" + documentName, content);
                            this.logger.LogTrace(this.GetLogRecord("Not found in cache, Retrive from docstore. Request->Url:" + this.restClient.requestUrl + " Path=" + path, "DocstoreClient.GetFileContent", path));
                        }
                    }
                    else {
                        string downloadfilePath = docstoreResponse.File;
                        content = new DocumentDownloadManager().DownloadFileContent(downloadfilePath);
                        this.memoryCache.Set("CONTENT_" + documentName, content);
                        this.logger.LogTrace(this.GetLogRecord("Not found in cache, Retrive from docstore. Request->Url:" + this.restClient.requestUrl + " Path=" + path, "DocstoreClient.GetFileContent", path));
                    }
                }
                else {
                    string error = await responseMessage.Content.ReadAsStringAsync();
                    stopwatch.Stop();
                    this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl + " ErrorMessage:" + error, "DocstoreClient.GetFileContent", path, stopwatch.ElapsedMilliseconds));

                    if(responseMessage.StatusCode == System.Net.HttpStatusCode.Forbidden) {
                       throw new ApplicationException("Token Expired. Please enter valid token.");
                    }
                }
            }

            return content;
        }

        public async Task<byte[]> GetFileContent(string documentUrl) {
            return new DocumentDownloadManager().DownloadFileContent(documentUrl);
        }

        public async Task<DocstoreApiResponse> CreateTempDocumeent(string documentName, string path, byte[] filecontent, Dictionary<string, string> metadata, int timetoLive = 5) {
            DocstoreApiResponse apiResponse = new DocstoreApiResponse();
            try {
                this.restClient.requestUrl = string.Format("{0}/{1}/", this.docstoreServiceendPoint, "documents-create-temp");

                //this.logger.LogTrace(this.GetLogRecord("Request->Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.CreateTempDocumeent", path));
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(documentName, Encoding.UTF8), "name");
                content.Add(new StringContent(path, Encoding.UTF8), "path");
                content.Add(new StringContent(timetoLive.ToString(), Encoding.UTF8), "ttl");
                content.Add(new ByteArrayContent(filecontent), "file", documentName);
                content.Add(new StringContent(JsonConvert.SerializeObject(metadata), Encoding.UTF8, "application/json"), "metadata");


                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                HttpResponseMessage responseMessage = await this.restClient.Post(content);
                if(responseMessage.IsSuccessStatusCode) {
                    apiResponse.Status = ApiStatus.Success;
                    var responseString = await responseMessage.Content.ReadAsStringAsync();
                    apiResponse.Response = GetDocstoreResponse(responseString);
                    stopwatch.Stop();
                    this.logger.LogTrace(this.GetLogRecord("Request Success -> Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.CreateTempDocumeent", path, stopwatch.ElapsedMilliseconds));
                }
                else {
                    apiResponse.Status = ApiStatus.Error;
                    apiResponse.Message = await responseMessage.Content.ReadAsStringAsync();

                    stopwatch.Stop();
                    this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl + " documentName:" + documentName + " ErrorMessage:" + apiResponse.Message, "DocstoreClient.CreateTempDocumeent", path, stopwatch.ElapsedMilliseconds));
                }
            } catch(Exception ex) {
                apiResponse.Status = ApiStatus.Error;
                apiResponse.Message = ex.Message;

                this.logger.LogError(this.GetLogRecord("Request Error -> Url:" + this.restClient.requestUrl + " documentName:" + documentName, "DocstoreClient.CreateTempDocumeent", path), ex);
            }

            return apiResponse;
        }

        private DocstoreResponse GetDocstoreResponse(string responseString) {
            var jObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);
            var metadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(jObject["metadata"].ToString());
            jObject.Remove("metadata");
            var response = JsonConvert.DeserializeObject<DocstoreResponse>(jObject.ToString());
            response.Metadata = metadata;

            return response;
        }
        protected LogRecord GetLogRecord(string message, string eventType, string docstorePath = "", long totalTimeTaken = 0) {
            LogRecord logRecord = new LogRecord();
            logRecord.TextMessage = message;
            logRecord.EventType = eventType;
            logRecord.DocStorePath = docstorePath;
            logRecord.TimeTaken = totalTimeTaken;
            return logRecord;
        }
    }
}
