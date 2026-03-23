using System.Text;
using System.Numerics;
using System.Diagnostics;

// -----------------------------------------------------------------------------------------------------

//class Program
//{
//    private static readonly object FileLock = new object();
//    static HttpClient client = new HttpClient();

//    static void Main()
//    {
//        Console.OutputEncoding = Encoding.UTF8;

//        int processorCount = Environment.ProcessorCount;
//        Console.WriteLine($"Кількість ядер процесора: {processorCount}");
//        Console.WriteLine("Режим: ВСІ ЯДРА\n");
//        var sw = Stopwatch.StartNew();

//        Parallel.For(0, 100, new ParallelOptions
//        {
//            MaxDegreeOfParallelism = processorCount
//        }, i =>
//        {
//            if (i % 3 == 0)
//                MakeApiRequest(i);
//        });

//        Console.WriteLine($"\nЧас виконання (всі ядра): {sw.Elapsed}");

//        static void MakeApiRequest(int i)
//        {
//            try
//            {
//                var response = client
//                    .GetStringAsync($"https://jsonplaceholder.typicode.com/posts/{i + 1}")
//                    .Result;

//                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Запит {i}");

//                WriteToFile($"Результат {i}: {response.Substring(0, 50)}");
//            }
//            catch (Exception ex)
//            {
//                WriteToFile($"Помилка {i}: {ex.Message}");
//            }
//        }

//        static void WriteToFile(string text)
//        {
//            lock (FileLock)
//            {
//                File.AppendAllText("output.txt", text + Environment.NewLine);
//            }
//        }
//    }
//}
// -----------------------------------------------------------------------------------------------------
//class Program
//{
//    // синхронізація для запису у файл
//    private static readonly object FileLock = new object();

//    static void Main()
//    {
//        Console.OutputEncoding = Encoding.UTF8;

//        // отримуємо кількість логічних процесорів (ядер)
//        int processorCount = Environment.ProcessorCount;
//        Console.WriteLine($"Кількість ядер процесора: {processorCount}");
//        Thread.Sleep(2000);

//        // створюємо список задач для виконання запитів до API
//        var tasks = new List<Task>();

//        // паралельний цикл від 0 до 100 з кроком 3, який генерує запити до API і записує результат у файл
//        // застосовується для ітерації по діапазону чисел (від початкового до кінцевого), це цикл по числовому діапазону або індексу
//        // MaxDegreeOfParallelism - максимальна кількість паралельних тасків
//        Parallel.For(0, 100, new ParallelOptions { MaxDegreeOfParallelism = processorCount }, i =>
//        {
//            Console.WriteLine("Ітерація Parallel.For: " + i);
//            if (i % 3 == 0)
//            {
//                // використовується синхронна операція, інакше ефект трохи втрачається
//                // задачі розподіляються по ядрах без додаткових накладних витрат на асинхронні переключення контексту
//                MakeApiRequest(i);
//            }
//        });

//        // паралельна обробка рядків із колекції
//        var textCollection = new List<string>
//        {
//            "Hello World", "C# Programming", "Parallel Programming", "Synchronous Tasks", "Multithreading"
//        };

//        // створюємо список задач для обробки рядків
//        var textTasks = new List<Task>();

//        // Parallel.ForEach застосовується для ітерації по елементах колекції (списки, масиви тощо)
//        Parallel.ForEach(textCollection, message =>
//        {
//            // синхронна операція запису у файл
//            WriteMessageToFile(message);
//        });

//        // паралельне виконання кількох задач, таких як завантаження веб-сторінок
//        ParallelInvoke();

//        Console.WriteLine("Усі задачі завершені.");
//    }

//    // синхронний метод для запиту до API
//    static void MakeApiRequest(int i)
//    {
//        try
//        {
//            var client = new HttpClient();
//            var response = client.GetStringAsync($"https://jsonplaceholder.typicode.com/posts/{i + 1}").Result; // .Result - синхронний виклик
//            Console.WriteLine($"Запит до API для {i} завершено на потоці {Thread.CurrentThread.ManagedThreadId}");
//            WriteToFile($"Результат запиту {i}: {response.Substring(0, 100)}");
//        }
//        catch (Exception ex)
//        {
//            WriteToFile($"Помилка запиту для {i}: {ex.Message}");
//        }
//    }

