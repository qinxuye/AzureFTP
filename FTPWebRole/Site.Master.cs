using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FTPWebRole
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool isSuperUser = false;

            var identity = HttpContext.Current.User.Identity;
            if (identity.IsAuthenticated)
            {
                var accountManager = new AccountManager();
                isSuperUser = accountManager.IsSuperUser(identity.Name);
            }

            if (isSuperUser)
            {
                NavigationMenu.Items.Add(new MenuItem() {
                    Text = "管理用户",
                    NavigateUrl = "~/UserManagement.aspx"
                });
            }

        }
    }
}
