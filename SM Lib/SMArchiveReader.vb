Imports System.IO.Compression
Imports System.IO

Public Class SMArchiveReader

    Private isOpened As Boolean
    Private zipArchive As System.IO.Compression.ZipArchive
    Private isArchiveOpened As Boolean

    ' construct SMArchiveReader with file name as parameter
    Public Sub New(fileName As String)
        ' Open Zip Archive
        zipArchive = System.IO.Compression.ZipFile.OpenRead(fileName)

        ' set the flag
        isArchiveOpened = True
    End Sub

    ' open entry and return its stream
    Public Function OpenEntry(path As String) As IO.Stream

        ' exit function if archive is not opened
        If isArchiveOpened = False Then
            OpenEntry = IO.Stream.Null
            Exit Function
        End If

        ' read the archive
        Dim entry As ZipArchiveEntry
        For Each entry In zipArchive.Entries

            If entry.FullName = path Then
                OpenEntry = entry.Open()
                Exit Function
            End If
        Next

        OpenEntry = IO.Stream.Null
    End Function

    ' read file entry with path and return string
    Public Function ReadTextFileEntry(path As String) As String

        ' exit function if archive is not opened
        If isArchiveOpened = False Then
            ReadTextFileEntry = ""
            Exit Function
        End If

        ' read the archive
        Dim entry As ZipArchiveEntry
        For Each entry In zipArchive.Entries

            If entry.FullName = path Then
                Try
                    Dim stream As System.IO.Stream = entry.Open()
                    Dim streamReader As IO.StreamReader
                    streamReader = New IO.StreamReader(stream)
                    ReadTextFileEntry = streamReader.ReadToEnd()
                    streamReader.DiscardBufferedData()
                    streamReader.Dispose()
                    streamReader = Nothing
                    Exit Function
                Catch e As Exception
                    Logger.getInstance().write("Error reading file " & path & " from archiver" & vbNewLine & e.ToString())
                End Try
            End If
        Next
    End Function

    Public Sub Dispode()
        isArchiveOpened = False
        zipArchive.Dispose()
    End Sub

End Class
