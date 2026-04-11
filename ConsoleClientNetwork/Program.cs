using System.Net.Sockets;
using System.Text;

public class ClientNetwork
{
    public async Task StartAsync()
    {
        using var client = new TcpClient();
        await client.ConnectAsync("localhost", 27015);
        Console.WriteLine("Connected to server!");
        Console.WriteLine("Commands: time, date, weather, euro, bitcoin\n");

        using var stream = client.GetStream();

        while (true)
        {
            Console.Write("Enter command: ");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input.ToLower() == "exit") break;

            byte[] data = Encoding.UTF8.GetBytes(input.Trim());
            await stream.WriteAsync(data);

            var buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Console.WriteLine($"Server: {response}\n");
        }

        Console.WriteLine("Disconnected.");
    }
}