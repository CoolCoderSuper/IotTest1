Imports System.Net
Imports System.Net.WebSockets
Imports System.Text
Imports System.Text.Json
Imports System.Threading
Imports Microsoft.AspNetCore.Http

Public Class NotificationService
    Public Shared Async Function Toggle(state As Boolean) As Task(Of Boolean)
        For Each client In _clients.ToArray()
            Dim dataBytes As Byte() = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(New Request(RequestType.Toggle, state)))
            Await client.SendAsync(dataBytes, WebSocketMessageType.Text, True, CancellationToken.None)
        Next
        Return True
    End Function

    Private Shared ReadOnly _clients As New List(Of WebSocket)

    Public Shared Async Function SocketHandler(context As HttpContext) As Task
        If context.WebSockets.IsWebSocketRequest Then
            Dim webSocket = Await context.WebSockets.AcceptWebSocketAsync()
            SyncLock _clients
                _clients.Add(webSocket)
            End SyncLock
            Try
                While webSocket.State = WebSocketState.Open
                End While
            Finally
                SyncLock _clients
                    _clients.Remove(webSocket)
                End SyncLock
                webSocket.Dispose()
            End Try
        Else
            context.Response.StatusCode = HttpStatusCode.BadRequest
        End If
    End Function
End Class

Public Class Request
    Public Sub New(type As RequestType, info As Object)
        Me.Type = type
        Me.Info = info
    End Sub

    Public Property Type As RequestType
    Public Property Info As Object
End Class

Public Enum RequestType
    Toggle
End Enum