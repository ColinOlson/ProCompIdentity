Imports System.Threading.Tasks
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin

Namespace Core.Identity

    Public Class ProCompMembershipService
        Private ReadOnly _context As IOwinContext
        Private ReadOnly _signInManager As ProCompSignInManager
        Private ReadOnly _userManager As ProCompUserManager

        Public Sub New(context As IOwinContext)
            _context = context
            _userManager = context.GetUserManager(Of ProCompUserManager)
            _signInManager = context.Get(Of ProCompSignInManager)
        End Sub

        Public Async Function SignIn(emailAddress As String, password As String, rememberUser As Boolean) As Task(Of SignInStatus)
            Dim result = Await _signInManager.PasswordSignInAsync(emailAddress, password, rememberUser, shouldLockout:=True)
            Return result
        End Function

        Public Sub SignOut()
            _context.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie)
        End Sub

        Public Async Function ReSignIn(userId As String) as Task
            SignOut()

            Dim user = Await _userManager.FindByIdAsync(userId)
            Await _signInManager.SignInAsync(user, True, False)
        End Function

        Public Async Function GetUser() As Task(Of ProCompUser)
            Dim userPrincipal = _context.Authentication.User
            If userPrincipal Is Nothing Then
                Return Nothing
            End If

            Dim id = userPrincipal.Identity.GetUserId()
            Dim user = Await _userManager.FindByIdAsync(id)
            Return user
        End Function

        Public Function RegisterUser(name As String, emailAddress As String, password As String) As RegisterResult
            Dim signInUser = New ProCompUser(name, emailAddress, "")
            Dim result = _userManager.Create(signInUser, password)

            If result.Succeeded Then
                ' For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                ' Dim code = manager.GenerateEmailConfirmationToken(user.Id)
                ' Dim callbackUrl = IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id, Request)
                ' manager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=""" & callbackUrl & """>here</a>.")

                _signInManager.SignIn(signInUser, isPersistent:=False, rememberBrowser:=False)
                Return New RegisterResult(True)
            Else
                Return New RegisterResult(False, result.Errors)
            End If
        End Function

    End Class
End Namespace