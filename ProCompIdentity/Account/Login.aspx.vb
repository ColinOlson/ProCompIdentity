Imports Microsoft.AspNet.Identity.Owin
Imports ProCompIdentity.Core.Identity
Imports ProCompIdentity.Models

Partial Public Class Login
    Inherits Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        RegisterHyperLink.NavigateUrl = "register"

        Dim returnUrl = HttpUtility.UrlEncode(Request.QueryString("ReturnUrl"))
        If Not String.IsNullOrEmpty(returnUrl) Then
            RegisterHyperLink.NavigateUrl += "?returnUrl=" & returnUrl
        End If

    End Sub

    Protected Async Sub LogIn(sender As Object, e As EventArgs)
        If IsValid Then
            ' Validate the user password

            Dim owinContext = Context.GetOwinContext()
            Dim membership = New ProCompMembershipService(owinContext)

            Dim result = Await membership.SignIn(Email.Text, Password.Text, RememberMe.Checked)

            Select Case result
                Case SignInStatus.Success
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString("ReturnUrl"), Response)
                    Exit Select

                Case SignInStatus.LockedOut
                    Response.Redirect("/account/lockout")
                    Exit Select

                Case Else
                    FailureText.Text = "Invalid login attempt"
                    ErrorMessage.Visible = True
                    Exit Select

            End Select
        End If
    End Sub
End Class