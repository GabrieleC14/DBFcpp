Imports System.Data.OleDb
Imports System.Globalization
Imports System.IO
Imports System.Diagnostics

Public Class CDXCreator


    Public Sub EseguiFoxPro(scriptPath As String, foxProExePath As String)

        Dim proc As New Process()
        proc.StartInfo.FileName = foxProExePath
        proc.StartInfo.Arguments = scriptPath
        proc.StartInfo.UseShellExecute = False
        proc.StartInfo.CreateNoWindow = True
        proc.Start()
        proc.WaitForExit()
        proc.Dispose()

    End Sub

End Class
