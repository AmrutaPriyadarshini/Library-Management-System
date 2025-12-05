using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class LibraryLogin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (txtId.Text == "Amruta" & txtPwd.Text == "Amruta@678")
        {
            Session["isLogin"] = "yes";
            Response.Redirect("StudentReg.aspx");
        }
        else
        {
            lblerror.Text = "Invalid ID , Password .";
        }

    }
}