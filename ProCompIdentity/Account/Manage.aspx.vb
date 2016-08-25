Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports ProCompIdentity.Core.Identity

Partial Public Class Manage
    Inherits System.Web.UI.Page
    Protected Property SuccessMessage As String

    Public Property UserManager As ProCompUserManager

    Private Function HasPassword(userManager As ProCompUserManager) As Boolean
        Dim appUser = userManager.FindById(User.Identity.GetUserId())
        Return (appUser IsNot Nothing AndAlso appUser.Entry.HashedPassword IsNot Nothing)
    End Function

    Public Property LoginsCount As Integer

    Protected Sub Page_Load() Handles Me.Load
        Dim manager = Context.GetOwinContext().GetUserManager(Of ProCompUserManager)()
        UserManager = manager

        If Not IsPostBack Then
            ' Determine the sections to render
            If HasPassword(manager) Then
                ChangePassword.Visible = True
            Else
                CreatePassword.Visible = True
                ChangePassword.Visible = False
            End If

            ' Render success message
            Dim message = Request.QueryString("m")
            If message IsNot Nothing Then
                ' Strip the query string from action
                Form.Action = ResolveUrl("~/account/manage")

                SuccessMessage = If(message = "ChangePwdSuccess", "Your password has been changed.",
                    If(message = "SetPwdSuccess", "Your password has been set.", String.Empty))
                SuccessMessagePlaceHolder.Visible = Not String.IsNullOrEmpty(SuccessMessage)
            End If
        End If
    End Sub

    Protected Sub btnRemoveAdmin_OnClick(sender As Object, e As EventArgs)
        Dim userId = User.Identity.GetUserId()
        UserManager.RemoveFromRole(userId, "admin")

        Dim owinContext = Context.GetOwinContext()
        Dim membership = New ProCompMembershipService(owinContext)
        Dim task = membership.ReSignIn(userId)
        task.Wait()

        ' We need to redirect so that the user's cookies have a chance to update and reflect the
        ' change to their account.
        Response.Redirect(ResolveUrl("~/account/manage"))
    End Sub

    Protected Sub btnBecomeAdmin_OnClick(sender As Object, e As EventArgs)
        Dim userId = User.Identity.GetUserId()
        UserManager.AddToRole(userId, "admin")

        Dim owinContext = Context.GetOwinContext()
        Dim membership = New ProCompMembershipService(owinContext)
        Dim task = membership.ReSignIn(userId)
        task.Wait()

        ' We need to redirect so that the user's cookies have a chance to update and reflect the
        ' change to their account.
        Response.Redirect(ResolveUrl("~/account/manage"))
    End Sub
End Class