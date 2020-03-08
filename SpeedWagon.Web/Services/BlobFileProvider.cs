using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SpeedWagon.Runtime.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Services
{
    public class BlobFileProvider : IFileProvider
    {
        private readonly string _connectionString;
        private readonly string _container;

        private CloudBlobContainer _cloudBlobContainer;


        public BlobFileProvider(string connectionString, string container)
        {
            this._connectionString = connectionString;
            this._container = container;

            CloudStorageAccount storageAccount;
            CloudStorageAccount.TryParse(this._connectionString, out storageAccount);
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            this._cloudBlobContainer = cloudBlobClient.GetContainerReference(this._container);
        }

        public string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public string Combine(string path1, string path2, string path3)
        {
            return Path.Combine(path1, path2, path3);
        }

        public void CreateDirectory(string path)
        {
            // No-op
        }

        public void Delete(string path)
        {
            this._cloudBlobContainer.GetBlockBlobReference(path).DeleteIfExistsAsync();
        }

        public bool Exists(string path)
        {
            return this._cloudBlobContainer.GetBlockBlobReference(path).ExistsAsync().Result;
        }

        public async Task<Stream> GetStream(string path)
        {
            return await this._cloudBlobContainer.GetBlockBlobReference(path).OpenWriteAsync();
        }

        public async Task<string[]> List(string path, string pattern, bool recursive)
        {
            await this._cloudBlobContainer.CreateIfNotExistsAsync();

            IList<string> blobs = new List<string>();

            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var results = await this._cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                foreach (IListBlobItem item in results.Results)
                {
                    blobs.Add(item.StorageUri.ToString());
                }
            }
            while (blobContinuationToken != null);


            return blobs.ToArray();
        }

        public Task<string> ReadAllText(string path)
        {
            return this._cloudBlobContainer.GetBlockBlobReference(path).DownloadTextAsync();
        }

        public async Task WriteAllText(string path, string contents)
        {
            await this._cloudBlobContainer.GetBlockBlobReference(path).UploadTextAsync(contents);
        }
    }
}