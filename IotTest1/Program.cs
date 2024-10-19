using System.Device.Gpio;
using System.Net.WebSockets;
using System.Text;

Console.WriteLine("Start");
var controller = new GpioController(PinNumberingScheme.Board);
var pin = 10;
controller.OpenPin(pin, PinMode.Output);
try
{
    var client = new ClientWebSocket();
    await client.ConnectAsync(new Uri("ws://192.168.11.74:754/ws"), CancellationToken.None);
    while (client.State == WebSocketState.Open)
    {
        var buf = new ArraySegment<byte>(new byte[1024]);
        await client.ReceiveAsync(buf, CancellationToken.None);
        var str = bool.Parse(Encoding.UTF8.GetString(buf.ToArray()));
        if (str)
        {
            controller.Write(pin, PinValue.High);
        }
        else
        {
            controller.Write(pin, PinValue.Low);
        }
    }
}
finally
{
    controller.ClosePin(pin);
}