using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace FTPWebRole
{
    public class FtpAccount : TableServiceEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsSuperUser { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class FtpAccountDataContext : TableServiceContext
    {
        public IQueryable<FtpAccount> FtpAccounts
        {
            get
            {
                return this.CreateQuery<FtpAccount>("AzureFtpAccount");
            }
        }

        public FtpAccountDataContext(string baseAddress, StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
            var tableStorage = new CloudTableClient(baseAddress, credentials);
            tableStorage.CreateTableIfNotExist("AzureFtpAccount");
        }

        public void AddFtpAccount(FtpAccount account)
        {
            this.AddObject("AzureFtpAccount", account);
            this.SaveChanges();
        }

        public void DeleteFtpAccount(FtpAccount account)
        {
            this.DeleteObject(account);
            this.SaveChanges();
        }

        public void UpdateFtpAccount(FtpAccount account)
        {
            this.UpdateObject(account);
            this.SaveChanges();
        }

    }


    public class AccountManager
    {
        private FtpAccountDataContext context;
        private static object OperationLock = new object();

        public static string AdminUserName = "chine";

        public AccountManager()
        {
            var account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            context = new FtpAccountDataContext(account.TableEndpoint.ToString(), account.Credentials);
        }

        public bool CertificateAccount(string Username, string Password)
        {
            string md5Password = FormsAuthentication.HashPasswordForStoringInConfigFile(Password, "MD5");

            var accounts = (from account in context.FtpAccounts
                            where (account.Username == Username
                                    && account.Password == md5Password
                                    && account.IsActive)
                            select account).ToList();
            return accounts.Count > 0;
        }

        public bool IsSuperUser(string Username)
        {
            if (Username.ToLower() == AdminUserName)
                return true;

            var accounts = (from account in context.FtpAccounts
                           where account.Username == Username
                           select account).ToList();
            if (accounts.Count() > 0)
                return accounts.First().IsSuperUser;
            return false;
        }

        public FtpAccount AddAccount(string Username, string Password, bool IsSuperUser)
        {
            lock (OperationLock)
            {
                var accounts = (from account in context.FtpAccounts
                               where account.Username == Username
                               select account).ToList();
                if (accounts.Count() > 0)
                    return accounts.First();

                bool isActive = true;
                DateTime createDate = DateTime.Now;
                string md5Password = FormsAuthentication.HashPasswordForStoringInConfigFile(Password, "MD5");

                FtpAccount ftpAccount = new FtpAccount
                {
                    Username = Username,
                    Password = md5Password,
                    IsSuperUser = IsSuperUser,
                    IsActive = isActive,
                    CreateDate = createDate,
                    PartitionKey = "AzureFtpAccount",
                    RowKey = Username
                };
                context.AddFtpAccount(ftpAccount);

                return ftpAccount;
            }
        }

        public FtpAccount AddAccount(string Username, string Password)
        {
            return this.AddAccount(Username, Password, false);
        }

        public void ActivateAccount(FtpAccount account)
        {
            lock (OperationLock)
            {
                if (account.IsActive == false)
                {
                    account.IsActive = true;
                    context.SaveChanges();
                }
            }
        }

        public void ActivateAccount(string Username)
        {
            var ftpAccount = (from account in context.FtpAccounts
                              where account.Username == Username
                              select account).ToList();
            if (ftpAccount.Count() == 0)
                return;

            this.ActivateAccount(ftpAccount.First());
        }

        public List<FtpAccount> QueryAllFtpAccount(bool filterSuperUser)
        {
            if (!filterSuperUser)
            {
                return (from account in context.FtpAccounts
                        select account).ToList();
            }

            return (from account in context.FtpAccounts
                    where !account.IsSuperUser
                    select account).ToList();
        }

        public void UpdateFtpAccount(string Username, bool isSuperUser, bool isActive)
        {
            var ftpAccount = (from account in context.FtpAccounts
                              where account.Username == Username
                              select account).First();
            ftpAccount.IsSuperUser = isSuperUser;
            ftpAccount.IsActive = isActive;

            this.context.UpdateFtpAccount(ftpAccount);
        }

        public void DeleteFtpAccount(string Username)
        {
            var ftpAccount = (from account in context.FtpAccounts
                              where account.Username == Username
                              select account).First();
            this.context.DeleteFtpAccount(ftpAccount);
        }

    }

}