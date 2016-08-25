Imports System.Security.Claims
Imports System.Threading.Tasks
Imports Microsoft.AspNet.Identity
Imports ProCompIdentity.Services.UserStore

Namespace Core.Identity
    ''' <summary>
    ''' To implement IUser we only need to provide the Id and UserName properties. This class wraps
    ''' UserEntry, which is the domain model that our storage system uses, and acts as a bridge
    ''' between the Identity framework and our custom user format and repository.
    ''' </summary>
    Public Class ProCompUser
        Implements IUser(Of String)

        Public ReadOnly Property Id As String Implements IUser(Of String).Id
            Get
                Return Entry.EmailAddress
            End Get
        End Property

        Public Property UserName As String Implements IUser(Of String).UserName
            Get
                Return Entry.Name
            End Get
            Set(value As String)
                Entry.Name = value
            End Set
        End Property

        Public Property Entry As UserEntry

        Public Sub New(name As String, emailAddress As String, hashedPassword As String)
            Entry = New UserEntry() With {
                .EmailAddress = emailAddress,
                .Name = name,
                .HashedPassword = hashedPassword
            }
        End Sub

        Public Sub New(userEntry As UserEntry)
            Entry = userEntry
        End Sub

        Public Async Function GenerateUserIdentityAsync(userManager As ProCompUserManager) As Task(Of ClaimsIdentity)
            ' Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            Dim userIdentity = Await userManager.CreateIdentityAsync(Me, DefaultAuthenticationTypes.ApplicationCookie)

            ' Add custom user claims here
            Dim dataStoreClaim = New Claim("http://procomp.com/example/claims/data_store", Entry.DataStore, GetType(String).Name, "ProComp")
            userIdentity.AddClaim(dataStoreClaim)

            ' Return populated identity
            Return userIdentity
        End Function
    End Class
End Namespace