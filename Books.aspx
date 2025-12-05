<%@ Page Title="" Language="C#" MasterPageFile="~/Login1.master" AutoEventWireup="true" CodeFile="Books.aspx.cs" Inherits="Books" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head2" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="formbody" Runat="Server">
<!-- ✅ Page Container -->
<div class="container py-4">

    <!-- 🏷️ Page Header -->
    <div class="d-flex justify-content-between align-items-center mb-5 flex-wrap gap-2">
        <h1 class="fw-bold page-title m-0">📝 Manage Books</h1>
        <a href="Default.aspx" class="btn btn-danger btn-lg px-4 rounded-pill shadow hover-lift">🚪 Logout</a>
    </div>

    <!-- 🔍 Search Section -->
    <div class="row mb-5 g-3 justify-content-center search-section py-3 px-2 rounded-4 shadow-sm">
                <!-- Book ID -->
        <div class="col-lg-2 col-md-4 col-sm-6">
            <asp:TextBox ID="txtSearchID" runat="server" 
                CssClass="form-control border-success fw-bold text-success search-box" 
                placeholder="Book ID"></asp:TextBox>
        </div>

        <!-- Book Name -->
        <div class="col-lg-2 col-md-4 col-sm-6">
            <asp:TextBox ID="txtSearchTitle" runat="server" 
                CssClass="form-control border-success fw-bold text-success search-box" 
                placeholder="Book Title"></asp:TextBox>
        </div>

        <!-- Issue Date -->
        <div class="col-lg-2 col-md-4 col-sm-6">
            <asp:TextBox ID="txtSearchAuthor" runat="server" 
                CssClass="form-control border-success fw-bold text-success search-box" 
                placeholder="Author Name."></asp:TextBox>
        </div>

        <!-- Issue Date -->
        <div class="col-lg-2 col-md-4 col-sm-6">
            <asp:TextBox ID="txtSearchISBN" runat="server" 
                CssClass="form-control border-success fw-bold text-success search-box" 
                placeholder="ISBN"></asp:TextBox>                                               
        </div>

        <!-- Publication -->
        <div class="col-lg-2 col-md-4 col-sm-6">
            <asp:TextBox ID="txtSearchPub" runat="server" 
                CssClass="form-control border-success fw-bold text-success search-box" 
                placeholder="Publication"></asp:TextBox>
        </div>
        <!-- Subject -->
        <div class="col-lg-2 col-md-4 col-sm-6">
            <asp:TextBox ID="txtSearchSubject" runat="server" 
                CssClass="form-control border-success fw-bold text-success search-box" 
                placeholder="Subject"></asp:TextBox>
        </div>

        <!-- Search Button -->
        <div class="col-lg-2 col-md-4 col-sm-6">
            <asp:Button ID="btnSearch" runat="server" Text="🔍 Search" 
                CssClass="btn btn-outline-success w-100 fw-bold search-btn rounded-pill" OnClick="btnSearch_Click" />
        </div>
    </div>

    <!-- 📚 Book Issue Info -->
    <div class="card border-0 shadow-lg mb-5 rounded-4 card-animate">
        <div class="card-header gradient-green text-white fw-bold fs-5 rounded-top-4">
            📚 Books Information
        </div>
        <div class="card-body">
            <div class="row g-4">
                <div class="col-md-4">
                    <label for="txtBookId" class="form-label fw-semibold">Book ID</label>
                    <asp:TextBox ID="txtBookId" runat="server" CssClass="form-control shadow-sm input-focus" Enabled="False"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label for="txtTitle" class="form-label fw-semibold">Title</label>
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control shadow-sm input-focus" Enabled="False"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label for="txtAuthor" class="form-label fw-semibold">Author</label>
                    <asp:TextBox ID="txtAuthor" runat="server" CssClass="form-control shadow-sm input-focus" Enabled="False"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label for="txtISBN" class="form-label fw-semibold">ISBN No.</label>
                    <asp:TextBox ID="txtISBN" runat="server" CssClass="form-control shadow-sm input-focus" Enabled="False"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label for="txtPublication" class="form-label fw-semibold">Publication</label>
                    <asp:TextBox ID="txtPublication" runat="server" CssClass="form-control shadow-sm input-focus" Enabled="False"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label for="txtSubject" class="form-label fw-semibold">Subject</label>
                    <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control shadow-sm input-focus" Enabled="False"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label for="txtOrgStock" class="form-label fw-semibold">Stock</label>
                    <asp:TextBox ID="txtOrgStock" runat="server" CssClass="form-control shadow-sm input-focus" Enabled="False"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label for="txtCurStock" class="form-label fw-semibold">Current Stock</label>
                    <asp:TextBox ID="txtCurStock" runat="server" CssClass="form-control shadow-sm input-focus" Enabled="False"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>

    <!-- 🛠️ Action Buttons -->
    <div class="card border-0 shadow-lg mb-4 rounded-4 card-animate">
        <div class="card-header gradient-green text-white fw-bold fs-5 rounded-top-4">
            🛠️ Actions
        </div>
        <div class="card-body text-center">
            <div class="d-flex flex-wrap gap-2 justify-content-center">
                <asp:Button ID="btnFirst" runat="server" Text="⏮ First" CssClass="btn btn-outline-success fw-bold action-btn rounded-pill" OnClick="btnFirst_Click" />
                <asp:Button ID="btnPrev" runat="server" Text="⬅ Prev" CssClass="btn btn-outline-success fw-bold action-btn rounded-pill" OnClick="btnPrev_Click" />
                <asp:Button ID="btnNew" runat="server" Text="➕ New" CssClass="btn btn-success fw-bold action-btn rounded-pill" OnClick="btnNew_Click" />
                <asp:Button ID="btnEdit" runat="server" Text="✏ Edit" CssClass="btn btn-warning fw-bold text-dark action-btn rounded-pill" OnClick="btnEdit_Click" />
                <asp:Button ID="btnSave" runat="server" Text="💾 Save" CssClass="btn btn-primary fw-bold action-btn rounded-pill" Enabled="False" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="❌ Cancel" CssClass="btn btn-secondary fw-bold action-btn rounded-pill" Enabled="False" OnClick="btnCancel_Click" />
                <asp:Button ID="btnDel" runat="server" Text="🗑 Delete" CssClass="btn btn-danger fw-bold action-btn rounded-pill" 
                    OnClientClick="return confirm('Are You Sure To Delete?')" OnClick="btnDel_Click" />
                <asp:Button ID="btnNext" runat="server" Text="Next ➡" CssClass="btn btn-outline-success fw-bold action-btn rounded-pill" OnClick="btnNext_Click" />
                <asp:Button ID="btnLast" runat="server" Text="Last ⏭" CssClass="btn btn-outline-success fw-bold action-btn rounded-pill" OnClick="btnLast_Click" />
            </div>
        </div>
    </div>
        <!-- Table Section -->
     <div class="table-responsive">
         <table class='table table-bordered table-hover table-striped text-center'>
             <thead class='table-success text-uppercase fw-bold text-dark'>
                 <tr>
                     <th>Book ID</th>
                     <th>Book Title</th>
                     <th>Author</th>
                     <th>ISBN NO.</th>
                     <th>Publication</th>
                     <th>Subject</th>
                     <th>Stock</th>
                     <th>Available Stock</th>
                 </tr>
             </thead>
             <tbody>
                 <%=sbTableData.ToString() %>
             </tbody>
         </table>
     </div>

    <!-- ⚠️ Error Label -->
    <asp:Label ID="lblErr" runat="server"
        CssClass="alert alert-danger d-block fw-bold text-center fs-5 rounded-3 shadow-sm fade-in"
        ViewStateMode="Disabled"></asp:Label>

