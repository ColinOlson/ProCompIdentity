Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin
Imports Microsoft.Owin.Security.Cookies
Imports Owin
Imports ProCompIdentity.Core.Identity
Imports ProCompIdentity.Services.UserStore

Partial Public Class Startup

    ' For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301883
    Public Sub ConfigureAuth(app As IAppBuilder)

        'Configure the user manager and signin manager to use a single instance per request
        app.CreatePerOwinContext(Of ProCompJsonUserStore)(AddressOf ProCompJsonUserStore.Create)
        app.CreatePerOwinContext(Of ProCompUserManager)(AddressOf ProCompUserManager.Create)
        app.CreatePerOwinContext(Of ProCompSignInManager)(AddressOf ProCompSignInManager.Create)

        ' Enable the application to use a cookie to store information for the signed in user
        app.UseCookieAuthentication(New CookieAuthenticationOptions() With {
            .AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            .Provider = New CookieAuthenticationProvider() With {
                .OnValidateIdentity = SecurityStampValidator.OnValidateIdentity(Of ProCompUserManager, ProCompUser)(
                    validateInterval:=TimeSpan.FromMinutes(30),
                    regenerateIdentity:=Function(manager, user) user.GenerateUserIdentityAsync(manager))},
            .LoginPath = New PathString("/account/login")
        })
    End Sub

End Class