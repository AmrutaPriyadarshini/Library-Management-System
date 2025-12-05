<%@ Page Title="" Language="C#" MasterPageFile="~/Main1.master" AutoEventWireup="true" CodeFile="LibraryLogin.aspx.cs" Inherits="LibraryLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
    <div class="container d-flex justify-content-center align-items-center" 
     style="min-height: 80vh; background-image: url('images/LoginBack.jpg');">
    <div class="card shadow-lg p-4" 
         style="width: 400px; border-radius: 20px; border: none; background: #ffffffee;">
        
        <!-- Title -->
        <h1 class="text-center mb-4" 
            style="color: #1b5e20; font-weight: bold; text-shadow: 1px 1px #c8e6c9;">
            Login
        </h1>

        <!-- ID -->
        <div class="mb-3">
            <label for="txtId" class="form-label" style="color:#2e7d32; font-weight:600;">
                👤 ID
            </label>
            <asp:TextBox ID="txtId" runat="server" CssClass="form-control border-success" 
                         placeholder="Enter your ID"></asp:TextBox>
        </div>

        <!-- Password -->
        <div class="mb-3">
            <label for="txtPwd" class="form-label" style="color:#2e7d32; font-weight:600;">
                🔒 Password
            </label>
            <asp:TextBox ID="txtPwd" runat="server" TextMode="Password" CssClass="form-control border-success" 
                         placeholder="Enter your password"></asp:TextBox>
        </div>

        <!-- Login Button -->
        <div class="d-grid mb-3">
            <asp:Button ID="btnLogin" runat="server" Text="Login" 
                        CssClass="btn" 
                        Style="background: linear-gradient(45deg,#a5d6a7,#81c784);
                               border:none; font-weight:bold; color:#1b5e20; 
                               border-radius:30px; padding:10px 0; 
                               transition:0.3s;" OnClick="btnLogin_Click"  />
        </div>

        <!-- Error Label -->
        <asp:Label ID="lblerror" runat="server" Text=""
                   CssClass="d-block text-center fw-bold mt-2"
                   Style="color:#d32f2f;"></asp:Label>
    </div>
</div>

</asp:Content>

