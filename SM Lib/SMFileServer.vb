Imports System
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Web
Public Class SMFileServer
    Implements IDisposable
    Public rootPath As String
    Private Const bufferSize As Integer = 1024 * 1024
    Private Shared webArchiver As SMArchiveReader
    Private ReadOnly http As HttpListener
    Public Sub New(ByVal rootPath As String, Optional ByVal Prefixes As String = "http://localhost:8080/")
        If rootPath = Nothing Then rootPath = Directory.GetCurrentDirectory() & "\cache"
        Me.rootPath = rootPath
        http = New HttpListener()

        Try
            http.Prefixes.Add(Prefixes)
            http.Start()
            http.BeginGetContext(AddressOf requestWait, Nothing)
        Catch ex As Exception
            Logger.getInstance().write(ex.ToString())
        End Try



    End Sub
    Public Sub Dispose() Implements System.IDisposable.Dispose
        http.[Stop]()
    End Sub


    Private Sub requestWait(ByVal ar As IAsyncResult)
        If Not http.IsListening Then
            Return
        End If
        Dim c = http.EndGetContext(ar)
        http.BeginGetContext(AddressOf requestWait, Nothing)

        Dim url = tuneUrl(c.Request.Url.LocalPath)


        If url = "" Then url = "index.html"

        Dim fullPath = IIf(String.IsNullOrEmpty(url), rootPath, Path.Combine(rootPath, url))
        'Logger.getInstance().write("request: " & c.Request.Url.LocalPath)
        If Directory.Exists(fullPath) And File.Exists(fullPath & "\index.html") Then
            'Logger.getInstance().write("Return Full Path" & fullPath)
            'returnDirContents(c, fullPath)
            returnFile(c, fullPath & "\index.html")
        ElseIf File.Exists(fullPath) Then
            'Logger.getInstance().write("Return File Path" & fullPath)
            returnFile(c, fullPath)
            'returnFileArchived(c, fullPath)
        Else
            return404(c)
        End If
    End Sub
    Private Sub returnDirContents(ByVal context As HttpListenerContext, ByVal dirPath As String)
        context.Response.ContentType = "text/html"
        context.Response.ContentEncoding = Encoding.UTF8
        Using sw = New StreamWriter(context.Response.OutputStream)
            sw.WriteLine("<html>")
            sw.WriteLine("<head meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""><title>Access Denied</title></head>")
            'sw.WriteLine("<body><ul>")
            'Dim dirs = Directory.GetDirectories(dirPath)
            'For Each d As Object In dirs
            'Dim link = d.Replace(rootPath, "").Replace("\"c, "/"c)
            'sw.WriteLine("<li>&lt;DIR&gt; <a href=""" + link + """>" + Path.GetFileName(d) + "</a>  </li> ")
            'Next
            'Dim files = Directory.GetFiles(dirPath)
            'For Each f As Object In files
            'Dim link = f.Replace(rootPath, "").Replace("\"c, "/"c)
            'sw.WriteLine(" <li> <a href=""" + link + """> " + Path.GetFileName(f) + " </a>  </li> ")
            'Next
            'sw.WriteLine("</ul>  </body>  </html> ")
            sw.WriteLine("<body><h2>404 Not Found</h2><p>The request " & dirPath & " not found.</body></html>")
        End Using
        context.Response.OutputStream.Close()
    End Sub
    Private Shared Sub returnFile(ByVal context As HttpListenerContext, ByVal filePath As String)


        context.Response.ContentType = getcontentType(Path.GetExtension(filePath))
        Dim buffer = New Byte(bufferSize - 1) {}
        Using fs = File.OpenRead(filePath)
            context.Response.ContentLength64 = fs.Length
            Dim read As Integer
            While (InlineAssignHelper(read, fs.Read(buffer, 0, buffer.Length))) > 0
                context.Response.OutputStream.Write(buffer, 0, read)
            End While
        End Using
        context.Response.OutputStream.Close()
    End Sub
    Private Shared Sub returnFileArchived(ByVal context As HttpListenerContext, ByVal filePath As String)
        Logger.getInstance().write("Getting file " & filePath)
        context.Response.ContentType = getcontentType(Path.GetExtension(filePath))

        Try
            Dim strBuff As Stream = webArchiver.OpenEntry(filePath)
            Dim sr As StreamReader = New StreamReader(strBuff)
            If sr.ReadToEnd().Length > 0 Then
                strBuff = webArchiver.OpenEntry(filePath)
                strBuff.CopyTo(context.Response.OutputStream)
                context.Response.OutputStream.Close()
            Else
                return404(context)
            End If
        Catch e As Exception
            Logger.getInstance().write("Error " & e.ToString())
        End Try
    End Sub
    Private Shared Sub return404(ByVal context As HttpListenerContext)
        context.Response.StatusCode = 404
        context.Response.Close()
    End Sub
    Private Shared Function tuneUrl(ByVal url As String) As String
        ' decode URL here
        url = url.Replace("/"c, "\"c)
        url = url.Substring(1)
        Return url
    End Function
    Private Shared Function getcontentType(ByVal extension As String) As String
        Select Case extension
            Case ".avi"
                Return "video/x-msvideo"
            Case ".css"
                Return "text/css"
            Case ".doc"
                Return "application/msword"
            Case ".gif"
                Return "image/gif"
            Case ".htm", ".html"
                Return "text/html"
            Case ".jpg", ".jpeg"
                Return "image/jpeg"
            Case ".js"
                Return "application/x-javascript"
            Case ".mp3"
                Return "audio/mpeg"
            Case ".png"
                Return "image/png"
            Case ".pdf"
                Return "application/pdf"
            Case ".ppt"
                Return "application/vnd.ms-powerpoint"
            Case ".zip"
                Return "application/zip"
            Case ".txt"
                Return "text/plain"
            Case ".json"
                Return "application/json"
            Case Else
                Return "application/octet-stream"
        End Select
    End Function
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function
End Class