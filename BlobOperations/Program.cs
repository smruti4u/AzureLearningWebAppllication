using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobOperations
{
    class Program
    {
        const string fileToUpload = "input2.txt";
        const string saConnectionString = "DefaultEndpointsProtocol=https;AccountName=storageaclearn;AccountKey=4ouGCOo97jErZZGnyJnL76n6yYwk5tWehRDVMQ0P2WZvzWEi1wrZ4UG5o0jUp32gpdEVDnk/zudAvhvSPmOq0Q==;EndpointSuffix=core.windows.net";
        static async Task Main(string[] args)
        {
            await BasicBlobOpertaion();
            Console.WriteLine("Execution Completed");
        }

        static async Task BasicBlobOpertaion()
        {
            var stoargeAccount = ParseStoargeAccount();

            CloudBlobClient blobClient = stoargeAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("vscontainer");
            await container.CreateIfNotExistsAsync();
            
            CloudBlobDirectory dir = container.GetDirectoryReference("vsupload");
            CloudBlockBlob blob = dir.GetBlockBlobReference(fileToUpload);
            await blob.UploadFromFileAsync(fileToUpload);

            blob.Metadata["Author"] = "Client Code";
            blob.Metadata["Priority"] = "High";            
            await blob.SetMetadataAsync();
        }

        static CloudStorageAccount ParseStoargeAccount()
        {
            CloudStorageAccount cloudStorageAccount = null;
            try
            {
                cloudStorageAccount = CloudStorageAccount.Parse(saConnectionString);
            }
            catch (FormatException exe)
            {

            }
            catch (Exception ex)
            {

            }

            return cloudStorageAccount;
        }

    }
}
