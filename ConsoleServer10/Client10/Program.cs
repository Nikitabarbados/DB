using System.Net;
using System.Net.Sockets;

class Program
{
    static async Task Main()
    {
        var client = new ClientNetwork();
        await client.StartAsync();
    }
}