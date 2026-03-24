using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    private const string SERVER_IP = "127.0.0.1";
    private const string SERVER_PORT = "27015";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "CLIENTDZ";

        Console.WriteLine("Клієнт запущено\n");

        // Список доступних команд
        Console.WriteLine("Доступні команди:");
        Console.WriteLine("привіт");
        Console.WriteLine("як справи");
        Console.WriteLine("котра година");
        Console.WriteLine("день тижня");
        Console.WriteLine("дата");
        Console.WriteLine("хто ти");

        try
        {
            using var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(new IPEndPoint(IPAddress.Parse(SERVER_IP), int.Parse(SERVER_PORT)));

            Console.WriteLine("Підключення до сервера успішне\n");

            byte[] buffer = new byte[1024];

            while (true)
            {
                Console.Write("Введи команду: ");
                string input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                    continue;

                if (input.ToLower() == "вихід")
                {
                    Console.WriteLine("До побачення");
                    break;
                }

                byte[] data = Encoding.UTF8.GetBytes(input);
                client.Send(data);

                int bytesReceived = client.Receive(buffer);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

                Console.WriteLine($"Сервер: {response}\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }

        Console.WriteLine("\nНатисни рандом клавішу для завершення");
        Console.ReadKey();
    }
}