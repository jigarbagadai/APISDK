using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Coherent.Docstore {
    internal class RestClient {
        private HttpClient client;

        public string requestUrl;

        private RetryPolicy<HttpResponseMessage> retryPolicy = null;

        public ILogger logger = null;

        private string token = null;

        internal RestClient(string token, int maxRetryAttempts, TimeSpan pauseBetweenFailures, ILoggerFactory LoggerFactory) {
            this.token = token;
            logger = LoggerFactory.CreateLogger("SecuritySDK");
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(500);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            client.DefaultRequestHeaders.Add("AUTHORIZATION", string.Format("bearer {0}", token));

            HttpStatusCode[] httpStatusCodesWorthRetrying = {
                HttpStatusCode.RequestTimeout, // 408
                HttpStatusCode.InternalServerError, // 500
                HttpStatusCode.BadGateway, // 502
                HttpStatusCode.ServiceUnavailable, // 503
                HttpStatusCode.GatewayTimeout, // 504
                HttpStatusCode.NotFound //404
            };

            this.retryPolicy = Policy.Handle<HttpRequestException>().OrResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains(r.StatusCode)).WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures, (exception, retryCount) => {
                if(exception != null && exception.Result != null) {
                    logger.LogError(exception.Exception, "Error generated from Security SDK");
                    Debug.WriteLine("Retry -  Result Status Code:" + exception.Result.StatusCode);
                }
            });
        }

        internal async Task<HttpResponseMessage> Post(HttpContent content = null) {
            if(content == null) {
                throw new ArgumentNullException("The supplied content to POST is null");
            }

            HttpResponseMessage response = await this.retryPolicy.ExecuteAsync(async () => {
                return await client.PostAsync(this.requestUrl, content);
            });

            return response;
        }

        internal async Task<HttpResponseMessage> Put(HttpContent content = null) {
            HttpResponseMessage response = await this.retryPolicy.ExecuteAsync(async () => {
                return await client.PutAsync(this.requestUrl, content);
            });

            return response;
        }

        internal async Task<HttpResponseMessage> Delete() {
            HttpResponseMessage response = await this.retryPolicy.ExecuteAsync(async () => {
                return await client.DeleteAsync(this.requestUrl);
            });

            return response;
        }

        internal async Task<HttpResponseMessage> Get() {
            HttpResponseMessage response = await this.retryPolicy.ExecuteAsync(async () => {
                return await client.GetAsync(this.requestUrl);
            });

            return response;
        }

        internal async Task<HttpResponseMessage> GetHttpResponse() {
            HttpResponseMessage response = await this.retryPolicy.ExecuteAsync(async () => {
                return await client.GetAsync(this.requestUrl);
            });

            return response;
        }

        internal async Task<HttpResponseMessage> Get(Dictionary<string, string> additionalHeaders) {
            foreach(var header in additionalHeaders) {
                if(!client.DefaultRequestHeaders.Contains(header.Key)) {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            HttpResponseMessage response = await this.retryPolicy.ExecuteAsync(async () => {
                return await client.GetAsync(this.requestUrl);
            });

            return response;
        }
    }
}
