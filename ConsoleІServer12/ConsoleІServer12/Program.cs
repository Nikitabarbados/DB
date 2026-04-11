using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Write("Вкажіть назву міста: ");
        string city = Console.ReadLine() ?? "Kyiv";

        string url = $"https://wttr.in/{city}?format=j1";

        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "curl/7.68.0");

            var response = await client.GetAsync(url);
            Console.WriteLine($"Код відповіді: {(int)response.StatusCode} ({response.ReasonPhrase})");
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var current = doc.RootElement
                             .GetProperty("current_condition")[0];

            string temp = current.GetProperty("temp_C").GetString() ?? "?";
            string wind = current.GetProperty("windspeedKmph").GetString() ?? "?";
            string humidity = current.GetProperty("humidity").GetString() ?? "?";

            // Вывод
            string line = new string('-', 60);
            Console.WriteLine(line);
            Console.WriteLine($"Погода в {city}:");
            Console.WriteLine();
            Console.WriteLine($"  Температура повітря : +{temp}°C");
            Console.WriteLine($"  Швидкість вітру     : {wind} km/h");
            Console.WriteLine($"  Вологість           : {humidity}%");
            Console.WriteLine(line);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }
}