using System.Collections.Concurrent;
using System.Text;

class Program
{
    static ManualResetEvent pauseEvent = new ManualResetEvent(true);
    static CancellationTokenSource cts = new CancellationTokenSource();

    static ConcurrentQueue<string> filesQueue = new ConcurrentQueue<string>();

    static int filesProcessed = 0;
    static int matchesFound = 0;

    static string searchWord = "";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Введіть шлях до директорії:");
        string path = Console.ReadLine();

        Console.WriteLine("Введіть слово для пошуку:");
        searchWord = Console.ReadLine().ToLower();

        if (!Directory.Exists(path))
        {
            Console.WriteLine("Папка не знайдена!");
            return;
        }

        var files = Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories);
        foreach (var file in files)
            filesQueue.Enqueue(file);

        Console.WriteLine("\n[Пошук запущено]");

        int workerCount = 2;
        var workers = new List<Thread>();

        for (int i = 0; i < workerCount; i++)
        {
            var thread = new Thread(SearchWorker);
            workers.Add(thread);
            thread.Start();
        }

        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.P)
                {
                    pauseEvent.Reset();
                    Console.WriteLine("Пауза");
                }
                else if (key == ConsoleKey.R)
                {
                    pauseEvent.Set();
                    Console.WriteLine("Продовження");
                }
                else if (key == ConsoleKey.S)
                {
                    cts.Cancel();
                    Console.WriteLine("Зупинка");
                    break;
                }
                else if (key == ConsoleKey.Escape)
                {
                    cts.Cancel();
                    return;
                }
            }

            Console.Write($"\rОброблено: {filesProcessed} Знайдено: {matchesFound} ");
            Thread.Sleep(200);
        }

        foreach (var t in workers)
            t.Join();

        Console.WriteLine("\n\nПошук завершено");
        Console.WriteLine($"Всього файлів: {filesProcessed}");
        Console.WriteLine($"Знайдено входжень: {matchesFound}");
    }

    static void SearchWorker()
    {
        while (!cts.Token.IsCancellationRequested)
        {
            pauseEvent.WaitOne();

            if (filesQueue.TryDequeue(out string file))
            {
                try
                {
                    string text = File.ReadAllText(file).ToLower();

                    int count = CountOccurrences(text, searchWord);

                    if (count > 0)
                    {
                        Interlocked.Add(ref matchesFound, count);
                        Console.WriteLine($"\n{file} {count}");
                    }

                    Interlocked.Increment(ref filesProcessed);
                }
                catch { }
            }
            else
            {
                break;
            }
        }
    }

    static int CountOccurrences(string text, string word)
    {
        int count = 0;
        int index = 0;

        while ((index = text.IndexOf(word, index)) != -1)
        {
            count++;
            index += word.Length;
        }

        return count;
    }
}