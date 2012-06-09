using AzureFTP;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.Provider;
using FTPWebRole;

namespace AzureFtpServer.Azure
{
    public class AzureFileSystemFactory : IFileSystemClassFactory
    {
        #region Implementation of IFileSystemClassFactory

        public IFileSystem Create(string sUser, string sPassword)
        {
            string containerName = "";

            // TODO: Put your authentication call here. Return NULL if authentication fails. Return an initialised AzureFileSystem() object if authentication is successful.
            // In the example below, to demonstrate, any username will work so long as the password == "test".
            // Remember: you can plug in your own authentication & authorisation API to fetch the correct container name for the specified user.
            /*
            #region "REPLACE THIS WITH CODE TO YOUR OWN AUTHENTICATION API"
            if (sPassword == "test")
            {
                containerName = sUser;
            }
            else
            {
                return null;
            }
            #endregion  */

            var accountManager = new AccountManager();
            if (accountManager.CertificateAccount(sUser, sPassword))
                containerName = sUser;
            else
                return null;

            var system = new AzureFileSystem(sUser, sPassword, containerName, Modes.Development);
            return system;
        }

        #endregion
    }
}