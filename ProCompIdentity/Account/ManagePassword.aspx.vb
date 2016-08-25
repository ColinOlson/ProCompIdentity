Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports ProCompIdentity.Core.Identity

Partial Public Class ManagePassword
    Inherits System.Web.UI.Page
    Protected Property SuccessMessage As String

    Private Function HasPassword(manager As ProCompUserManager) As Boolean
        Return manager.HasPassword(User.Identity.GetUserId())
    End Function

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim manager = Context.GetOwinContext().GetUserManager(Of ProCompUserManager)()

        If Not IsPostBack Then
            ' Determine the sections to render
            If HasPassword(manager) Then
                changePasswordHolder.Visible = True
            Else
                setPassword.Visible = True
                changePasswordHolder.Visible = False
            End If

            ' Render success message
            Dim message = Request.QueryString("m")
            If message IsNot Nothing Then
                ' Strip the query string from action
                Form.Action = ResolveUrl("~/account/manage")
            End If
        End If
    End Sub

    Protected Sub ChangePassword_Click(sender As Object, e As EventArgs)
        If IsValid Then
            Dim manager = Context.GetOwinContext().GetUserManager(Of ProCompUserManager)()
            Dim signInManager = Context.GetOwinContext().Get(Of ProCompSignInManager)()
            Dim result As IdentityResult = manager.ChangePassword(User.Identity.GetUserId(), CurrentPassword.Text, NewPassword.Text)
            If result.Succeeded Then
                Dim userInfo = manager.FindById(User.Identity.GetUserId())
                signInManager.SignIn(userInfo, isPersistent:=False, rememberBrowser:=False)
                Response.Redirect("~/account/manage?m=ChangePwdSuccess")
            Else
                AddErrors(result)
            End If
        End If
    End Sub

    Protected Sub SetPassword_Click(sender As Object, e As EventArgs)
        If IsValid Then
            ' Create the local login info and link the local account to the user
            Dim manager = Context.GetOwinContext().GetUserManager(Of ProCompUserManager)()
            Dim result As IdentityResult = manager.AddPassword(User.Identity.GetUserId(), password.Text)
            If result.Succeeded Then
                Response.Redirect("~/account/manage?m=SetPwdSuccess")
            Else
                AddErrors(result)
            End If
        End If
    End Sub

    Private Sub AddErrors(result As IdentityResult)
        For Each [error] As String In result.Errors
            ModelState.AddModelError("", [error])
        Next
    End Sub
End Class