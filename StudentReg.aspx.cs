using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class StudentReg : System.Web.UI.Page
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
            ViewState["IsAdd"] = false;
            ViewState["IsEdit"] = false;
            // Show Last Record
            btnLast_Click(sender, e);
            LoadBooks();
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        txtName.Text = "";
        txtRegd.Text = "";
        txtMobile.Text = "";
        txtBranch.Text = "";
        txtStudentId.Text = "";

        MyEnabled(true);

        ViewState["IsAdd"] = true;
        this.SetFocus(txtName.ClientID);
    }
    private void MyEnabled(bool plEnabled)
    {
        txtName.Enabled = plEnabled;
        txtRegd.Enabled = plEnabled;
        txtBranch.Enabled = plEnabled;
        txtMobile.Enabled= plEnabled;
        btnSave.Enabled = plEnabled;
        btnCancel.Enabled = plEnabled;

        btnFirst.Enabled = !plEnabled;
        btnLast.Enabled = !plEnabled;
        btnPrev.Enabled = !plEnabled;
        btnNext.Enabled = !plEnabled;
        btnEdit.Enabled = !plEnabled;
        btnDel.Enabled = !plEnabled;
        btnNew.Enabled = !plEnabled;
        txtSearchBranch.Enabled = !plEnabled;
        txtSearchRegd.Enabled = !plEnabled;
        txtSearchID.Enabled = !plEnabled;
        txtSearchName.Enabled = !plEnabled;
        btnSearch.Enabled = !plEnabled;
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        txtRegd.Text = txtRegd.Text.Trim();
        if ((bool)ViewState["IsAdd"] == true)
        {
            if (txtName.Text == "" || txtRegd.Text == "" || txtBranch.Text=="")
            {
                lblErr.Text = "Student name or Regd No. or Branch is missing.";
                return;

            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string strSQLDupRegdNo = "SELECT StudentID FROM Student WHERE (RegdNo = @RegdNo)";
                SqlCommand cmdDupRegdNo = new SqlCommand(strSQLDupRegdNo, con);
                cmdDupRegdNo.Parameters.AddWithValue("@RegdNo", txtRegd.Text.Trim());

                string strSQLNewID = "SELECT ISNULL((SELECT TOP 1 StudentID FROM Student ORDER BY StudentID DESC), 0) AS StudentID ";
                SqlCommand cmdNewID = new SqlCommand(strSQLNewID, con);
                int NewStudentID = 1;
                string strSQL = @"INSERT INTO Student (StudentID,StudentName,MobileNo,RegdNo,Branch) 
                         VALUES (@StudentID, @StudentName, @MobileNo, @RegdNo, @Branch)";
                SqlCommand cmd = new SqlCommand(strSQL, con);
                cmd.Parameters.AddWithValue("@StudentName", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@MobileNo", txtMobile.Text.Trim());
                cmd.Parameters.AddWithValue("@RegdNo", txtRegd.Text.Trim());
                cmd.Parameters.AddWithValue("@Branch", txtBranch.Text.Trim());

                try
                {
                    con.Open();

                    var obj1 = cmdDupRegdNo.ExecuteScalar();
                    if (obj1 != DBNull.Value)
                    {
                        lblErr.Text = "Can't Save, This Registration No, Already Save In Student ID: " + obj1.ToString();
                    } else
                    {
                        NewStudentID = (int)cmdNewID.ExecuteScalar();
                        NewStudentID += 1;

                        cmd.Parameters.AddWithValue("@StudentID", NewStudentID);

                        int added = cmd.ExecuteNonQuery();
                        lblErr.Text = added + " record saved successfully.";

                        txtStudentId.Text = NewStudentID.ToString();

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
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Student SET StudentName = @StudentName, MobileNo = @MobileNo, RegdNo = @RegdNo, Branch = @Branch WHERE StudentID = @StudentID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StudentID", txtStudentId.Text.Trim());
                cmd.Parameters.AddWithValue("@StudentName", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@MobileNo", txtMobile.Text.Trim());
                cmd.Parameters.AddWithValue("@RegdNo", txtRegd.Text.Trim());
                cmd.Parameters.AddWithValue("@Branch", txtBranch.Text.Trim());

                try
                {
                    con.Open();
                    int added = cmd.ExecuteNonQuery();
                    lblErr.Text = added + " record updated successfully.";
                    ViewState["IsEdit"] = false;
                    MyEnabled(false);

                }
                catch (Exception ex)
                {
                    lblErr.Text = "Error updateing record: " + ex.Message;
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

        string strSQL = "SELECT * FROM Student WHERE StudentID = " + ViewState["StudentID"];
        MyShowSQLData(strSQL, "");

    }
    private void MyShowSQLData(string pcSQL, string pcShowErrMsg)
    {
        // sql command to show SkinProblems Data
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(pcSQL, con);
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtStudentId.Text = reader["StudentID"].ToString();
                    txtName.Text = reader["StudentName"].ToString();
                    txtMobile.Text = reader["MobileNo"].ToString();
                    txtRegd.Text = reader["RegdNo"].ToString();
                    txtBranch.Text = reader["Branch"].ToString();

                    ViewState["StudentID"] = int.Parse(txtStudentId.Text);
                }
                else
                {
                    lblErr.Text = pcShowErrMsg;
                }
                reader.Close();
            }
            catch (Exception err)
            {
                lblErr.Text = "Error In Data: " + err.Message;// + "<br />" + pcSQL;
            }
            finally
            {
                con.Close();
            }
        }
    }

    protected void btnLast_Click(object sender, EventArgs e)
    {
        string strSQL = "SELECT TOP 1 * FROM Student ORDER BY StudentID DESC ";
        MyShowSQLData(strSQL, "");
    }

    protected void btnFirst_Click(object sender, EventArgs e)
    {
        string strSQL = "SELECT TOP 1 * FROM Student ORDER BY StudentID ";
        MyShowSQLData(strSQL, "");
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        string strSQL = "SELECT TOP 1 * FROM Student WHERE StudentID > " + ViewState["StudentID"] + " ORDER BY StudentID";
        MyShowSQLData(strSQL, "Can't Move End Of Data.");
    }

    protected void btnPrev_Click(object sender, EventArgs e)
    {
        string strSQL = "SELECT TOP 1 * FROM Student WHERE StudentID < " + ViewState["StudentID"] + " ORDER BY StudentID DESC ";
        MyShowSQLData(strSQL, "Can't Move Begin Of Data.");
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        ViewState["IsEdit"] = true;
        MyEnabled(true);

        this.SetFocus(txtName.ClientID);
    }

    protected void btnDel_Click(object sender, EventArgs e)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM Student WHERE StudentID = @StudentID";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@StudentID", txtStudentId.Text.Trim());

            try
            {
                con.Open();
                int deleted = cmd.ExecuteNonQuery();
                lblErr.Text = deleted + " record(s) deleted successfully.";
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
        string sql = "SELECT * FROM Student " + searchCondition;
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
                    "<td>{0}</td>", row1["StudentID"]).AppendFormat(
                    "<td>{0}</td>", row1["StudentName"]).AppendFormat(
                    "<td>{0}</td>", row1["RegdNo"]).AppendFormat(
                    "<td>{0}</td>", row1["Branch"]).AppendFormat(
                    "<td>{0}</td>", row1["MobileNo"]).AppendFormat(
                    "</tr>");
            }
            // lblErr.Text = "";
        }
        else
        {
            lblErr.Text = "No records found";
        }
    }


    protected void btnSearch_Click1(object sender, EventArgs e)
    {
        String strCondition = "";
        if (txtSearchID.Text.Trim().Length > 0)
        {
            strCondition += " (StudentID LIKE '%" + txtSearchID.Text.Trim() + "%') ";
            string StudentID = txtSearchID.Text.Trim();
            txtSearchID.Text = "";
            string strSQL = "SELECT TOP 1 * FROM Student WHERE StudentID = " + StudentID;
            MyShowSQLData(strSQL, "Not Available ID " + StudentID);
        }
        if (txtSearchRegd.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (RegdNo LIKE '" + txtSearchRegd.Text.Trim() + "%') ";
            string RegdNo = txtSearchRegd.Text.Trim();
            txtSearchRegd.Text = "";
            string strSQL = "SELECT TOP 1 * FROM Student WHERE RegdNo = '" + RegdNo + "'";
            MyShowSQLData(strSQL, "Not Available Regd No " + RegdNo);
        }
        if (txtSearchBranch.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (Branch LIKE '" + txtSearchBranch.Text.Trim() + "%') ";
            string Branch = txtSearchBranch.Text.Trim();
            txtSearchBranch.Text = "";
            string strSQL = "SELECT TOP 1 * FROM Student WHERE Branch = '" + Branch +"'";
            MyShowSQLData(strSQL, "Not Available Branch " + Branch);

        }
        if (txtSearchName.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (StudentName LIKE '" + txtSearchName.Text.Trim() + "%') ";
            string StudentName = txtSearchName.Text.Trim();
            txtSearchName.Text = "";
            string strSQL = "SELECT TOP 1 * FROM Student WHERE StudentName = '" + StudentName +"'";
            MyShowSQLData(strSQL, "Not Available Student Name " + StudentName);

        }
        LoadBooks(strCondition);
    }

}