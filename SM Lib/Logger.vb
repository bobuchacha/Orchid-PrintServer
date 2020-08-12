Public NotInheritable Class Logger
    Public Shared data As String
    Public Event onNewEntry(message As String)
    Private Shared _myInstance As Logger
    Private Sub New()

    End Sub
    Public Shared Function getInstance() As Logger
        If _myInstance Is Nothing Then
            _myInstance = New Logger
        End If
        Return _myInstance
    End Function
    Public Sub write(msg As String)
        data = data & Now() & " " & msg & vbNewLine
        RaiseEvent onNewEntry(msg)

    End Sub
End Class