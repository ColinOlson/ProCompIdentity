<%@ Page Title="Manage Account" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Manage.aspx.vb" Inherits="ProCompIdentity.Manage" Async="true" %>

<%@ Import Namespace="System.Security.Claims" %>
<%@ Import Namespace="Microsoft.AspNet.Identity" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>

    <div>
        <asp:PlaceHolder runat="server" ID="SuccessMessagePlaceHolder" Visible="false" ViewStateMode="Disabled">
            <p class="text-success"><%: SuccessMessage %></p>
        </asp:PlaceHolder>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:LoginView runat="server">
                <RoleGroups>
                    <asp:RoleGroup Roles="admin">
                        <ContentTemplate>
                            <div class="alert alert-info">
                                This block of html is only visible to admins.
                            </div>
                        </ContentTemplate>
                    </asp:RoleGroup>
                </RoleGroups>
                <LoggedInTemplate>
                    <div class="alert alert-warning">
                        This block of html is visible to any logged in user who isn't an admin, or any role specifically handled by a RoleGroup (see markup)
                    </div>
                </LoggedInTemplate>
            </asp:LoginView>

            <div class="form-horizontal">
                <h4>Change your account settings</h4>
                <hr />
                <dl class="dl-horizontal">
                    <dt>Password:</dt>
                    <dd>
                        <asp:HyperLink NavigateUrl="/account/managepassword" Text="[Change]" Visible="false" ID="ChangePassword" runat="server" />
                        <asp:HyperLink NavigateUrl="/account/managepassword" Text="[Create]" Visible="false" ID="CreatePassword" runat="server" />
                    </dd>
                    <dt>Claims:</dt>
                    <dd>
                        <table class="table table-striped table-bordered table-condensed">
                            <tr>
                                <th>Claim</th>
                                <th>Value</th>
                                <th>Source</th>
                            </tr>
                            <%
                                Dim identity = CType(User.Identity, ClaimsIdentity)
                                Dim claims = identity.Claims
                                For Each claim In claims
                            %>
                            <tr>
                                <td><%: claim.Type %></td>
                                <td><%: claim.Value %></td>
                                <td style="white-space: nowrap;"><%: claim.Issuer %></td>
                            </tr>
                            <% Next %>
                        </table>
                        <div class="alert alert-info">
                            <code>data_store</code> is meant to be an example of a claim that associates
                            a user with the database where their data is stored. Claims can be used in
                            this way to associate important information about users with their identity.
                        </div>
                    </dd>
                    <dt>Is Admin:</dt>
                    <dd>
                        <%
                            Dim isAdmin = UserManager.IsInRole(identity.GetUserId(), "admin")
                        %>
                        <%: isAdmin  %>
                    </dd>

                    <% If isAdmin Then %>
                    <dt></dt>
                    <dd>
                        <asp:Button runat="server" ID="btnRemoveAdmin" Text="Remove Admin Role" OnClick="btnRemoveAdmin_OnClick" />
                    </dd>
                    <% Else %>
                    <dt></dt>
                    <dd>
                        <asp:Button runat="server" ID="btnBecomeAdmin" Text="Add Admin Role" OnClick="btnBecomeAdmin_OnClick" />
                    </dd>
                    <% End If %>
                </dl>
            </div>
        </div>
    </div>
</asp:Content>
