using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace FTPWebRole.Account
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
        }

        protected void FtpAccount_Login(object sender, EventArgs e)
        {
            string username = UserName.Text;
            string password = Password.Text;

            var accountManager = new AccountManager();
            bool loginSuccess = accountManager.CertificateAccount(username, password);
            if (!loginSuccess)
            {
                FailureText.Text = "登录失败，请确保用户名和密码输入正确";
                return;
            }

            FormsAuthentication.SetAuthCookie(username, false /* createPersistentCookie */);
            Response.Redirect("~/");
        }

    }
}
