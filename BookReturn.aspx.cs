using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class BookReturn : System.Web.UI.Page
{
    protected StringBuilder sbTableData = new StringBuilder();
    string connectionString = WebConfigurationManager.ConnectionStrings["Pubs"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        if ((Session["isLogin"] == null) || (Session["isLogin"].ToString() != "yes"))       // Check For Valid Login
        {
            Response.Redirect("LibraryLogin.aspx");
        }

        if (!IsPostBack)
        {
            txtReturnDate.Attributes["max"] = DateTime.Now.ToString("yyyy-MM-dd");
            // Show Last Record
            btnLast_Click(sender, e);
            LoadBooks();
        }

    }
    private void MyEnabled(bool plEnabled)
    {
        txtReturnDate.Enabled = plEnabled;
        btnCancel.Enabled = plEnabled;
        btnSave.Enabled = plEnabled;

        btnFirst.Enabled = !plEnabled;
        btnLast.Enabled = !plEnabled;
        btnPrev.Enabled = !plEnabled;
        btnNext.Enabled = !plEnabled;
        btnEdit.Enabled = !plEnabled;
        //btnDel.Enabled = !plEnabled;
        txtSearchStudentID.Enabled = !plEnabled;
        txtSearchIssueID.Enabled = !plEnabled;
        txtSearchBookID.Enabled = !plEnabled;
        txtSearchIsDate.Enabled = !plEnabled;
        txtSearcRehDate.Enabled = !plEnabled;
        btnSearch.Enabled = !plEnabled;
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (txtReturnDate.Text == "")
        {
            lblErr.Text = " Return Date is missing.";
            return;

        }

        string oldBookID = ViewState["OldBookID"]?.ToString();   // You should set this when loading the record for editing
        string newBookID = txtBookId.Text.Trim();

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string strSQL = "UPDATE BookIssued SET ReturnDate = @ReturnDate ,IssueStatus = @IssueStatus WHERE IssueID = @IssueID";

            using (SqlCommand cmd = new SqlCommand(strSQL, con))
            {
                cmd.Parameters.AddWithValue("@IssueID", txtIssueId.Text.Trim());
                cmd.Parameters.AddWithValue("@ReturnDate", txtReturnDate.Text.Trim());
                // Since IssueStatus is a BIT column, pass a bool (true = 1, false = 0)
                cmd.Parameters.AddWithValue("@IssueStatus", true);  // success = true

                //if (lblStatus.Text == "Pending")
                //{ 
                string strSQLStock = "UPDATE Books SET CurrentStock = CurrentStock + 1 WHERE BookID = @BookID ";
                SqlCommand cmdStock = new SqlCommand(strSQLStock, con);
                cmdStock.Parameters.AddWithValue("@BookID", txtBookId.Text.Trim());
                //}
                try
                {
                    con.Open();

                    int updated = cmd.ExecuteNonQuery();
                    if (updated > 0)
                    {
                        lblStatus.Text = "Success";
                        lblStatus.CssClass = "badge bg-success text-light fs-5 px-3 py-2 rounded-pill";

                        lblErr.Text = "Book return updated successfully!";
                    }
                    else
                    {
                        lblErr.Text = "Update failed. Please check the Issue ID.";
                    }
                    if (lblStatus.Text == "Pending")
                    {
                        int rowsAffected = cmdStock.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            lblErr.Text = "Stock not updated. ";
                        }
                    }
                    MyEnabled(false);
                }
                catch (Exception ex)
                {
                    lblErr.Text = "Error updating record: " + ex.Message;
                }
            }
        }
        lblStatus.Text = "Success";
        lblStatus.CssClass = "badge bg-success text-light fs-5 px-3 py-2 rounded-pill";

    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        MyEnabled(false);
        lblErr.Text = "";

        string strSQL = @"
        SELECT 
            bi.IssueID,
            bi.IssueDate,
            bi.ReturnDate,
            bi.BookID,
            bi.StudentID,
            s.StudentName,
            s.Branch,
            s.RegdNo,
            b.Title,
        FROM BookIssued bi
        INNER JOIN Student s ON bi.StudentID = s.StudentID
        INNER JOIN Books b ON bi.BookID = b.BookID
        WHERE bi.IssueID = @IssueID";

        MyShowSQLData(strSQL, "No data found.", new SqlParameter("@IssueID", ViewState["IssueID"]));
    }
    private void MyShowSQLData(string sql, string errorMsg, params SqlParameter[] parameters)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand(sql, con))
        {
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())

                {
                    // BookIssued
                    txtIssueId.Text = reader["IssueID"].ToString();
                    txtStuId.Text = reader["StudentID"].ToString();
                    txtBookId.Text = reader["BookID"].ToString();

                    if (reader["ReturnDate"] != DBNull.Value)
                    {
                        txtReturnDate.Text = Convert.ToDateTime(reader["ReturnDate"])
                            .ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        txtReturnDate.Text = "";
                    }

                    ViewState["IssueID"] = Convert.ToInt32(reader["IssueID"]);
                    ViewState["StudentID"] = Convert.ToInt32(reader["StudentID"]);
                    ViewState["BookID"] = Convert.ToInt32(reader["BookID"]);

                    // Student
                    txtName.Text = reader["StudentName"].ToString();
                    txtBranch.Text = reader["Branch"].ToString();
                    txtRegd.Text = reader["RegdNo"].ToString();

                    // Book
                    txtBook.Text = reader["Title"].ToString();

                    // ✅ Status
                    if (reader["IssueStatus"] != DBNull.Value)
                    {
                        bool issueStatus = Convert.ToBoolean(reader["IssueStatus"]);
                        if (issueStatus)
                        {
                            lblStatus.Text = "Success";
                            lblStatus.CssClass = "badge bg-success text-light fs-5 px-3 py-2 rounded-pill";
                        }
                        else
                        {
                            lblStatus.Text = "Pending";
                            lblStatus.CssClass = "badge bg-warning text-dark fs-5 px-3 py-2 rounded-pill";
                        }
                    }
                    else
                    {
                        lblStatus.Text = "Pending";
                        lblStatus.CssClass = "badge bg-warning text-dark fs-5 px-3 py-2 rounded-pill";
                    }

                }
                else
                {
                    lblErr.Text = errorMsg;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                lblErr.Text = "Error in Data: " + ex.Message;
            }
        }
    }


    protected void btnEdit_Click(object sender, EventArgs e)
    {
        MyEnabled(true);
        ViewState["OldBookID"] = txtBook.Text.Trim();

        this.SetFocus(txtReturnDate.ClientID);

    }
    protected void btnFirst_Click(object sender, EventArgs e)
    {
        string strSQL = @"
        SELECT TOP 1 
            bi.IssueID,
            bi.IssueDate,
            bi.ReturnDate,
            bi.IssueStatus,
            bi.BookID,
            bi.StudentID,
            s.StudentName,
            s.Branch,
            s.RegdNo,
            b.Title,
            b.ISBN
        FROM BookIssued bi
        INNER JOIN Student s ON bi.StudentID = s.StudentID
        INNER JOIN Books b ON bi.BookID = b.BookID
        ORDER BY bi.IssueID ASC";

        MyShowSQLData(strSQL, "No data found.");
    }

    protected void btnLast_Click(object sender, EventArgs e)
    {
        string strSQL = @"
        SELECT TOP 1 
            bi.IssueID,
            bi.IssueDate,
            bi.ReturnDate,
            bi.IssueStatus,
            bi.BookID,
            bi.StudentID,
            s.StudentName,
            s.Branch,
            s.RegdNo,
            b.Title,
            b.ISBN
        FROM BookIssued bi
        INNER JOIN Student s ON bi.StudentID = s.StudentID
        INNER JOIN Books b ON bi.BookID = b.BookID
        ORDER BY bi.IssueID DESC";

        MyShowSQLData(strSQL, "No data found.");
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        string strSQL = @"
        SELECT TOP 1 
            bi.IssueID,
            bi.IssueDate,
            bi.ReturnDate,
            bi.IssueStatus,
            bi.BookID,
            bi.StudentID,
            s.StudentName,
            s.Branch,
            s.RegdNo,
            b.Title,
            b.ISBN
        FROM BookIssued bi
        INNER JOIN Student s ON bi.StudentID = s.StudentID
        INNER JOIN Books b ON bi.BookID = b.BookID
        WHERE bi.IssueID > @IssueID
        ORDER BY bi.IssueID ASC";

        MyShowSQLData(strSQL, "Can't Move End Of Data.", new SqlParameter("@IssueID", ViewState["IssueID"]));
    }

    protected void btnPrev_Click(object sender, EventArgs e)
    {
        string strSQL = @"
        SELECT TOP 1 
            bi.IssueID,
            bi.IssueDate,
            bi.ReturnDate,
            bi.IssueStatus,
            bi.BookID,
            bi.StudentID,
            s.StudentName,
            s.Branch,
            s.RegdNo,
            b.Title,
            b.ISBN
        FROM BookIssued bi
        INNER JOIN Student s ON bi.StudentID = s.StudentID
        INNER JOIN Books b ON bi.BookID = b.BookID
        WHERE bi.IssueID < @IssueID
        ORDER BY bi.IssueID DESC";

        MyShowSQLData(strSQL, "Can't Move Begin Of Data.", new SqlParameter("@IssueID", ViewState["IssueID"]));
    }
    protected void btnDel_Click(object sender, EventArgs e)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "UPDATE BookIssued SET ReturnDate = NULL, IssueStatus = NULL WHERE IssueID = @IssueID;";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@IssueID", txtIssueId.Text.Trim());

            string strSQLStock = "UPDATE Books SET CurrentStock = CurrentStock - 1 WHERE BookID = @BookID ";
            SqlCommand cmdStock = new SqlCommand(strSQLStock, con);
            cmdStock.Parameters.AddWithValue("@BookID", txtBookId.Text.Trim());

            try
            {
                con.Open();
                int deleted = cmd.ExecuteNonQuery();
                lblErr.Text = deleted + " record(s) deleted successfully.";

                int rowsAffected = cmdStock.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    lblErr.Text = "Stock not updated. ";
                }

                btnNext_Click(sender, e);
                if (lblErr.Text == "Can't Move End Of Data.")
                {
                    lblErr.Text = "";
                    btnLast_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                lblErr.Text = "Error deleting record: " + ex.Message;
            }
        }

    }
    private void LoadBooks(string searchCondition = "")
    {
        if (searchCondition != "")
        {
            searchCondition = " WHERE " + searchCondition;
        }

        string conString = WebConfigurationManager.ConnectionStrings["Pubs"].ConnectionString;
        string sql = "SELECT IssueID, StudentID, BookID, IssueDate, ReturnDate, IssueStatus FROM BookIssued " + searchCondition;

        using (SqlConnection con1 = new SqlConnection(conString))
        using (SqlCommand cmd = new SqlCommand(sql, con1))
        using (SqlDataAdapter sda1 = new SqlDataAdapter(cmd))
        {
            DataTable dt1 = new DataTable();

            try
            {
                con1.Open();
                sda1.Fill(dt1);
            }
            catch (Exception ex)
            {
                lblErr.Text = "Error In Data: " + ex.Message;
                return;
            }

            sbTableData.Clear(); // clear previous data

            if (dt1.Rows.Count > 0)
            {
                foreach (DataRow row1 in dt1.Rows)
                {
                    string issueStatusHtml;

                    // ✅ Check IssueStatus and generate proper badge
                    if (row1["IssueStatus"] != DBNull.Value && Convert.ToBoolean(row1["IssueStatus"]) == true)
                    {
                        issueStatusHtml = "<span class='badge bg-success text-light px-2 py-1 rounded-pill'>Success</span>";
                    }
                    else
                    {
                        issueStatusHtml = "<span class='badge bg-warning text-dark px-2 py-1 rounded-pill'>Pending</span>";
                    }

                    sbTableData
                        .Append("<tr>")
                        .AppendFormat("<td>{0}</td>", row1["IssueID"])
                        .AppendFormat("<td>{0}</td>", row1["StudentID"])
                        .AppendFormat("<td>{0}</td>", row1["BookID"])
                        .AppendFormat("<td>{0:yyyy-MM-dd}</td>", row1["IssueDate"])
                        .AppendFormat("<td>{0}</td>", row1["ReturnDate"] == DBNull.Value ? "" : Convert.ToDateTime(row1["ReturnDate"]).ToString("yyyy-MM-dd"))
                        .AppendFormat("<td>{0}</td>", issueStatusHtml)
                        .Append("</tr>");
                }

                lblErr.Text = "";
            }
            else
            {
                lblErr.Text = "No records found";
            }
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        String strCondition = "";
        if (txtSearchBookID.Text.Trim().Length > 0)
        {
            strCondition += " (BookID LIKE '%" + txtSearchBookID.Text.Trim() + "%') ";
            string BookID = txtSearchBookID.Text.Trim();
            txtSearchBookID.Text = "";
            string strSQL = "SELECT TOP 1 * FROM BookIssued WHERE BookID = " + BookID;
            MyShowSQLData(strSQL, "Not Available ID " + BookID);
        }
        if (txtSearchStudentID.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (StudentID LIKE '%" + txtSearchStudentID.Text.Trim() + "%') ";
            string StudentID = txtSearchStudentID.Text.Trim();
            txtSearchStudentID.Text = "";
            string strSQL = "SELECT TOP 1 * FROM BookIssued WHERE StudentID = " + StudentID;
            MyShowSQLData(strSQL, "Not Available ID " + StudentID);
        }
        if (txtSearchIssueID.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (IssueID LIKE '%" + txtSearchIssueID.Text.Trim() + "%') ";
            string IssueID = txtSearchIssueID.Text.Trim();
            txtSearchIssueID.Text = "";
            string strSQL = "SELECT TOP 1 * FROM BookIssued WHERE IssueID = " + IssueID;
            MyShowSQLData(strSQL, "Not Available ID " + IssueID);
        }
        if (txtSearchIsDate.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (IssueDate LIKE '" + txtSearchIsDate.Text.Trim() + "%') ";
            string IssueDate = txtSearchIsDate.Text.Trim();
            txtSearchIsDate.Text = "";
            string strSQL = "SELECT TOP 1 * FROM BookIssued WHERE IssueDate = '" + IssueDate + "'";
            MyShowSQLData(strSQL, "Not Available Issue Date " + IssueDate);

        }
        if (txtSearcRehDate.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (ReturnDate LIKE '" + txtSearcRehDate.Text.Trim() + "%') ";
            string ReturnDate = txtSearcRehDate.Text.Trim();
            txtSearcRehDate.Text = "";
            string strSQL = "SELECT TOP 1 * FROM BookIssued WHERE ReturnDate = '" + ReturnDate + "'";
            MyShowSQLData(strSQL, "Not Available Return Date " + ReturnDate);

        }

        LoadBooks(strCondition);
    }

}


