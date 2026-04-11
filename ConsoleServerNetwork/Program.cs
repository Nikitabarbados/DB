using System.Net;
using System.Net.Sockets;
using System.Text;

public class ServerNetwork
{
    private readonly TcpListener _listener;
    private readonly DataService _dataService;

    public ServerNetwork()
    {
        _listener = new TcpListener(IPAddress.Any, 27015);
        _dataService = new DataService();
    }

    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine("Server started on port 27015...");

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected!");
            _ = HandleClientAsync(client); // каждый клиент в отдельной задаче
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using var stream = client.GetStream();
        var buffer = new byte[512];

        try
        {
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer);
                if (bytesRead == 0) break;

                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                Console.WriteLine($"Request: {request}");

                string response = await _dataService.GetResponseAsync(request);

                byte[] data = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(data);
                Console.WriteLine($"Sent: {response}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client disconnected: {ex.Message}");
        }
    }
}