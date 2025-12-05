using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Runtime.Remoting.Messaging;

public partial class BookIssue : System.Web.UI.Page
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
            txtIssueDate.Attributes["max"] = DateTime.Now.ToString("yyyy-MM-dd");
            ViewState["IsAdd"] = false;
            ViewState["IsEdit"] = false;
       
            // Show Last Record
            btnLast_Click(sender, e);
            LoadBooks();
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        txtIssueID.Text = "";
        txtBookID.Text = "";
        txtStudentId.Text = "";
        txtName.Text = "";
        txtRegd.Text = "";
        txtBookName.Text = "";
        txtBranch.Text = "";
        txtISBN.Text = "";
        txtIssueDate.Text = "";

        MyEnabled(true);

        ViewState["IsAdd"] = true;
        this.SetFocus(txtStudentId.ClientID);

    }
    private void MyEnabled(bool plEnabled)
    {
        txtStudentId.Enabled=plEnabled;
        txtBookID.Enabled=plEnabled;
        txtIssueDate.Enabled = plEnabled;
        btnSave.Enabled = plEnabled;
        btnCancel.Enabled = plEnabled;

        btnFirst.Enabled = !plEnabled;
        btnLast.Enabled = !plEnabled;
        btnPrev.Enabled = !plEnabled;
        btnNext.Enabled = !plEnabled;
        btnEdit.Enabled = !plEnabled;
        btnDel.Enabled = !plEnabled;
        btnNew.Enabled = !plEnabled;
        txtSearchStudentID.Enabled = !plEnabled;
        txtSearchIssueID.Enabled = !plEnabled;
        txtSearchBookID.Enabled = !plEnabled;
        txtSearchDate.Enabled = !plEnabled;
        btnSearch.Enabled = !plEnabled;
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if ((bool)ViewState["IsAdd"] == true)
        {
            if (txtStudentId.Text == "" || txtBookID.Text == "" || txtIssueDate.Text == "")
            {
                lblErr.Text = "Student ID or Book ID or Issue date is missing.";
                return;

            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                //Book Id section
                SqlCommand cmdFindBook = new SqlCommand("SELECT Title, ISBN WHERE (BookID = @BookID) ", con);
                cmdFindBook.Parameters.AddWithValue("@BookID", txtBookID.Text.Trim());

                SqlCommand cmdFindStudent = new SqlCommand("SELECT StudentName, Branch, RegdNo FROM Student WHERE (StudentID = @StudentID)", con);
                cmdFindStudent.Parameters.AddWithValue("@StudentID", txtStudentId.Text.Trim());

                

                string strSQLNewID = "SELECT ISNULL((SELECT TOP 1 IssueID FROM BookIssued ORDER BY IssueID DESC), 0) AS IssueID ";
                SqlCommand cmdNewID = new SqlCommand(strSQLNewID, con);
                int NewIssueID = 1;

                string strSQLStock = "UPDATE Books SET CurrentStock = CurrentStock - 1 WHERE BookID = @BookID AND CurrentStock > 0";
                SqlCommand cmdStock = new SqlCommand(strSQLStock, con);
                cmdStock.Parameters.AddWithValue("@BookID", txtBookID.Text.Trim());

                string strSQLSave = @"INSERT INTO BookIssued (IssueID,StudentID, BookID, IssueDate) 
                         VALUES (@IssueID, @StudentID, @BookID, @IssueDate )";
                SqlCommand cmdSave = new SqlCommand(strSQLSave, con);
                cmdSave.Parameters.AddWithValue("@StudentID", txtStudentId.Text.Trim());
                cmdSave.Parameters.AddWithValue("@BookID", txtBookID.Text.Trim());
                cmdSave.Parameters.AddWithValue("@IssueDate", txtIssueDate.Text.Trim());
                bool isValidData = true;
                SqlDataReader reader1;
                try
                {
                    con.Open();

                    reader1 = cmdFindBook.ExecuteReader();
                    if (reader1.Read())
                    {
                        txtBookName.Text = reader1["Title"].ToString();
                        txtISBN.Text = reader1["ISBN"].ToString();
                    }
                    else
                    {
                        isValidData = false;
                        lblErr.Text = "Invalid Book ID";
                    }
                    if (isValidData)
                    {
                        reader1 = cmdFindStudent.ExecuteReader();
                        if (reader1.Read())
                        {
                            txtName.Text = reader1["StudentName"].ToString();
                            txtRegd.Text = reader1["RegdNo"].ToString();
                            txtBranch.Text = reader1["Branch"].ToString();
                        }
                        else
                        {
                            isValidData = false;
                            lblErr.Text = "Invalid Student ID";
                        }
                    }
                    if (isValidData)
                    {
                        int rowsAffected = cmdStock.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            isValidData = false;
                            lblErr.Text = "Stock not updated. The book might be out of stock.";
                        }

                    }
                    if (isValidData)
                    {
                        NewIssueID = (int)cmdNewID.ExecuteScalar();
                        NewIssueID += 1;

                        cmdSave.Parameters.AddWithValue("@IssueID", NewIssueID);

                        int added = cmdSave.ExecuteNonQuery();
                        lblErr.Text = added + " record saved successfully.";

                        txtIssueID.Text = NewIssueID.ToString();

                        ViewState["IsAdd"] = false;
                        MyEnabled(false);
                    }
                }
                catch (Exception ex)
                {
                    lblErr.Text = "Error inserting record: " + ex.Message;
                }
            }
        }

        if ((bool)ViewState["IsEdit"] == true)
        {
            if ((bool)ViewState["IsEdit"] == true)
            {
                string oldBookID = ViewState["OldBookID"]?.ToString();   // You should set this when loading the record for editing                                                                                                                                           
                string newBookID = txtBookID.Text.Trim();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string strSQL = @"UPDATE BookIssued 
                          SET StudentID = @StudentID, BookID = @BookID, IssueDate = @IssueDate 
                          WHERE IssueID = @IssueID";

                    using (SqlCommand cmd = new SqlCommand(strSQL, con))
                    {
                        cmd.Parameters.AddWithValue("@IssueID", txtIssueID.Text.Trim());
                        cmd.Parameters.AddWithValue("@StudentID", txtStudentId.Text.Trim());
                        cmd.Parameters.AddWithValue("@BookID", newBookID);
                        cmd.Parameters.AddWithValue("@IssueDate", txtIssueDate.Text.Trim());

                        SqlCommand cmdStockDecrease = null;
                        SqlCommand cmdStockIncrease = null;

                        // Check if BookID changed
                        if (!string.Equals(oldBookID, newBookID, StringComparison.OrdinalIgnoreCase))
                        {
                            // Decrease stock for new book
                            string strSQLStockDecrease = @"UPDATE Books 
                                               SET CurrentStock = CurrentStock - 1 
                                               WHERE BookID = @BookID AND CurrentStock > 0";
                            cmdStockDecrease = new SqlCommand(strSQLStockDecrease, con);
                            cmdStockDecrease.Parameters.AddWithValue("@BookID", newBookID);

                            // Increase stock for new book
                            string strSQLStockIncrease = @"UPDATE Books 
                                               SET CurrentStock = CurrentStock + 1 
                                               WHERE BookID = @BookID ";
                            cmdStockIncrease = new SqlCommand(strSQLStockIncrease, con);
                            cmdStockIncrease.Parameters.AddWithValue("@BookID", oldBookID);


                        }

                        try
                        {
                            con.Open();

                            int updated = cmd.ExecuteNonQuery();
                            lblErr.Text = updated + " record updated successfully.";

                            // Run stock commands if needed
                            if (cmdStockDecrease != null)
                            {
                                int rowsAffected = cmdStockDecrease.ExecuteNonQuery();
                                if (rowsAffected == 0)
                                {
                                    lblErr.Text += " Stock not updated. The book might be out of stock.";
                                }

                                if (cmdStockIncrease != null)
                                {
                                    cmdStockIncrease.ExecuteNonQuery();  // return stock to old book
                                }
                            }

                            ViewState["IsEdit"] = false;
                            MyEnabled(false);
                        }
                        catch (Exception ex)
                        {
                            lblErr.Text = "Error updating record: " + ex.Message;
                        }
                    }
                }
            }

        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ViewState["IsEdit"] = false;
        ViewState["IsAddt"] = false;
        MyEnabled(false);
        lblErr.Text = "";

        string strSQL = @"
        SELECT 
            bi.IssueID,
            bi.IssueDate,
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
        WHERE bi.IssueID = @IssueID";

        MyShowSQLData(strSQL, "No data found.", new SqlParameter("@IssueID", ViewState["IssueID"]));
    }

    protected void btnFirst_Click(object sender, EventArgs e)
    {
        string strSQL = @"
        SELECT TOP 1 
            bi.IssueID,
            bi.IssueDate,
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
                    txtIssueID.Text = reader["IssueID"].ToString();
                    txtStudentId.Text = reader["StudentID"].ToString();
                    txtBookID.Text = reader["BookID"].ToString();

                    if (reader["IssueDate"] != DBNull.Value)
                    {
                        txtIssueDate.Text = Convert.ToDateTime(reader["IssueDate"])
                            .ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        txtIssueDate.Text = "";
                    }

                    ViewState["IssueID"] = Convert.ToInt32(reader["IssueID"]);
                    ViewState["StudentID"] = Convert.ToInt32(reader["StudentID"]);
                    ViewState["BookID"] = Convert.ToInt32(reader["BookID"]);

                    // Student
                    txtName.Text = reader["StudentName"].ToString();
                    txtBranch.Text = reader["Branch"].ToString();
                    txtRegd.Text = reader["RegdNo"].ToString();

                    // Book
                    txtBookName.Text = reader["Title"].ToString();
                    txtISBN.Text = reader["ISBN"].ToString();
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
        ViewState["IsEdit"] = true;
        MyEnabled(true);
        ViewState["OldBookID"] = txtBookID.Text.Trim();

        this.SetFocus(txtStudentId.ClientID);

    }

    protected void btnDel_Click(object sender, EventArgs e)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string strSQLStockIncrease = "UPDATE Books SET CurrentStock = CurrentStock + 1  WHERE BookID = @BookID ";
            SqlCommand cmdStockIncrease = new SqlCommand(strSQLStockIncrease, con);
            cmdStockIncrease.Parameters.AddWithValue("@BookID", txtBookID.Text.Trim());

            string query = "DELETE FROM BookIssued WHERE IssueID = @IssueID";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@IssueID", txtIssueID.Text.Trim());


            try
            {
                con.Open();

                int deleted = cmd.ExecuteNonQuery();
                lblErr.Text = deleted + " record(s) deleted successfully.";
                cmdStockIncrease.ExecuteNonQuery();  // return stock to old book

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
        if (searchCondition != "") { searchCondition = " WHERE " + searchCondition; }
        string conString = WebConfigurationManager.ConnectionStrings["Pubs"].ConnectionString;
        string sql = "SELECT IssueID, StudentID, BookID, IssueDate FROM BookIssued " + searchCondition;
        SqlConnection con1 = new SqlConnection(conString);
        SqlCommand cmd = new SqlCommand(sql, con1);
        SqlDataAdapter sda1 = new SqlDataAdapter(cmd);
        DataTable dt1 = new DataTable();
        using (con1)
        {
            try
            {
                con1.Open();
                sda1.Fill(dt1);

            }
            catch (Exception ex)
            {
                lblErr.Text = "Error In Data: " + ex.Message;
            }
        }
        if (dt1.Rows.Count > 0)
        {
            foreach (DataRow row1 in dt1.Rows)
            {
                sbTableData.Append("<tr>").AppendFormat(
                    "<td>{0}</td>", row1["IssueID"]).AppendFormat(
                    "<td>{0}</td>", row1["StudentID"]).AppendFormat(
                    "<td>{0}</td>", row1["BookID"]).AppendFormat(
                    "<td>{0}</td>", row1["IssueDate"]).AppendFormat(
                    "</tr>");
            }
            // lblErr.Text = "";
        }
        else
        {
            lblErr.Text = "No records found";
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
            string strSQL = "SELECT TOP 1 * FROM BookIssued WHERE StudentID = " + StudentID ;
            MyShowSQLData(strSQL, "Not Available ID " + StudentID);
        }
        if (txtSearchIssueID.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (IssueID LIKE '%" + txtSearchIssueID.Text.Trim() + "%') ";
            string IssueID = txtSearchIssueID.Text.Trim();
            txtSearchIssueID.Text = "";
            string strSQL = "SELECT TOP 1 * FROM BookIssued WHERE IssueID = " + IssueID ;
            MyShowSQLData(strSQL, "Not Available ID " + IssueID);
        }
        if (txtSearchDate.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (IssueDate LIKE '" + txtSearchDate.Text.Trim() + "%') ";
            string IssueDate = txtSearchDate.Text.Trim();
            txtSearchDate.Text = "";
            string strSQL = "SELECT TOP 1 * FROM BookIssued WHERE IssueDate = '" + IssueDate + "'";
            MyShowSQLData(strSQL, "Not Available Issue Date " + IssueDate);

        }
        LoadBooks(strCondition);
    }

}
