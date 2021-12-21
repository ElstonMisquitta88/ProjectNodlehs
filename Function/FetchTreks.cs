using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System;
using Azure.Storage;
using Newtonsoft.Json;
using System.IO;

namespace Trek.Function
{

    public class FetchTreks
    {

        private readonly ILogger _logger;

        public FetchTreks(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FetchTreks>();
        }

        [Function("FetchTreks")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var nfolder = query["nfolder"];
            List<ImageList> lstobj = new List<ImageList>();
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            // Create a service level SAS that only allows reading from service level APIs
            AccountSasBuilder sas = new AccountSasBuilder
            {
                // Allow access to blobs
                Services = AccountSasServices.Blobs,

                // Allow access to the service level APIs
                ResourceTypes = AccountSasResourceTypes.All,

                // Access expires in 1 hour!
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };

            // Allow read access
            sas.SetPermissions(AccountSasPermissions.All);

            // Create a SharedKeyCredential that we can use to sign the SAS token
            string StorageAccountName = Environment.GetEnvironmentVariable("K_StorageAccountName");
            string StorageAccountKey = Environment.GetEnvironmentVariable("K_StorageAccountKey");
            StorageSharedKeyCredential credential = new StorageSharedKeyCredential(StorageAccountName, StorageAccountKey);

            // Build a SAS URI
            string StorageAccountBlobUri = Environment.GetEnvironmentVariable("K_StorageAccountBlobUri");
            UriBuilder sasUri = new UriBuilder(StorageAccountBlobUri);
            sasUri.Query = sas.ToSasQueryParameters(credential).ToString();

            // Create a client that can authenticate with the SAS URI
            string? containerName = null;
            containerName = nfolder; // Fetch the Container name Dynamically // data
            BlobServiceClient service = new BlobServiceClient(sasUri.Uri);

            if (containerName is null)
            {
                response = req.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
            {
                if (service.GetBlobContainerClient(containerName).GetBlobs() is null)
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {

                    foreach (BlobItem blob in service.GetBlobContainerClient(containerName).GetBlobs())
                    {
                        BlobClient blobClient = service.GetBlobContainerClient(containerName).GetBlobClient(blob.Name);
                        ImageList obj = new ImageList();
                        obj.ImageName = Path.GetFileNameWithoutExtension(blobClient.Name);
                        obj.ImageUrl = blobClient.Uri.ToString();
                        lstobj.Add(obj);
                    }
                    response = req.CreateResponse(HttpStatusCode.OK);
                    response.WriteString(JsonConvert.SerializeObject(lstobj));

                }
            }
            return response;
        }
    }
}
