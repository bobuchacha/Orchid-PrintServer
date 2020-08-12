Imports System.Net.Sockets
Imports System.Net
Imports System
Imports System.Text
Imports System.Text.RegularExpressions


Namespace SMWebSocket
    Public Class Server
        Inherits System.Net.Sockets.TcpListener
        Public Delegate Sub OnClientConnectDelegate(ByVal sender As Object,
                                             ByRef Client As SMWebSocket.Client)
        Public Event OnClientConnect As OnClientConnectDelegate
        Public Event onMessageReceived(Server As Server,
                                       client As Client,
                                       message As String)
        Public Event onServerStartError(Server As Server,
                                        e As SocketException)
        Public Event onNewClientConnected(server As Server,
                                       client As Client)
        Public Event onClientDisconnect(Server As Server,
                                        client As Client)

        Dim WithEvents PendingCheckTimer As Timers.Timer
        Dim WithEvents ClientDataAvailableTimer As Timers.Timer

        Property ClientCollection As List(Of SMWebSocket.Client)
        Private _PortNumber As Integer               ' port number
        Private _myId As Integer                    ' my id
        Private _myLogger As Logger                  ' logger
        Private Shared _idCounter As Integer        ' next id of server

        ' New Constructor, listen to IPAddress Object
        Sub New(ByVal IPAdd As IPAddress, ByVal Port As Integer)
            MyBase.New(IPAdd, Port)
            _PortNumber = Port
            PendingCheckTimer = New Timers.Timer(500)
            ClientDataAvailableTimer = New Timers.Timer(50)
            ClientCollection = New List(Of SMWebSocket.Client)

            ' set id
            _myId = _idCounter
            _idCounter += 1

            ' get logger
            _myLogger = Logger.getInstance()
        End Sub
        ' New COntructor - listen to a specific host
        Sub New(ByVal url As String, ByVal port As Integer)
            MyBase.New(IPAddress.Parse(url), port)

            _PortNumber = port
            PendingCheckTimer = New Timers.Timer(500)
            ClientDataAvailableTimer = New Timers.Timer(50)
            ClientCollection = New List(Of SMWebSocket.Client)

            ' set id
            _myId = _idCounter
            _idCounter += 1

            ' get logger
            _myLogger = Logger.getInstance()

        End Sub

        ' get Local IP Address
        Public Shared Function LocalIPAddress() As String
            Dim host As IPHostEntry
            Dim localIP As String = ""
            host = Dns.GetHostEntry(Dns.GetHostName())
            For Each ip As IPAddress In host.AddressList
                If ip.AddressFamily = AddressFamily.InterNetwork Then
                    localIP = ip.ToString()
                    Exit For
                End If
            Next
            Return localIP
        End Function

        ' Stop Server
        ' Send @Close Message to all connected client, then stop 
        ' all timers before calling TcpListener stop() method
        Sub stopServer()
            On Error Resume Next
            For Each a As Client In ClientCollection
                a.SendMessage("@CLOSE")             ' sending close message command to connected clients
                a.Close()                           ' close tcpClient
            Next
            PendingCheckTimer.Stop()
            ClientDataAvailableTimer.Stop()
            MyBase.Stop()


        End Sub

        ' Start Server
        Sub startServer()
            Try
                _myLogger.write("[" & _myId & "] Starting Websocket Server")
                Me.Start()
                PendingCheckTimer.Start()
            Catch e As SocketException
                RaiseEvent onServerStartError(Me, e)
            End Try
        End Sub

        ' When client connected, this method invoked
        ' first add client to client list, and create handshake
        ' then send welcome message, and write log
        ' then start data available timer
        Sub Client_Connected(ByVal sender As Object, ByRef client As SMWebSocket.Client) Handles Me.OnClientConnect
            Me.ClientCollection.Add(client)
            AddHandler client.onClientDisconnect, AddressOf Client_Disconnected
            AddHandler client.onMessageReceived, AddressOf Client_MessageReceived
            client.HandShake()
            ClientDataAvailableTimer.Start()

            ' raise event 
            RaiseEvent onNewClientConnected(Me, client)
        End Sub
        Sub Client_Disconnected()

        End Sub
        Sub Client_MessageReceived(sender As Object, message As String)
            RaiseEvent onMessageReceived(Me, sender, message)
        End Sub
        Function isClientDisconnected(ByVal client As SMWebSocket.Client) As Boolean
            isClientDisconnected = False
            If Not client.isConnected Then Return True
        End Function
        Function isClientConnected(ByVal client As SMWebSocket.Client) As Boolean
            isClientConnected = False
            If client.isConnected Then Return True
        End Function

        Private Sub PendingCheckTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles PendingCheckTimer.Elapsed
            If Pending() Then RaiseEvent OnClientConnect(Me, New SMWebSocket.Client(Me.AcceptTcpClient()))
        End Sub

        Private Sub ClientDataAvailableTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles ClientDataAvailableTimer.Elapsed
            Me.ClientCollection.RemoveAll(AddressOf isClientDisconnected)           ' remove all disconnected clients
            If Me.ClientCollection.Count < 1 Then ClientDataAvailableTimer.Stop()   ' Automatically stop timer if no Client connected
            On Error Resume Next
            For Each Client As SMWebSocket.Client In Me.ClientCollection
                Client.CheckForDataAvailability()
            Next
        End Sub

        Public Function getId() As Integer
            Return _myId
        End Function
    End Class

End Namespace

