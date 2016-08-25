Imports Newtonsoft.Json

Namespace Services.UserStore

    Public Class UserEntry

        <JsonProperty("email_address")>
        Public Property EmailAddress As String

        <JsonProperty("name")>
        Public Property Name As String

        <JsonProperty("password")>
        Public Property HashedPassword As String

        <JsonProperty("created")>
        Public Property DateCreated As DateTimeOffset

        <JsonProperty("roles")>
        Public Property Roles As List(Of String)

        <JsonProperty("data_store")>
        Public Property DataStore As String

        <JsonProperty("lockout_until")>
        Public Property LockoutEndDate As Nullable(Of DateTimeOffset)

        <JsonProperty("failed_attemps")>
        Public Property FailedLoginAttemps As Nullable(Of Integer)

    End Class

End Namespace