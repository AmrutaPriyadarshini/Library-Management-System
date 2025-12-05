<%@ Page Title="" Language="C#" MasterPageFile="~/Main1.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
    <style>
    /* HERO SECTION */
    .hero {
        color: white;
        padding: 60px 0;           /* original padding */
        text-align: center;
        background-image: url('images/Hero.jpg'); /* path to your image */
    background-size: cover;   /* make image cover entire section */
    background-position: center; /* center the image */
    background-repeat: no-repeat; /* prevent repeating */
    display: flex;
    align-items: center; /* vertically center content */
    justify-content: center; /* horizontally center content */
    text-align: center;
        width: 100%;               /* full width */
    }

    .hero h1 {
        font-size: 2.1rem;
        font-style: italic;
        max-width: 800px;
        margin: 0 auto;
        line-height: 1.4;
        font-weight: 700;
    }

    /* Responsive adjustments */
    @media (max-width: 768px) {
        .hero h1 {
            font-size: 1.8rem;
        }
    }
</style>

<!-- HERO SECTION -->
<section class="hero">
    <div class="container">
        <h1>“Connecting people to knowledge,<br />one book at a time.”</h1>
    </div>
</section>


    <div class="container my-4">

    <!-- Search Bar Section -->
    <div class="row mb-4 justify-content-center">
    <!-- Book ID -->
    <div class="col-lg-2 col-md-4 col-sm-6 mb-2">
        <asp:TextBox ID="txtSearchID" runat="server" CssClass="form-control border-success fw-bold text-success" 
            placeholder="Book ID" TextMode="Search"></asp:TextBox>
    </div>

    <!-- Book Title -->
    <div class="col-lg-2 col-md-4 col-sm-6 mb-2">
        <asp:TextBox ID="txtSearchTitle" runat="server" CssClass="form-control border-success fw-bold text-success" 
            placeholder="Book Title" TextMode="Search"></asp:TextBox>
    </div>

    <!-- Author Name -->
    <div class="col-lg-2 col-md-4 col-sm-6 mb-2">
        <asp:TextBox ID="txtSearchAuthor" runat="server" CssClass="form-control border-success fw-bold text-success" 
            placeholder="Author Name" TextMode="Search"></asp:TextBox>
    </div>

    <!-- ISBN -->
    <div class="col-lg-2 col-md-4 col-sm-6 mb-2">
        <asp:TextBox ID="txtSearchISBN" runat="server" CssClass="form-control border-success fw-bold text-success" 
            placeholder="ISBN" TextMode="Search"></asp:TextBox>
    </div>

    <!-- Publication -->
    <div class="col-lg-2 col-md-4 col-sm-6 mb-2">
        <asp:TextBox ID="txtSearchPublication" runat="server" CssClass="form-control border-success fw-bold text-success" 
            placeholder="Publication" TextMode="Search"></asp:TextBox>
    </div>
    <!-- Subject -->
    <div class="col-lg-2 col-md-4 col-sm-6 mb-2">
        <asp:TextBox ID="txtSearchSub" runat="server" CssClass="form-control border-success fw-bold text-success" 
            placeholder="Subject" TextMode="Search"></asp:TextBox>
    </div>

    <!-- Search Button -->
    <div class="col-lg-2 col-md-4 col-sm-6 mb-2">
        <asp:Button ID="btnSearch" runat="server" Text="Search" 
            CssClass="btn btn-outline-success w-100 fw-bold" OnClick="btnSearch_Click"  />
    </div>
</div>

    <!-- Table Section -->
     <div class="table-responsive">
         <table class='table table-bordered table-hover table-striped text-center'>
             <thead class='table-success text-uppercase fw-bold text-dark'>
                 <tr>
                     <th>Book ID</th>
                     <th>Book Title</th>
                     <th>Author Name</th>
                     <th>ISBN No.</th>
                     <th>Publication</th>
                     <th>Subject</th>
                     <th>Available Stock</th>
                 </tr>
             </thead>
             <tbody>
                 <%=sbTableData.ToString() %>
             </tbody>
         </table>
     </div>


    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger fw-bold"></asp:Label>
</div>
</asp:Content>

