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
using System.Xml.Linq;

public partial class Books : System.Web.UI.Page
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
            txtBookId.Text = "";
            txtAuthor.Text = "";
            txtCurStock.Text = "";
            txtISBN.Text = "";
            txtOrgStock.Text = "";
            txtPublication.Text = "";
            txtSubject.Text = "";
            txtTitle.Text = "";

            MyEnabled(true);

            ViewState["IsAdd"] = true;
            this.SetFocus(txtTitle.ClientID);
        }
        private void MyEnabled(bool plEnabled)
        {
            txtAuthor.Enabled = plEnabled;
            txtCurStock.Enabled = plEnabled;
            txtISBN.Enabled = plEnabled;
            txtOrgStock.Enabled = plEnabled;
            txtPublication.Enabled = plEnabled;
            txtSubject.Enabled = plEnabled;
            txtTitle.Enabled = plEnabled;
            btnSave.Enabled = plEnabled;
            btnCancel.Enabled = plEnabled;

            btnFirst.Enabled = !plEnabled;
            btnLast.Enabled = !plEnabled;
            btnPrev.Enabled = !plEnabled;
            btnNext.Enabled = !plEnabled;
            btnEdit.Enabled = !plEnabled;
            btnDel.Enabled = !plEnabled;
            btnNew.Enabled = !plEnabled;
            txtSearchAuthor.Enabled = !plEnabled;
            txtSearchISBN.Enabled = !plEnabled;
            txtSearchPub.Enabled = !plEnabled;
            txtSearchTitle.Enabled = !plEnabled;
        txtSearchID.Enabled = !plEnabled;
        txtSearchSubject.Enabled = !plEnabled;
            btnSearch.Enabled = !plEnabled;
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            txtTitle.Text = txtTitle.Text.Trim();
            if ((bool)ViewState["IsAdd"] == true)
            {
                if (txtTitle.Text == "" || txtOrgStock.Text == "" )
                {
                    lblErr.Text = "Book title or stock number is missing.";
                    return;

                }
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string strSQLDupTitle = "SELECT BookID FROM Books WHERE (Title = @Title)";
                    SqlCommand cmdDupTitle = new SqlCommand(strSQLDupTitle, con);
                cmdDupTitle.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());

                    string strSQLNewID = "SELECT ISNULL((SELECT TOP 1 BookID FROM Books ORDER BY BookID DESC), 0) AS BookID ";
                    SqlCommand cmdNewID = new SqlCommand(strSQLNewID, con);
                    int NewBookID = 1;
                    string strSQL = @"INSERT INTO Books (BookID,Title,Author,ISBN, Publication,Subject,OriginalStock, CurrentStock) 
                         VALUES (@BookID, @Title, @Author, @ISBN, @Publication, @Subject, @OriginalStock, @CurrentStock)";
                    SqlCommand cmd = new SqlCommand(strSQL, con);
                // cmd.Parameters.AddWithValue("@BookID", txtBookId.Text.Trim());
                cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@Author", txtAuthor.Text.Trim());
                cmd.Parameters.AddWithValue("@ISBN", txtISBN.Text.Trim());
                cmd.Parameters.AddWithValue("@Publication", txtPublication.Text.Trim());
                cmd.Parameters.AddWithValue("@Subject", txtSubject.Text.Trim());
                cmd.Parameters.AddWithValue("@OriginalStock", txtOrgStock.Text.Trim());
                cmd.Parameters.AddWithValue("@CurrentStock", txtCurStock.Text.Trim());

                try
                {
                    con.Open();

                        var obj1 = cmdDupTitle.ExecuteScalar();
                        if ((obj1 != DBNull.Value) && (obj1 != null))
                        {
                    lblErr.Text = "Can't Save, This Title, Already Save In Book ID: " + obj1.ToString();
                        }
                        else
                        {
                        NewBookID = (int)cmdNewID.ExecuteScalar();
                        NewBookID += 1;

                            cmd.Parameters.AddWithValue("@BookID", NewBookID);

                            int added = cmd.ExecuteNonQuery();
                            lblErr.Text = added + " record saved successfully.";

                            txtBookId.Text = NewBookID.ToString();

                            ViewState["IsAdd"] = false;
                            MyEnabled(false);
                        }
                }
                catch (Exception ex)
                {
                    lblErr.Text = "Error inserting record: " + ex.Message /*+ "<br />" + strSQL*/;
                }
            }
            }

            if ((bool)ViewState["IsEdit"] == true)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Books SET Title = @Title, Author = @Author, ISBN = @ISBN, Publication = @Publication, Subject = @Subject, OriginalStock = @OriginalStock, CurrentStock = @CurrentStock WHERE BookID = @BookID";
                    SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@BookID", txtBookId.Text.Trim());
                cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@Author", txtAuthor.Text.Trim());
                cmd.Parameters.AddWithValue("@ISBN", txtISBN.Text.Trim());
                cmd.Parameters.AddWithValue("@Publication", txtPublication.Text.Trim());
                cmd.Parameters.AddWithValue("@Subject", txtSubject.Text.Trim());
                cmd.Parameters.AddWithValue("@OriginalStock", txtOrgStock.Text.Trim());
                cmd.Parameters.AddWithValue("@CurrentStock", txtCurStock.Text.Trim());

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

            string strSQL = "SELECT * FROM Books WHERE BookID = " + ViewState["BookID"];
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
                        txtBookId.Text = reader["BookID"].ToString();
                    txtTitle.Text = reader["Title"].ToString();
                        txtAuthor.Text = reader["Author"].ToString();
                    txtISBN.Text = reader["ISBN"].ToString();
                        txtPublication.Text = reader["Publication"].ToString();
                        txtSubject.Text = reader["Subject"].ToString();
                        txtOrgStock.Text = reader["OriginalStock"].ToString();
                        txtCurStock.Text = reader["CurrentStock"].ToString();


                    ViewState["BookID"] = int.Parse(txtBookId.Text);
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
            string strSQL = "SELECT TOP 1 * FROM Books ORDER BY BookID DESC ";
            MyShowSQLData(strSQL, "");
        }

        protected void btnFirst_Click(object sender, EventArgs e)
        {
            string strSQL = "SELECT TOP 1 * FROM Books ORDER BY BookID ";
            MyShowSQLData(strSQL, "");
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            string strSQL = "SELECT TOP 1 * FROM Books WHERE BookID > " + ViewState["BookID"] + " ORDER BY BookID";
            MyShowSQLData(strSQL, "Can't Move End Of Data.");
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            string strSQL = "SELECT TOP 1 * FROM Books WHERE BookID < " + ViewState["BookID"] + " ORDER BY BookID DESC ";
            MyShowSQLData(strSQL, "Can't Move Begin Of Data.");
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            ViewState["IsEdit"] = true;
            MyEnabled(true);

            this.SetFocus(txtTitle.ClientID);
        }

        protected void btnDel_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Books WHERE BookID = @BookID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@BookID", txtBookId.Text.Trim());

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
            string sql = "SELECT TOP 20 * FROM Books " + searchCondition;
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
                        "<td>{0}</td>", row1["BookID"]).AppendFormat(
                        "<td>{0}</td>", row1["Title"]).AppendFormat(
                        "<td>{0}</td>", row1["Author"]).AppendFormat(
                        "<td>{0}</td>", row1["ISBN"]).AppendFormat(
                        "<td>{0}</td>", row1["Publication"]).AppendFormat(
                        "<td>{0}</td>", row1["Subject"]).AppendFormat(
                        "<td>{0}</td>", row1["OriginalStock"]).AppendFormat(
                        "<td>{0}</td>", row1["CurrentStock"]).AppendFormat(
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
            if (txtSearchTitle.Text.Trim().Length > 0)
            {
                strCondition += " (Title LIKE '%" + txtSearchTitle.Text.Trim() + "%') ";
                string Title = txtSearchTitle.Text.Trim();
            txtSearchTitle.Text = "";
                string strSQL = "SELECT TOP 1 * FROM Books WHERE Title ='" + Title + "'";
                MyShowSQLData(strSQL, "Not Available Title " + Title);
            }
            if (txtSearchAuthor.Text.Trim().Length > 0)
            {
                if (strCondition != "") { strCondition += " AND "; }
                strCondition += " (Author LIKE '" + txtSearchAuthor.Text.Trim() + "%') ";
                string Author = txtSearchAuthor.Text.Trim();
            txtSearchAuthor.Text = "";
                string strSQL = "SELECT TOP 1 * FROM Books WHERE Author = '" + Author + "'";
                MyShowSQLData(strSQL, "Not Available Author  " + Author);
            }
            if (txtSearchPub.Text.Trim().Length > 0)
            {
                if (strCondition != "") { strCondition += " AND "; }
                strCondition += " (Publication LIKE '" + txtSearchPub.Text.Trim() + "%') ";
                string Publication = txtSearchPub.Text.Trim();
            txtSearchPub.Text = "";
                string strSQL = "SELECT TOP 1 * FROM Books WHERE Publication = '" + Publication + "'";
                MyShowSQLData(strSQL, "Not Available Publication " + Publication);

            }
            if (txtSearchISBN.Text.Trim().Length > 0)
            {
                if (strCondition != "") { strCondition += " AND "; }
                strCondition += " (ISBN LIKE '" + txtSearchISBN.Text.Trim() + "%') ";
                string ISBN = txtSearchISBN.Text.Trim();
            txtSearchISBN.Text = "";
                string strSQL = "SELECT TOP 1 * FROM Books WHERE ISBN = '" + ISBN + "'";
                MyShowSQLData(strSQL, "Not Available ISBN No. " + ISBN);

            }
        if (txtSearchID.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (BookID =" + txtSearchID.Text.Trim() + ") ";
            string BookID = txtSearchID.Text.Trim();
            txtSearchID.Text = "";
            string strSQL = "SELECT TOP 1 * FROM Books WHERE BookID = " + BookID ;
            MyShowSQLData(strSQL, "Not Available Book ID  " + BookID);

        }
        if (txtSearchSubject.Text.Trim().Length > 0)
        {
            if (strCondition != "") { strCondition += " AND "; }
            strCondition += " (Subject LIKE '" + txtSearchSubject.Text.Trim() + "%') ";
            string Subject = txtSearchISBN.Text.Trim();
            txtSearchSubject.Text = "";
            string strSQL = "SELECT TOP 1 * FROM Books WHERE Subject = '" + Subject + "'";
            MyShowSQLData(strSQL, "Not Available Subject. " + Subject);

        }

        LoadBooks(strCondition);
        }

    }