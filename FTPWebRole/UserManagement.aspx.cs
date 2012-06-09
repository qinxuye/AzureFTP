using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FTPWebRole
{
    public partial class UserManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var identity = HttpContext.Current.User.Identity;
            if (identity.IsAuthenticated)
            {
                var accountManager = new AccountManager();
                bool isSuperUser = identity.Name.ToLower() == AccountManager.AdminUserName
                    || accountManager.IsSuperUser(identity.Name);

                if (!isSuperUser)
                    Response.Redirect("~/");
            }
        }
    }

    public class UserManagementProvider
    {
        public static List<FtpAccount> GetAllFtpAccount()
        {
            var identity = HttpContext.Current.User.Identity;
            if (identity.IsAuthenticated)
            {
                bool filterSuperUser = identity.Name.ToLower() != AccountManager.AdminUserName;
                var accountManager = new AccountManager();

                if (!accountManager.IsSuperUser(identity.Name))
                    return new List<FtpAccount>();

                return accountManager.QueryAllFtpAccount(filterSuperUser);
            }

            return new List<FtpAccount>();

        }

        public static void UpdateFtpAccount(FtpAccount ftpAccount)
        {
            var identity = HttpContext.Current.User.Identity;
            if (identity.IsAuthenticated)
            {
                var accountManager = new AccountManager();

                string Username = ftpAccount.Username;
                bool isActive = ftpAccount.IsActive;
                bool isSuperUser = identity.Name.ToLower() == AccountManager.AdminUserName ?
                    ftpAccount.IsSuperUser : false;

                accountManager.UpdateFtpAccount(Username, isSuperUser, isActive);
            }
        }

        public static void DeleteFtpAccount(FtpAccount ftpAccount)
        {
            var identity = HttpContext.Current.User.Identity;
            if (identity.IsAuthenticated)
            {
                var accountManager = new AccountManager();
                accountManager.DeleteFtpAccount(ftpAccount.Username);
            }
        }
    }

}