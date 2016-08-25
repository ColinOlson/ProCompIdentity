Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin
Imports ProCompIdentity.Services.UserStore

Namespace Core.Identity

    Public Class ProCompUserManager
        Inherits UserManager(Of ProCompUser)
        Public Sub New(store As IUserStore(Of ProCompUser))
            MyBase.New(store)
        End Sub

        Public Shared Function Create(options As IdentityFactoryOptions(Of ProCompUserManager), context As IOwinContext) As ProCompUserManager
            Dim userStore = context.Get(Of ProCompJsonUserStore)
            Dim manager = New ProCompUserManager(userStore)

            ' Configure validation logic for usernames
            manager.UserValidator = New UserValidator(Of ProCompUser)(manager) With {
                .AllowOnlyAlphanumericUserNames = False,
                .RequireUniqueEmail = True
            }

            ' Configure validation logic for passwords
            manager.PasswordValidator = New PasswordValidator() With {
                .RequiredLength = 6,
                .RequireNonLetterOrDigit = True,
                .RequireDigit = True,
                .RequireLowercase = True,
                .RequireUppercase = True
            }

            ' Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = True
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5)
            manager.MaxFailedAccessAttemptsBeforeLockout = 5

            manager.PasswordHasher = New ProCompPasswordHasher()

            Dim dataProtectionProvider = options.DataProtectionProvider
            If dataProtectionProvider IsNot Nothing Then
                manager.UserTokenProvider = New DataProtectorTokenProvider(Of ProCompUser)(dataProtectionProvider.Create("ProComp Identity Example"))
            End If

            Return manager
        End Function
    End Class
End Namespace