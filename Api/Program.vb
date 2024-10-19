Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.DependencyInjection

Public Module Program
    Public Sub Main(args As String())
        Dim builder As WebApplicationBuilder = WebApplication.CreateBuilder(args)
        builder.Services.AddControllers()
        builder.Services.AddEndpointsApiExplorer()
        builder.Services.AddSwaggerGen()
        Dim app As WebApplication = builder.Build()
        app.UseSwagger()
        app.UseSwaggerUI()
        app.UseAuthorization()
        app.MapControllers()
        app.UseWebSockets()
        app.Map("/ws", AddressOf NotificationService.SocketHandler)
        app.MapGet("/{state}", Function(state As Boolean)
                                   Return NotificationService.Send(state)
                               End Function)
#If DEBUG Then
        app.Run("http://0.0.0.0:754")
#Else
        app.Run()
#End If
    End Sub
End Module