//    static void WriteMessageToFile(string message)
//    {
//        Thread.Sleep(500); // синхронна затримка
//        WriteToFile($"Записано у файл: {message} на потоці {Thread.CurrentThread.ManagedThreadId}");
//    }

//    // метод для запису даних у файл
//    static void WriteToFile(string text)
//    {
//        try
//        {
//            // lock потрібен для синхронізації доступу до файлу, інакше буде виключення
//            lock (FileLock)
//            {
//                File.AppendAllText("output.txt", text + Environment.NewLine);
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Помилка запису у файл: {ex.Message}");
//        }
//    }

//    // приклад паралельного виконання кількох задач (наприклад, завантаження кількох сторінок)
//    static void ParallelInvoke()
//    {
//        Parallel.Invoke(
//            // обгортаємо синхронні методи в делегати
//            () => DownloadWebPage("https://jsonplaceholder.typicode.com/posts"),
//            () => DownloadWebPage("https://jsonplaceholder.typicode.com/comments"),
//            () => DownloadWebPage("https://jsonplaceholder.typicode.com/albums")
//        );
//    }

//    // синхронний метод для завантаження веб-сторінки
//    static void DownloadWebPage(string url)
//    {
//        try
//        {
//            var client = new HttpClient();
//            string content = client.GetStringAsync(url).Result;
//            Console.WriteLine($"Завантаження сторінки {url} завершено на потоці {Thread.CurrentThread.ManagedThreadId}");
//            WriteToFile($"Сторінка {url} завантажена. Розмір: {content.Length} символів");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Помилка при завантаженні {url}: {ex.Message}");
//        }
//    }
//}
// -----------------------------------------------------------------------------------------------------
class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var numbers = Enumerable.Range(1, 50000).ToArray();

        Console.WriteLine("=== Послідовне виконання ===");
        var stopwatch = Stopwatch.StartNew(); // запускаємо таймер для вимірювання часу роботи

        var sequentialResult = ComputeFactorialsSequential(numbers); // послідовне обчислення факторіалів

        stopwatch.Stop(); // зупиняємо таймер
        Console.WriteLine($"Час виконання: {stopwatch.ElapsedMilliseconds} мс\n");

        Console.WriteLine("=== Паралельне виконання ===");
        stopwatch.Restart();

        var parallelResult = ComputeFactorialsParallel(numbers); // паралельне обчислення факторіалів

        stopwatch.Stop();
        Console.WriteLine($"Час виконання: {stopwatch.ElapsedMilliseconds} мс\n");

        Console.WriteLine("Обробка завершена.");
    }

    static BigInteger[] ComputeFactorialsSequential(int[] numbers)
    {
        // масив для зберігання результатів
        BigInteger[] results = new BigInteger[numbers.Length];
        results[0] = 1; // факторіал 1 (0! = 1)

        // проходимо по масиву і обчислюємо факторіали
        for (int i = 1; i < numbers.Length; i++)
        {
            results[i] = results[i - 1] * numbers[i]; // множимо попереднє значення на поточне число
        }

        return results; // масив факторіалів
    }

    static BigInteger[] ComputeFactorialsParallel(int[] numbers)
    {
        // масив для зберігання результатів
        BigInteger[] results = new BigInteger[numbers.Length];
        results[0] = 1;

        // обчислюємо розмір блоку для кожного потоку, наприклад, 4 шматки по 12500 елементів
        int chunkSize = numbers.Length / Environment.ProcessorCount;

        // використовуємо паралельний цикл для обробки блоків чисел
        Parallel.For(0, Environment.ProcessorCount, chunk =>
        {
            // обчислюємо початковий та кінцевий індекс блоку
            int start = chunk * chunkSize;
            int end = (chunk == Environment.ProcessorCount - 1) ? numbers.Length : start + chunkSize;

            // ініціалізуємо локальний факторіал
            var localFactorial = (start == 0) ? 1 : results[start - 1];

            // обчислюємо факторіали у своєму блоці
            for (int i = start; i < end; i++)
            {
                localFactorial *= numbers[i]; // множимо попереднє значення на поточне число
                results[i] = localFactorial; // зберігаємо результат у масив
            }
        });

        return results; // масив факторіалів
    }
}
// -----------------------------------------------------------------------------------------------------