Imports System.Security.Claims
Imports System.Threading.Tasks
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin
Imports Microsoft.Owin.Security

Namespace Core.Identity

    ' Configure the application user manager used in this application. UserManager is defined in
    ' ASP.NET Identity and is used by the application.

    Public Class ProCompSignInManager
        Inherits SignInManager(Of ProCompUser, String)
        Public Sub New(userManager As ProCompUserManager, authenticationManager As IAuthenticationManager)
            MyBase.New(userManager, authenticationManager)
        End Sub

        Public Overrides Function CreateUserIdentityAsync(user As ProCompUser) As Task(Of ClaimsIdentity)
            Return user.GenerateUserIdentityAsync(DirectCast(UserManager, ProCompUserManager))
        End Function

        Public Shared Function Create(options As IdentityFactoryOptions(Of ProCompSignInManager), context As IOwinContext) As ProCompSignInManager
            Return New ProCompSignInManager(context.GetUserManager(Of ProCompUserManager)(), context.Authentication)
        End Function
    End Class

End Namespace