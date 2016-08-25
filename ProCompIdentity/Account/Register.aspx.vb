Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports ProCompIdentity.Core.Identity
Imports ProCompIdentity.Models

Partial Public Class Register
    Inherits Page

    Protected Sub CreateUser_Click(sender As Object, e As EventArgs)

        Dim owinContext = Context.GetOwinContext()
        Dim membership = New ProCompMembershipService(owinContext)

        Dim result = membership.RegisterUser(Name.Text, Email.Text, Password.Text)

        If result.Success Then
            IdentityHelper.RedirectToReturnUrl(Request.QueryString("ReturnUrl"), Response)
        Else
            ErrorMessage.Text = result.Errors.FirstOrDefault()
        End If

    End Sub

End Class