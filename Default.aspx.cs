using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

public partial class _Default : System.Web.UI.Page
{
    protected StringBuilder sbTableData = new StringBuilder();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadBooks();
        }
    }

    private void LoadBooks(string searchCondition = "")
    {
        if (searchCondition != "") { searchCondition = " WHERE " + searchCondition; }
        string conString = WebConfigurationManager.ConnectionStrings["Pubs"].ConnectionString;
        string sql = "SELECT TOP 20 BookID, Title, Author, ISBN, Publication, Subject, CurrentStock FROM Books " + searchCondition;
        SqlConnection con1 = new SqlConnection(conString);
        SqlCommand cmd = new SqlCommand(sql, con1);
        SqlDataAdapter sda1 = new SqlDataAdapter(cmd);
        DataTable dt1 = new DataTable();
        bool IsError = false;
        using (con1)
        {
            try
            {
                con1.Open();
                sda1.Fill(dt1);

            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error In Data: " + ex.Message;
                IsError = true;
            }
        }
        if (IsError) { return; }
        if (dt1.Rows.Count > 0)
        {
            foreach (DataRow row1 in dt1.Rows)
            {
                sbTableData.Append("<tr>").AppendFormat(
                    "<td>{0}</td>", row1["BookID"]).AppendFormat(
                    "<td>{0}</td>", row1["Title"]).AppendFormat(
                    "<td>{0}</td>", row1["Author"]).AppendFormat(
                    "<td>{0}</td>", row1["ISBN"]).AppendFormat(
                    "<td>{0}</td>", row1["Publication"]).AppendFormat(
                    "<td>{0}</td>", row1["Subject"]).AppendFormat(
                    "<td>{0}</td>", row1["CurrentStock"]).Append(
                    "</tr>");
            }
            lblMessage.Text = "";
            
        }
        else
        {
            lblMessage.Text = "No records found";
        }
        // lblMessage.Text += "<br />" + sql;
    }
        

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        String strCondition = "";
        if (txtSearchTitle.Text.Trim().Length > 0)
        {
            strCondition += " (Title LIKE '%" + mySafeSQLString(txtSearchTitle.Text) + "%') ";
        }
        if (txtSearchAuthor.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (Author LIKE '%" + mySafeSQLString(txtSearchAuthor.Text) + "%') ";
        }
        if (txtSearchISBN.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (ISBN = " + mySafeSQLString(txtSearchISBN.Text) + ") ";
        }
        if (txtSearchPublication.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (Publication LIKE '%" + mySafeSQLString(txtSearchPublication.Text) + "%') ";
        }
        if (txtSearchID.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (BookID = " + mySafeSQLString(txtSearchID.Text) + ") ";
        }
        if (txtSearchSub.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (Subject LIKE '%" + mySafeSQLString(txtSearchSub.Text) + "%') ";
        }

        LoadBooks(strCondition);
    }
    private string mySafeSQLString(string pcStr1)
    {
        return pcStr1.Trim().Replace(";", "").Replace("'", "");
    }
}
