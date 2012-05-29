using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using AzureFtpServer.Provider;

namespace AzureFtpServer.Provider {

    public class StorageProviderEventArgs : EventArgs {
        public StorageOperation Operation;
        public StorageOperationResult Result;
    }

    public sealed class AzureBlobStorageProvider
    {
     

        #region Delegates

        public delegate void PutCompletedEventHandler(AzureCloudFile o, StorageOperationResult r);

        #endregion

        private CloudStorageAccount _account;
        private CloudBlobClient _blobClient;

        public AzureBlobStorageProvider(String containerName)
        {
            Initialise(containerName);
        }

     
        public AzureBlobStorageProvider()
        {
            Initialise(null); 
        }

        private Uri BaseUri
        {
            get { return new Uri(StorageProviderConfiguration.BaseUri + "/" + StorageProviderConfiguration.AccountName + @"/" + ContainerName); }
        }

        public bool UseHttps { get; private set; }

        public Boolean RetryOnTimeout { get; set; }
        public Boolean UseAsynchCalls { get; set; }
        public String ContainerName { private get; set; }

        #region IStorageProvider Members

        public event EventHandler<StorageProviderEventArgs> StorageProviderOperationCompleted;

        public String FolderDelimiter
        {
            get { return "/"; }
        }

        #endregion

       
        private void Initialise(String containerName)
        {
           
            if (String.IsNullOrEmpty(containerName))
                throw new ArgumentException("You must provide the base Container Name", "containerName");
            
            ContainerName = containerName;

            if (StorageProviderConfiguration.Mode == Modes.Development || StorageProviderConfiguration.Mode == Modes.Debug)
            {
                _account = CloudStorageAccount.DevelopmentStorageAccount;
                _blobClient = _account.CreateCloudBlobClient();
                _blobClient.Timeout = new TimeSpan(0, 0, 0, 50);
            }
            else
            {
                _account = new CloudStorageAccount(
                    new StorageCredentialsAccountAndKey(StorageProviderConfiguration.AccountName, StorageProviderConfiguration.AccountKey), UseHttps);
                _blobClient = _account.CreateCloudBlobClient();
            
                _blobClient.Timeout = new TimeSpan(0, 0, 0, 15);

            }
      
            _blobClient.GetContainerReference(ContainerName).CreateIfNotExist();
            
        }

