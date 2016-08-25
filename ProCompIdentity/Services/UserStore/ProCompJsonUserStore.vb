Imports System.IO
Imports System.Threading.Tasks
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin
Imports Newtonsoft.Json
Imports ProCompIdentity.Core.Identity

Namespace Services.UserStore
    Public Class ProCompJsonUserStore
        Implements IUserStore(Of ProCompUser),
            IUserPasswordStore(Of ProCompUser),
            IUserLockoutStore(Of ProCompUser, String),
            IUserEmailStore(Of ProCompUser),
            IUserTwoFactorStore(Of ProCompUser, String),
            IUserRoleStore(Of ProCompUser)

        Private ReadOnly _userDefinitionFile As String

        Public Shared Function Create(options As IdentityFactoryOptions(Of ProCompJsonUserStore), context As IOwinContext) As ProCompJsonUserStore
            Dim appDataPath = Hosting.HostingEnvironment.MapPath("~/App_Data")
            Dim userStoreFile = Path.Combine(appDataPath, "users.json")
            Dim userStore = New ProCompJsonUserStore(userStoreFile)
            Return userStore
        End Function

        Public Sub New(userDefinitionFile As String)
            _userDefinitionFile = userDefinitionFile
        End Sub

        Private Function ReadUsers() As List(Of UserEntry)
            ' If no user file exists yet, we have no users to return.
            If Not File.Exists(_userDefinitionFile) Then
                Return New List(Of UserEntry)
            End If

            Dim userData = File.ReadAllText(_userDefinitionFile)
            Dim userCollection = JsonConvert.DeserializeObject(Of List(Of UserEntry))(userData)

            Return userCollection
        End Function

        Private Sub WriteUsers(users As List(Of UserEntry))
            Dim userFileDirectory = Path.GetDirectoryName(_userDefinitionFile)
            If Not Directory.Exists(userFileDirectory) Then
                Directory.CreateDirectory(userFileDirectory)
            End If

            Dim userJson = JsonConvert.SerializeObject(users, Formatting.Indented)
            File.WriteAllText(_userDefinitionFile, userJson)
        End Sub

        Public Function CreateAsync(user As ProCompUser) As Task Implements IUserStore(Of ProCompUser, String).CreateAsync
            Dim entry = user.Entry

            entry.DateCreated = DateTime.Now

            ' Unless told otherwise, all new users are simply 'user'
            entry.Roles = New List(Of String) From {"user"}

            ' This is an example of a claim and can be used to assign users to particular databases.
            entry.DataStore = "Server=example.com;Database=PartitionedDatabase1;User Id=username; Password=password;"

            ' In a real application we would write the user to an actual database but for this
            ' example we just read and write a JSON file to demonstate that Identity framework really
            ' can be used with any application needs.
            Dim allUsers = ReadUsers()
            allUsers.Add(entry)
            WriteUsers(allUsers)

            Return Task.FromResult(0)
        End Function

        Public Function UpdateAsync(user As ProCompUser) As Task Implements IUserStore(Of ProCompUser, String).UpdateAsync
            Dim allUsers = ReadUsers()
            Dim currentUserIndex = allUsers.FindIndex(Function(u) u.EmailAddress.Equals(user.Id))

            If currentUserIndex = -1 Then
                Throw New ApplicationException(String.Format("Couldn't update user {0} because it couldn't be found in the passwords file.", user.Id))
            End If

            ' Replace the user entry entirely. In a real application this would be a database row update.
            allUsers(currentUserIndex) = user.Entry

            WriteUsers(allUsers)

            Return Task.FromResult(0)
        End Function

        Public Function DeleteAsync(user As ProCompUser) As Task Implements IUserStore(Of ProCompUser, String).DeleteAsync
            Throw New NotImplementedException
        End Function

        Public Function FindByIdAsync(userId As String) As Task(Of ProCompUser) Implements IUserStore(Of ProCompUser, String).FindByIdAsync
            Dim users = ReadUsers()
            Dim user = users.FirstOrDefault(Function(u) u.EmailAddress.Equals(userId))

            If user Is Nothing Then
                Return Task.FromResult(Of ProCompUser)(Nothing)
            Else
                Return Task.FromResult(New ProCompUser(user))
            End If
        End Function

        Public Function FindByNameAsync(userName As String) As Task(Of ProCompUser) Implements IUserStore(Of ProCompUser, String).FindByNameAsync
            ' We don't distinguish between ID and username for sign in purposes
            Return FindByIdAsync(userName)
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
        End Sub

        Public Function GetLockoutEndDateAsync(user As ProCompUser) As Task(Of DateTimeOffset) Implements IUserLockoutStore(Of ProCompUser, String).GetLockoutEndDateAsync
            Dim endDate = user.Entry.LockoutEndDate
            If endDate Is Nothing Then
                Return Task.FromResult(DateTimeOffset.MinValue)
            Else
                Return Task.FromResult(endDate.Value)
            End If
        End Function

        Public Function SetLockoutEndDateAsync(user As ProCompUser, lockoutEnd As DateTimeOffset) As Task Implements IUserLockoutStore(Of ProCompUser, String).SetLockoutEndDateAsync
            user.Entry.LockoutEndDate = lockoutEnd
            Return Task.FromResult(0)
        End Function

        Public Function IncrementAccessFailedCountAsync(user As ProCompUser) As Task(Of Integer) Implements IUserLockoutStore(Of ProCompUser, String).IncrementAccessFailedCountAsync
            Dim accessCount = If(user.Entry.FailedLoginAttemps.HasValue, user.Entry.FailedLoginAttemps.Value, 0)
            accessCount += 1
            user.Entry.FailedLoginAttemps = accessCount

            Return Task.FromResult(accessCount)
        End Function

        Public Function ResetAccessFailedCountAsync(user As ProCompUser) As Task Implements IUserLockoutStore(Of ProCompUser, String).ResetAccessFailedCountAsync
            user.Entry.FailedLoginAttemps = Nothing
            Return Task.FromResult(0)
        End Function

        Public Function GetAccessFailedCountAsync(user As ProCompUser) As Task(Of Integer) Implements IUserLockoutStore(Of ProCompUser, String).GetAccessFailedCountAsync
            Dim accessCount = If(user.Entry.FailedLoginAttemps.HasValue, user.Entry.FailedLoginAttemps.Value, 0)
            Return Task.FromResult(accessCount)
        End Function

        Public Async Function GetLockoutEnabledAsync(user As ProCompUser) As Task(Of Boolean) Implements IUserLockoutStore(Of ProCompUser, String).GetLockoutEnabledAsync
            ' All accounts can be locked out unless they are administrators.
            Dim isAdmin = Await IsInRoleAsync(user, "admin")
            Return Not isAdmin
        End Function

        Public Function SetLockoutEnabledAsync(user As ProCompUser, enabled As Boolean) As Task Implements IUserLockoutStore(Of ProCompUser, String).SetLockoutEnabledAsync
            ' Account lockout is determined by role so we ignore this.
            Return Task.FromResult(0)
        End Function

        Public Function SetPasswordHashAsync(user As ProCompUser, passwordHash As String) As Task Implements IUserPasswordStore(Of ProCompUser, String).SetPasswordHashAsync
            user.Entry.HashedPassword = passwordHash
            Return Task.FromResult(0)
        End Function

        Public Function GetPasswordHashAsync(user As ProCompUser) As Task(Of String) Implements IUserPasswordStore(Of ProCompUser, String).GetPasswordHashAsync
            Return Task.FromResult(user.Entry.HashedPassword)
        End Function

        Public Function HasPasswordAsync(user As ProCompUser) As Task(Of Boolean) Implements IUserPasswordStore(Of ProCompUser, String).HasPasswordAsync
            Return Task.FromResult(Not String.IsNullOrWhiteSpace(user.Entry.HashedPassword))
        End Function

        Public Function SetEmailAsync(user As ProCompUser, email As String) As Task Implements IUserEmailStore(Of ProCompUser, String).SetEmailAsync
            ' We don't allow the email to change.
            Return Task.FromResult(0)
        End Function

        Public Function GetEmailAsync(user As ProCompUser) As Task(Of String) Implements IUserEmailStore(Of ProCompUser, String).GetEmailAsync
            Return Task.FromResult(user.Id)
        End Function

        Public Function GetEmailConfirmedAsync(user As ProCompUser) As Task(Of Boolean) Implements IUserEmailStore(Of ProCompUser, String).GetEmailConfirmedAsync
            ' We don't verify and confirm user accounts.
            Return Task.FromResult(True)
        End Function

        Public Function SetEmailConfirmedAsync(user As ProCompUser, confirmed As Boolean) As Task Implements IUserEmailStore(Of ProCompUser, String).SetEmailConfirmedAsync
            ' We don't verify and confirm user accounts.
            Return Task.FromResult(0)
        End Function

        Public Function FindByEmailAsync(email As String) As Task(Of ProCompUser) Implements IUserEmailStore(Of ProCompUser, String).FindByEmailAsync
            ' We don't distinguish between user ID and email address.
            Return FindByIdAsync(email)
        End Function

        Public Function SetTwoFactorEnabledAsync(user As ProCompUser, enabled As Boolean) As Task Implements IUserTwoFactorStore(Of ProCompUser, String).SetTwoFactorEnabledAsync
            Return Task.FromResult(0)
        End Function

        Public Function GetTwoFactorEnabledAsync(user As ProCompUser) As Task(Of Boolean) Implements IUserTwoFactorStore(Of ProCompUser, String).GetTwoFactorEnabledAsync
            Return Task.FromResult(False)
        End Function

        Public Function AddToRoleAsync(user As ProCompUser, roleName As String) As Task Implements IUserRoleStore(Of ProCompUser, String).AddToRoleAsync
            user.Entry.Roles.Add(roleName)
            Return Task.FromResult(0)
        End Function

        Public Function RemoveFromRoleAsync(user As ProCompUser, roleName As String) As Task Implements IUserRoleStore(Of ProCompUser, String).RemoveFromRoleAsync
            user.Entry.Roles.Remove(roleName)
            Return Task.FromResult(0)
        End Function

        Public Function GetRolesAsync(user As ProCompUser) As Task(Of IList(Of String)) Implements IUserRoleStore(Of ProCompUser, String).GetRolesAsync
            Dim roles As IList(Of String) = user.Entry.Roles
            Return Task.FromResult(roles)
        End Function

        Public Function IsInRoleAsync(user As ProCompUser, roleName As String) As Task(Of Boolean) Implements IUserRoleStore(Of ProCompUser, String).IsInRoleAsync
            Dim roles = user.Entry.Roles
            If roles Is Nothing OrElse roles.Count = 0 Then
                Return Task.FromResult(False)
            Else
                Return Task.FromResult(roles.Any(Function(r) r.Equals(roleName, StringComparison.InvariantCultureIgnoreCase)))
            End If
        End Function
    End Class

End Namespace