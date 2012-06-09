using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace FTPWebRole.Account
{
    public partial class Register : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void RegisterUser_CreatedUser(object sender, EventArgs e)
        {
            string username = UserName.Text;
            string password = Password.Text;

            System.Diagnostics.Trace.WriteLine(username + " start to register");

            var accountManager = new AccountManager();
            accountManager.AddAccount(username, password);

            System.Diagnostics.Trace.WriteLine(username + " registered");

            FormsAuthentication.SetAuthCookie(username, false /* createPersistentCookie */);

            string continueUrl = Request.QueryString["ReturnUrl"];
            if (String.IsNullOrEmpty(continueUrl))
            {
                continueUrl = "~/";
            }
            Response.Redirect(continueUrl);
        }

    }
}
