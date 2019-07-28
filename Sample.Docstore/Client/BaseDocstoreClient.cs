using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coherent.Docstore {
    public class BaseDocstoreClient {
        internal RestClient restClient;

        private string baseUrl;

        protected ILogger logger;

        public BaseDocstoreClient(string token, string servicebaseUrl, ILoggerFactory LoggerFactory) {
            this.restClient = new RestClient(token, 0, TimeSpan.FromSeconds(1), LoggerFactory);
            this.logger = LoggerFactory.CreateLogger("securityClient");
            this.baseUrl = servicebaseUrl;
        }

        public BaseDocstoreClient(string token, int maxRetryAttempts, TimeSpan pauseBetweenFailures, string servicebaseUrl, ILoggerFactory LoggerFactory) {
            this.restClient = new RestClient(token, maxRetryAttempts, pauseBetweenFailures, LoggerFactory);
            this.logger = this.restClient.logger;
            this.baseUrl = servicebaseUrl;
        }
    }
}
