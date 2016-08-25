Imports System
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin.Security

Namespace Models

    ' You can add profile data for the user by adding more properties to your User class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

#Region "Helpers"
    Public Class IdentityHelper

        Private Shared Function IsLocalUrl(url As String) As Boolean
            Return Not String.IsNullOrEmpty(url) AndAlso ((url(0) = "/"c AndAlso (url.Length = 1 OrElse (url(1) <> "/"c AndAlso url(1) <> "\"c))) OrElse (url.Length > 1 AndAlso url(0) = "~"c AndAlso url(1) = "/"c))
        End Function

        Public Shared Sub RedirectToReturnUrl(returnUrl As String, response As HttpResponse)
            If Not [String].IsNullOrEmpty(returnUrl) AndAlso IsLocalUrl(returnUrl) Then
                response.Redirect(returnUrl)
            Else
                response.Redirect("~/")
            End If
        End Sub

    End Class

#End Region

End Namespace