        #region "Storage operations"

      
        public void Put(AzureCloudFile o)
        {
            if (o.Data == null)
                throw new ArgumentNullException("o", "AzureCloudFile cannot be null.");

            if (o.Uri == null)
                throw new ArgumentException("Parameter 'Uri' of argument 'o' cannot be null.");

            string path = o.Uri.ToString();

            if (path.StartsWith(@"/"))
                path = path.Remove(0, 1);

            if (path.StartsWith(@"\\"))
                path = path.Remove(0, 1);

            if (path.StartsWith(@"\"))
                path = path.Remove(0, 1);

   
            path = path.Replace(@"\\", @"\");

            CloudBlobContainer container = _blobClient.GetContainerReference(ContainerName);
            container.CreateIfNotExist();

     
            BlobContainerPermissions perms = container.GetPermissions();
            if (perms.PublicAccess != BlobContainerPublicAccessType.Container)
            {
                perms.PublicAccess = BlobContainerPublicAccessType.Container;
                container.SetPermissions(perms);
            }

            String uniqueName = path;
            CloudBlob blob = container.GetBlobReference(uniqueName);

       
            AsyncCallback callback = PutOperationCompleteCallback;
            blob.BeginUploadFromStream(new MemoryStream(o.Data), callback, o.Uri);

            // Uncomment for synchronous upload
            //blob.UploadFromStream(new System.IO.MemoryStream(o.Data));
        }

      
        public void Delete(AzureCloudFile o)
        {
            string path = UriPathToString(o.Uri);
            if (path.StartsWith("/"))
                path = path.Remove(0, 1);

            CloudBlobContainer c = GetContainerReference(ContainerName);
            CloudBlob b = c.GetBlobReference(path);
            if (b != null)
                b.BeginDelete(new AsyncCallback(DeleteOperationCompleteCallback), o.Uri);
            else
                throw new ArgumentException("The container reference could not be retrieved from storage provider.", "o");
        }

       
        public AzureCloudFile Get(string path, bool downloadData)
        {
            var u = new Uri(path, UriKind.RelativeOrAbsolute);
            string blobPath = UriPathToString(u);

            if (blobPath.StartsWith(@"/"))
                blobPath = blobPath.Remove(0, 1);

            blobPath = RemoveContainerName(blobPath);

            var o = new AzureCloudFile();
            CloudBlobContainer c = GetContainerReference(ContainerName);

            CloudBlob b = null;

            try
            {
                b = c.GetBlobReference(blobPath);
                b.FetchAttributes();
                o = new AzureCloudFile
                        {
                            Meta = b.Metadata,
                            StorageOperationResult = StorageOperationResult.Completed,
                            Uri = new Uri(blobPath, UriKind.RelativeOrAbsolute),
                            LastModified = b.Properties.LastModifiedUtc,
                            ContentType = b.Properties.ContentType,
                            Size = b.Properties.Length
                        };

                o.Meta.Add("ContentType", b.Properties.ContentType);
            }
            catch (StorageClientException ex)
            {
                if (ex.ErrorCode == StorageErrorCode.BlobNotFound)
                {
                    throw new FileNotFoundException(
                        "The storage provider was unable to locate the object identified by the given URI.",
                        u.ToString());
                }

                if (ex.ErrorCode == StorageErrorCode.ResourceNotFound)
                {
                    return null;
                }
            }

        
            // TODO: Implement asynchronous calls for this
            try
            {
                if (downloadData && b != null)
                {
                    byte[] data = b.DownloadByteArray();
                    o.Data = data;
                }
            }

            catch (TimeoutException)
            {
                if (RetryOnTimeout)
                {
                    Get(blobPath, downloadData); 
                    // TODO: Implement retry attempt limitation
                }
                else
                {
                    throw;
                }
            }

            return o;
        }


     
        public bool CheckBlobExists(string path)
        {
            string p = path;

            if (p.StartsWith("/"))
            {
                p = p.Remove(0, 1);
            }

            if (p.StartsWith(ContainerName + @"/"))
            {
                p = p.Replace(ContainerName + @"/", @"");
            }

            CloudBlobContainer c = GetContainerReference(ContainerName);
            CloudBlob b = c.GetBlobReference(p);

            try
            {
                b.FetchAttributes();
                return true;
            }
            catch (StorageClientException e)
            {
                if (e.ErrorCode == StorageErrorCode.ResourceNotFound)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public CloudDirectoryCollection GetDirectoryListing(string path)
        {
            path = ParsePath(path);
            CloudBlobContainer container = _blobClient.GetContainerReference(ContainerName);
            var directories = new CloudDirectoryCollection();

            if (path == "")
            {
                directories.AddRange(
                    container.ListBlobs().OfType<CloudBlobDirectory>().Select(
                        dir => new CloudDirectory {Path = dir.Uri.ToString()}));
            }
            else
            {
                CloudBlobDirectory parent = container.GetDirectoryReference(path);
                directories.AddRange(
                    parent.ListBlobs().OfType<CloudBlobDirectory>().Select(
                        dir => new CloudDirectory {Path = dir.Uri.ToString()}));
            }

            return directories;
        }

        public CloudFileCollection GetFileListing(string path)
        {
            String prefix = String.Concat(ContainerName, "/", ParsePath(path));
            var files = new CloudFileCollection();
            files.AddRange(
                _blobClient.ListBlobsWithPrefix(prefix).OfType<CloudBlob>().Select(
                    blob =>
                    new AzureCloudFile
                        {
                            Meta = blob.Metadata,
                            Uri = blob.Uri,
                            Size = blob.Properties.Length,
                            ContentType = blob.Properties.ContentType
                        }));

            return files;
        }

      
        public void Overwrite(string originalPath, AzureCloudFile newObject)
        {
           
            if (!CheckBlobExists(originalPath))
            {
                throw new FileNotFoundException("The path supplied does not exist on the storage provider.",
                                                originalPath);
            }

      
            Put(newObject);
        }

        
        public StorageOperationResult Rename(string originalPath, string newPath)
        {
            var u = new Uri(newPath, UriKind.RelativeOrAbsolute);
            CloudBlobContainer c = GetContainerReference(ContainerName);

            newPath = UriPathToString(u);
            if (newPath.StartsWith("/"))
                newPath = newPath.Remove(0, 1);

            originalPath = UriPathToString(new Uri(originalPath, UriKind.RelativeOrAbsolute));
            if (originalPath.StartsWith("/"))
                originalPath = originalPath.Remove(0, 1);

            CloudBlob newBlob = c.GetBlobReference(newPath);
            CloudBlob originalBlob = c.GetBlobReference(originalPath);

       
            if (!CheckBlobExists(originalPath))
            {
                throw new FileNotFoundException("The path supplied does not exist on the storage provider.",
                                                originalPath);
            }

            newBlob.CopyFromBlob(originalBlob);

            try
            {
                newBlob.FetchAttributes();
                originalBlob.Delete();
                return StorageOperationResult.Completed;
            }
            catch (StorageClientException e)
            {
                throw;
            }
        }

        public void CreateDirectory(string path)
        {
            if (path.StartsWith("/"))
                path = path.Remove(0, 1);

            CloudBlobContainer container = _blobClient.GetContainerReference(ContainerName);
            string blobName = String.Concat(path, "/required.req");
            CloudBlob blob = container.GetBlobReference(blobName);

            string message = "#REQUIRED: At least one file is required to be present in this folder.";
            byte[] msg = Encoding.ASCII.GetBytes(message);
            blob.UploadByteArray(msg);

            BlobProperties props = blob.Properties;
            props.ContentType = "text/text";
            blob.SetProperties();
        }

     
        public bool IsValidPath(string path)
        {
            if (path != null)
                if (path == "/")
                    return true;

            CloudBlobContainer c = GetContainerReference(ContainerName);
            if (c.HasDirectories(path))
                return true;

            if (c.HasFiles(path))
                return true;

            CloudBlob b = c.GetBlobReference(path);
            try
            {
                b.FetchAttributes();
            }
            catch (StorageClientException ex)
            {
                if (ex.ErrorCode == StorageErrorCode.ResourceNotFound)
                    return false;
                else
                {
                    throw;
                }
            }

            return false;
        }

        #endregion

        #region "Helper methods"

       
        private static string ExtractContainerName(String path)
        {
            return path.Split('/')[0].ToLower(); // Azure requires URI's in lowercase
        }

    
        private CloudBlobContainer GetContainerReference(string containerName)
        {
           
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            container.CreateIfNotExist();

            BlobContainerPermissions permissions = container.GetPermissions();
            if (permissions.PublicAccess != BlobContainerPublicAccessType.Container)
            {
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                container.SetPermissions(permissions);
            }

            return container;
        }

    
        private String ParsePath(String path)
        {
            if (!path.EndsWith("/"))
                path += "/";

            switch (path)
            {
                case "/":
                    path = "";
                    break;
                default:
                    if (!path.EndsWith("/"))
                    {
                        path += "/";
                    }
                    else
                    {
                        path = path.Remove(0, 1);
                    }

                    break;
            }

            path = path.Replace(@"//", "/");

            return path;
        }

    
        private string UriPathToString(Uri u)
        {
            if (u.IsAbsoluteUri)
            {
                return u.PathAndQuery;
            }
            else
            {
                return u.ToString();
            }
        }

        private string RemoveContainerName(string path)
        {
            path = path.Replace(ContainerName + @"/", "");
            return path;
        }

        #endregion

        #region "Callbacks"

        private void PutOperationCompleteCallback(IAsyncResult result)
        {
            var o = (Uri) result.AsyncState;
            if (StorageProviderOperationCompleted == null) return;
            var a = new StorageProviderEventArgs
                        {Operation = StorageOperation.Put, Result = StorageOperationResult.Created};

       
            StorageProviderOperationCompleted(o, a);
        }

   
        private void DeleteOperationCompleteCallback(IAsyncResult result)
        {
            var o = (Uri) result.AsyncState;

            if (StorageProviderOperationCompleted == null) return;
            var a = new StorageProviderEventArgs
                        {Operation = StorageOperation.Delete, Result = StorageOperationResult.Deleted};
    
            StorageProviderOperationCompleted(o, a);
        }

        #endregion
    }
}