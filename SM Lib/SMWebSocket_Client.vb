Imports System.Net.Sockets
Imports System.Net
Imports System
Imports System.Text
Imports System.Text.RegularExpressions


Namespace SMWebSocket
    Public Class Client
        Dim _TcpClient As System.Net.Sockets.TcpClient

        Public Delegate Sub OnClientDisconnectDelegateHandler()
        Public Event onClientDisconnect As OnClientDisconnectDelegateHandler
        Public Event onMessageReceived(sender As Object, message As String)

        Private Shared _nextId As Integer
        Private _myId As Integer

        Sub New(ByVal tcpClient As System.Net.Sockets.TcpClient)
            Me._TcpClient = tcpClient

            ' set Id
            _myId = _nextId
            _nextId += 1
        End Sub
        Function getId() As Integer
            Return _myId
        End Function

        Function isConnected() As Boolean
            Return Me._TcpClient.Connected
        End Function
        Public Sub Close()
            _TcpClient.Close()
        End Sub

        Sub HandShake()
            Dim stream As NetworkStream = Me._TcpClient.GetStream()

            Dim bytes As Byte()
            Dim data As String


            While Me._TcpClient.Connected
                While (stream.DataAvailable)
                    ReDim bytes(Me._TcpClient.Client.Available)
                    stream.Read(bytes, 0, bytes.Length)
                    data = System.Text.Encoding.UTF8.GetString(bytes)

                    If (New System.Text.RegularExpressions.Regex("^GET").IsMatch(data)) Then

                        Dim response As Byte() = System.Text.Encoding.UTF8.GetBytes(
                            "HTTP/1.1 101 Switching Protocols" & Environment.NewLine &
                            "Connection: Upgrade" & Environment.NewLine &
                            "Upgrade: websocket" & Environment.NewLine &
                            "Sec-WebSocket-Accept: " & Convert.ToBase64String(System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(New Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups(1).Value.Trim() & "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"))) & Environment.NewLine & Environment.NewLine)
                        stream.Write(response, 0, response.Length)
                        Exit Sub
                    Else
                        'We're going to disconnect the client here, because he's not handshacking properly (or at least to the scope of this code sample)
                        Me._TcpClient.Close() 'The next While Me._TcpClient.Connected Loop Check should fail.. and raise the onClientDisconnect Event Thereafter
                    End If
                End While
            End While
            RaiseEvent onClientDisconnect()
        End Sub

        Public Sub SendMessage(message As String)
            Dim rawData = System.Text.Encoding.UTF8.GetBytes(message)
            Dim frameCount = 0
            Dim frame(10) As Byte

            frame(0) = CByte(129)
            Try
                If rawData.Length <= 125 Then
                    frame(1) = CByte(rawData.Length)
                    frameCount = 2
                ElseIf rawData.Length >= 126 AndAlso rawData.Length <= 65535 Then
                    frame(1) = CByte(126)
                    Dim len = CInt(rawData.Length)
                    frame(2) = CByte(((len >> 8) And CByte(255)))
                    frame(3) = CByte((len And CByte(255)))
                    frameCount = 4
                Else
                    frame(1) = CByte(127)
                    Dim len = CInt(rawData.Length)
                    frame(2) = CByte(((len >> 56) And CByte(255)))
                    frame(3) = CByte(((len >> 48) And CByte(255)))
                    frame(4) = CByte(((len >> 40) And CByte(255)))
                    frame(5) = CByte(((len >> 32) And CByte(255)))
                    frame(6) = CByte(((len >> 24) And CByte(255)))
                    frame(7) = CByte(((len >> 16) And CByte(255)))
                    frame(8) = CByte(((len >> 8) And CByte(255)))
                    frame(9) = CByte((len And CByte(255)))
                    frameCount = 10
                End If
            Catch ex As Exception
                '  MsgBox(ex.ToString)
                '  MsgBox(rawData.Length)
            End Try

            Dim bLength = frameCount + rawData.Length
            ' Logger.getInstance().write("Frame Count: " & frameCount)
            ' Logger.getInstance().write("Raw Data Length: " & rawData.Length)
            Dim reply(bLength + 1) As Byte

            Dim bLim = 0
            For i = 0 To frameCount - 1
                'Console.WriteLine(bLim)
                reply(bLim) = frame(i)
                bLim += 1
            Next

            For i = 0 To rawData.Length - 1
                'Console.WriteLine(bLim)
                reply(bLim) = rawData(i)
                bLim += 1
            Next

            Try
                Dim stream As NetworkStream = Me._TcpClient.GetStream()
                stream.Write(reply, 0, reply.Length)

            Catch ex As Exception
            End Try
        End Sub

        Sub CheckForDataAvailability()
            If (Me._TcpClient.GetStream().DataAvailable) Then
                Dim stream As NetworkStream = Me._TcpClient.GetStream()
                Dim frameCount = 2
                Dim b As Byte()
                Dim len As Integer
                ReDim b(Me._TcpClient.Client.Available)
                len = stream.Read(b, 0, b.Length)         'Read the stream, don't close it.. 

                If (len > 0) Then
                    ' get opcode
                    Dim firstByte As Integer = b(0)
                    Dim opCode As Integer = firstByte And 15

                    ' opCode = 1 => text data received
                    If opCode = 1 Then
                        Dim rLength As Byte = 0
                        Dim rMaskIndex As Integer = 2
                        Dim rDataStart As Integer = 0
                        Dim Data As Byte = b(1)
                        Dim op As Byte = 127
                        rLength = (Data And op)

                        If (rLength = 126) Then rMaskIndex = 4
                        If (rLength = 127) Then rMaskIndex = 10

                        Dim masks(4) As Byte
                        Dim j As Integer = 0
                        Dim i As Integer = 0
                        For i = rMaskIndex To rMaskIndex + 3
                            masks(j) = b(i)
                            j = j + 1
                        Next
                        rDataStart = rMaskIndex + 4

                        Dim messLen As Integer = len - rDataStart
                        Dim message(messLen) As Byte
                        j = 0
                        For i = rDataStart To len - 1
                            message(j) = (b(i) Xor masks(j Mod 4))
                            j = j + 1
                        Next

                        Dim result As String
                        result = Trim(System.Text.Encoding.UTF8.GetString(message))
                        RaiseEvent onMessageReceived(Me, result)

                        ' opCode = 8 => connection closed
                    ElseIf opCode = 8 Then
                        _TcpClient.Close()
                        RaiseEvent onClientDisconnect()
                    End If
                End If
            End If
        End Sub
    End Class
End Namespace

