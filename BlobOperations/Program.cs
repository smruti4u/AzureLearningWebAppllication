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
            Console.Read();
        }

        static async Task BasicBlobOpertaion()
        {
            var stoargeAccount = ParseStoargeAccount();

            CloudBlobClient blobClient = stoargeAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("vscontainer1");
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Off, null, null);

            SharedAccessAccountPolicy policy = new SharedAccessAccountPolicy()
            {
                Permissions = SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.Write,
                Services = SharedAccessAccountServices.Blob,
                ResourceTypes = SharedAccessAccountResourceTypes.Container | SharedAccessAccountResourceTypes.Object | SharedAccessAccountResourceTypes.Service,
                SharedAccessExpiryTime = DateTime.Now.AddMinutes(5)
            };
            string saasToken = stoargeAccount.GetSharedAccessSignature(policy);

            CloudBlobDirectory dir = container.GetDirectoryReference("vsupload");
            CloudBlockBlob blob = dir.GetBlockBlobReference(fileToUpload);
            await blob.UploadFromFileAsync(fileToUpload);


           // await HandleConcurrencyAsync(blob, ConcurrencyType.Default);
            //await HandleConcurrencyAsync(blob, ConcurrencyType.Optimistic);
            await HandleConcurrencyAsync(blob, ConcurrencyType.Pessimistic);

            blob.Metadata["Author"] = "Client Code";
            blob.Metadata["Priority"] = "High";            
            await blob.SetMetadataAsync();

            BlobContinuationToken token = null;
            do
            {
                BlobResultSegment result = await dir.ListBlobsSegmentedAsync(true, BlobListingDetails.Metadata, 1, token, null, null);
                token = result.ContinuationToken;

                foreach(IListBlobItem item in result.Results)
                {
                    Console.WriteLine($"The Blob Uri {item.Uri} + {saasToken}");
                }
            }
            while (token != null);
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

        private static async Task<bool> HandleConcurrencyAsync(CloudBlockBlob blob, ConcurrencyType type)
        {
            switch(type)
            {
                case ConcurrencyType.Default:
                    blob.Metadata["Type"] = "Default";
                    await blob.SetMetadataAsync();
                    break;
                case ConcurrencyType.Optimistic:
                    blob.Metadata["Type"] = "Optimistic";
                    AccessCondition accessCondition = new AccessCondition()
                    {
                        IfMatchETag = blob.Properties.ETag
                    };

                    try
                    {
                        await blob.SetMetadataAsync(accessCondition, null, null);
                    }
                    catch(Exception exe)
                    {
                        return false;
                    }
                    break;
                case ConcurrencyType.Pessimistic:
                    blob.Metadata["Type"] = "Pessimistic";
                    string leaseId = await blob.AcquireLeaseAsync(TimeSpan.FromMinutes(1));
                    AccessCondition accessConditionOptimistic = new AccessCondition()
                    {
                       LeaseId = leaseId
                    };

                    try
                    {
                        await blob.SetMetadataAsync(accessConditionOptimistic, null, null);
                    }
                    catch (Exception exe)
                    {
                        return false;
                    }
                    break;
                default:
                    break;

            }

            return true;
        }

    }
}
