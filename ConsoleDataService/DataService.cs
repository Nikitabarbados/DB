using System.Text.Json;

public class DataService
{
    private readonly HttpClient _http = new();
    private const string WeatherApiKey = "c682e05304a87b031fbbeccbec856fa4";
    private const string City = "Sofia";

    public async Task<string> GetResponseAsync(string request)
    {
        return request.ToLower() switch
        {
            "time" => await GetTimeAsync(),
            "date" => await GetDateAsync(),
            "weather" => await GetWeatherAsync(),
            "euro" => await GetEuroRateAsync(),
            "bitcoin" => await GetBitcoinRateAsync(),
            _ => "Unknown command. Use: time, date, weather, euro, bitcoin"
        };
    }

    private Task<string> GetTimeAsync()
        => Task.FromResult($"Current time: {DateTime.Now:HH:mm:ss}");

    private Task<string> GetDateAsync()
        => Task.FromResult($"Current date: {DateTime.Now:dd.MM.yyyy}");

    private async Task<string> GetWeatherAsync()
    {
        try
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={City}&appid={WeatherApiKey}&units=metric";
            var json = await _http.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            double temp = root.GetProperty("main").GetProperty("temp").GetDouble();
            string desc = root.GetProperty("weather")[0].GetProperty("description").GetString()!;
            double feels = root.GetProperty("main").GetProperty("feels_like").GetDouble();
            int humidity = root.GetProperty("main").GetProperty("humidity").GetInt32();

            return $"Weather in Sofia: {temp:F1}°C, {desc}, feels like {feels:F1}°C, humidity {humidity}%";
        }
        catch (Exception ex)
        {
            return $"Weather error: {ex.Message}";
        }
    }

    private async Task<string> GetEuroRateAsync()
    {
        try
        {
            var json = await _http.GetStringAsync("https://api.frankfurter.app/latest?from=EUR&to=USD,BGN");
            using var doc = JsonDocument.Parse(json);
            var rates = doc.RootElement.GetProperty("rates");

            double usd = rates.GetProperty("USD").GetDouble();
            double bgn = rates.GetProperty("BGN").GetDouble();

            return $"Euro rate: 1 EUR = {usd:F4} USD | {bgn:F4} BGN";
        }
        catch (Exception ex)
        {
            return $"Euro rate error: {ex.Message}";
        }
    }

    private async Task<string> GetBitcoinRateAsync()
    {
        try
        {
            var json = await _http.GetStringAsync("https://api.coindesk.com/v1/bpi/currentprice/USD.json");
            using var doc = JsonDocument.Parse(json);
            string rate = doc.RootElement
                .GetProperty("bpi")
                .GetProperty("USD")
                .GetProperty("rate")
                .GetString()!;

            return $"Bitcoin rate: {rate} USD";
        }
        catch (Exception ex)
        {
            return $"Bitcoin error: {ex.Message}";
        }
    }
}