</div>

<!-- 🌿 Optional Extra Styling -->
<style>
    body {
        background: linear-gradient(to bottom right, #ffffff, #f9f9f9);
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        min-height: 100vh;
    }

    .page-title {
        font-size: 2.5rem;
        color: #1c3d36;
        font-weight: 800;
        letter-spacing: 1px;
        text-shadow: 1px 1px 2px rgba(0,0,0,0.1);
    }

    .gradient-green {
        background: linear-gradient(to right, #1c3d36, #2e865f);
    }

    .form-label {
        color: #1c3d36;
        font-weight: 600;
    }

    /* ✨ Inputs */
    .input-focus:focus {
        box-shadow: 0 0 10px rgba(25, 135, 84, 0.4);
        border-color: #198754;
        transform: scale(1.02);
        transition: all 0.3s ease;
    }

    /* 🔍 Search */
    .search-section {
        background: #ffffff;
    }
    .search-box:focus {
        box-shadow: 0 0 8px rgba(25, 135, 84, 0.5);
        border-color: #198754;
    }
    .search-btn {
        transition: all 0.3s ease;
    }
    .search-btn:hover {
        background-color: #198754;
        color: white;
        transform: scale(1.05);
        box-shadow: 0 4px 8px rgba(25, 135, 84, 0.3);
    }

    /* 🛠️ Buttons */
    .action-btn {
        transition: all 0.3s ease;
        padding: 10px 20px;
    }
    .action-btn:hover {
        transform: translateY(-3px);
        box-shadow: 0 5px 12px rgba(0,0,0,0.1);
    }

    .hover-lift:hover {
        transform: translateY(-3px);
        box-shadow: 0 5px 15px rgba(0,0,0,0.2);
    }

    /* 🪄 Card Animation */
    .card-animate {
        transition: all 0.3s ease;
    }
    .card-animate:hover {
        transform: translateY(-5px);
        box-shadow: 0 8px 20px rgba(0,0,0,0.15);
    }

    /* 🌟 Fade In Effect */
    .fade-in {
        animation: fadeIn 0.6s ease-in-out;
    }
    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(-10px); }
        to { opacity: 1; transform: translateY(0); }
    }
</style>

</asp:Content>

