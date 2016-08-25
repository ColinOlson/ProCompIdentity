Namespace Core.Identity
    Public Class RegisterResult
        Public Property Success As Boolean

        Public Property Errors As IEnumerable(Of String)

        Public Sub New(success As Boolean)
            Me.Success = success
        End Sub

        Public Sub New(success As Boolean, errors As IEnumerable(Of String))
            Me.Success = success
            Me.Errors = errors
        End Sub
    End Class
End NameSpace