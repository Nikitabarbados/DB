using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

class Program
{
    static string currentText = "";
    static readonly object fileLock = new object();

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Введіть текст (Enter для оновлення, 'exit' для виходу):\n");

        while (true)
        {
            string input = Console.ReadLine();

            if (input.ToLower() == "exit")
                break;

            currentText = input;

            new Thread(CountWords).Start();
            new Thread(CountNumbers).Start();
            new Thread(CountPunctuation).Start();
            new Thread(MostFrequentWord).Start();
            new Thread(AverageWordLength).Start();
        }
    }

    static void CountWords()
    {
        var words = Regex.Matches(currentText, @"\b[а-яА-Яa-zA-Z]+\b");
        WriteResult($"Кількість слів: {words.Count}");
    }

    static void CountNumbers()
    {
        var numbers = Regex.Matches(currentText, @"\b\d+\b");
        WriteResult($"Кількість чисел: {numbers.Count}");
    }

    static void CountPunctuation()
    {
        int count = currentText.Count(char.IsPunctuation);
        WriteResult($"Кількість розділових знаків: {count}");
    }

    static void MostFrequentWord()
    {
        var words = Regex.Matches(currentText.ToLower(), @"\b[а-яА-Яa-zA-Z]+\b")
                         .Select(m => m.Value);

        var mostCommon = words
            .GroupBy(w => w)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        string result = mostCommon != null
            ? $"Найчастіше слово: {mostCommon.Key} ({mostCommon.Count()})"
            : "Найчастіше слово: немає";

        WriteResult(result);
    }

    static void AverageWordLength()
    {
        var words = Regex.Matches(currentText, @"\b[а-яА-Яa-zA-Z]+\b")
                         .Select(m => m.Value);

        double avg = words.Any() ? words.Average(w => w.Length) : 0;

        WriteResult($"Середня довжина слова: {avg:F2}");
    }

    static void WriteResult(string text)
    {
        lock (fileLock)
        {
            File.AppendAllText("result.txt", text + Environment.NewLine);
        }

        Console.WriteLine(text);
    }
}