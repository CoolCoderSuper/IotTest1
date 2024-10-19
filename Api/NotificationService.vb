Imports System.Net
Imports System.Net.WebSockets
Imports System.Text
Imports System.Threading
Imports Microsoft.AspNetCore.Http

Public Class NotificationService
    Public Shared Async Function Send(state As Boolean) As Task(Of Boolean)
        For Each client In _clients.ToArray()
            Dim dataBytes As Byte() = Encoding.UTF8.GetBytes(state.ToString)
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