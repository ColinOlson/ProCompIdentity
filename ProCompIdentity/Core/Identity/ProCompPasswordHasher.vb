Imports System.Security.Cryptography
Imports Microsoft.AspNet.Identity

Namespace Core.Identity

    Public Class ProCompPasswordHasher
        Implements IPasswordHasher

        Public Function HashPassword(password As String) As String Implements IPasswordHasher.HashPassword
            Return ComputeHash(password)
        End Function

        Private Function ComputeHash(password As String) As String
            Dim hasher = SHA1.Create()
            Dim bytes = Encoding.ASCII.GetBytes(password)
            Dim hashBytes = hasher.ComputeHash(bytes)
            Dim hashString = New StringBuilder()

            For Each b In hashBytes
                hashString.AppendFormat("{0:X2}", b)
            Next

            Return hashString.ToString()
        End Function

        Public Function VerifyHashedPassword(hashedPassword As String, providedPassword As String) As PasswordVerificationResult Implements IPasswordHasher.VerifyHashedPassword
            Dim hashOfProvidedPassword = ComputeHash(providedPassword)

            Return If(hashedPassword.Equals(hashOfProvidedPassword), PasswordVerificationResult.Success, PasswordVerificationResult.Failed)
        End Function

    End Class

End Namespace