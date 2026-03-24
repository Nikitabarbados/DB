using System.Net;
using System.Text;
using System.Net.Sockets;

class Server
{
    const int PORT = 27015;

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "SERVERDZ";

        Console.WriteLine("Доступні команди:");
        Console.WriteLine("- привіт");
        Console.WriteLine("- як справи");
        Console.WriteLine("- котра година");
        Console.WriteLine("- день тижня");
        Console.WriteLine("- дата\n");

        var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(new IPEndPoint(IPAddress.Any, PORT));
        listener.Listen(10);

        Console.WriteLine("Сервер запущено\n");

        var client = listener.Accept();
        Console.WriteLine("Клієнт підключився\n");

        while (true)
        {
            byte[] buffer = new byte[512];
            int bytes = client.Receive(buffer);

            if (bytes <= 0)
                continue;

            string request = Encoding.UTF8.GetString(buffer, 0, bytes).ToLower();
            Console.WriteLine($"Клієнт: {request}");

            string response = GetResponse(request);

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            client.Send(responseBytes);
        }
    }

    static string GetResponse(string request)
    {
        return request switch
        {
            "привіт" => "Привіт",
            "як справи" => "Чудово",
            "котра година" => DateTime.Now.ToString("HH:mm"),
            "день тижня" => DateTime.Now.ToString("dddd"),
            "дата" => DateTime.Now.ToString("dd.MM.yyyy"),
        };
    }
}