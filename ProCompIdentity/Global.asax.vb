Imports System.Threading
Imports System.Web.Optimization

Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(sender As Object, e As EventArgs)
        ' Fires when the application is started
        RouteConfig.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles(BundleTable.Bundles)
    End Sub

    Sub Application_Error(sender As Object, e As EventArgs)
        Dim lastException = Server.GetLastError()

        ' We don't care if the thread was being aborted. It happens every time we redirect.
        If TypeOf (lastException) Is ThreadAbortException Then
            Server.ClearError()
        End If

    End Sub
End Class