<%@ Page Title="Admin Only" Language="vb" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminOnly.aspx.vb" Inherits="ProCompIdentity.AdminOnly" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>

    <div>
        This page is only visible to users in the admin role.
    </div>
</asp:Content>
