using System;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
// ----------------------------------------------------------------------------------------------
//class Program
//{
//    // створює бар'єр для 3 потоків з дією після завершення кожного етапу
//    static Barrier barrier = new Barrier(3, (b) =>
//    {
//        Console.WriteLine($"\nЕтап {b.CurrentPhaseNumber + 1} завершено усіма, переходимо до наступного!\n");
//    });

//    static void Friend(object data)
//    {
//        var friendData = (Tuple<string, string[]>)data;
//        string name = friendData.Item1;
//        string[] actions = friendData.Item2;
//        var random = new Random();

//        foreach (var action in actions)
//        {
//            int duration = random.Next(1000, 2000);
//            Console.WriteLine($"{name}: Розпочинає таку дію: {action}");
//            Thread.Sleep(duration);
//            Console.WriteLine($"{name}: Завершує цю дію");
//            barrier.SignalAndWait(); // чекаємо, поки всі дійдуть до бар'єру
//        }

//        Console.WriteLine($"{name}: Готово!");
//    }

//    static void Main()
//    {
//        Console.OutputEncoding = System.Text.Encoding.UTF8;

//        var friends = new[]
//        {
//            Tuple.Create("Костянтин", new[]
//                {
//                    "виходить з дому з квитком на концерт Мадонни",
//                    "їде на таксі до вокзалу, слухаючи її запальні хіти",
//                    "сідає в потяг з Русланом та Оленою"
//                }),
//            Tuple.Create("Руслан", new[]
//                {
//                    "збирає речі і виходить з дому",
//                    "розминається на вокзалі перед танцями на стадіоні",
//                    "сідає в потяг з Костянтином та Оленою"
//                }),
//            Tuple.Create("Олена", new[]
//                {
//                    "робить останній штрих макіяжу перед виходом",
//                    "йде пішки на вокзал, бо живе на Привокзальній",
//                    "сідає в потяг з Русланом та Костянтином"
//                })
//        };

//        var threads = new Thread[friends.Length];

//        for (int i = 0; i < friends.Length; i++)
//        {
//            threads[i] = new Thread(Friend);
//            threads[i].Start(friends[i]);
//        }

//        foreach (var thread in threads)
//        {
//            thread.Join();
//        }

//        Console.WriteLine("\nЇдемо в Київ на концерт Мадонни!!! ЄЄЄЄ )))\n");

//        var concertActions = new[]
//        {
//            Tuple.Create("Костянтин", new[]
//                {
//                    "чекає біля турнікетів на стадіоні з великим плакатом",
//                    "проходить турнікети, шукаючи де сцена",
//                    "зустрічається з друзями біля сцени, співаючи пісні"
//                }),
//            Tuple.Create("Руслан", new[]
//                {
//                    "танцює біля турнікетів на стадіоні",
//                    "проходить турнікети, перевіряючи зачіску",
//                    "танцює з бодігардами під сценою"
//                }),
//            Tuple.Create("Олена", new[]
//                {
//                    "фотографується біля турнікетів на стадіоні",
//                    "проходить турнікети, хутчіш до голден тріанглу!",
//                    "махає руками Мадонні біля сцени! а та підмаргує у відповідь"
//                })
//        };

//        for (int i = 0; i < friends.Length; i++)
//        {
//            threads[i] = new Thread(Friend);
//            threads[i].Start(concertActions[i]);
//        }

//        foreach (var thread in threads)
//        {
//            thread.Join();
//        }

//        Console.WriteLine("\nКонцерт Мадонни в розпалі!!!");
//    }
//}
// ----------------------------------------------------------------------------------------------
//class Program
//{
//    static Barrier barrier = new Barrier(4, (b) =>
//    {
//        Console.WriteLine("\nУсі зібралися тому їдемо на пікнік\n");
//    });

//    static void Friend(object data)
//    {
//        var friendData = (Tuple<string, string>)data;
//        string name = friendData.Item1;
//        string item = friendData.Item2;

//        var random = new Random();

//        Console.WriteLine($"{name}: Починаю збирати {item}");
//        Thread.Sleep(random.Next(1000, 3000));
//        Console.WriteLine($"{name}: Закінчив збирати {item}");

//        barrier.SignalAndWait();

//        Console.WriteLine($"{name}: Насолоджується пікніком");
//    }

//    static void Main()
//    {
//        Console.OutputEncoding = System.Text.Encoding.UTF8;

//        var friends = new[]
//        {
//            Tuple.Create("Костянтин", "мангал "),
//            Tuple.Create("Руслан", "м'яч"),
//            Tuple.Create("Олена", "їжу"),
//            Tuple.Create("Ірина", "ковдру")
//        };

//        Thread[] threads = new Thread[friends.Length];

//        for (int i = 0; i < friends.Length; i++)
//        {
//            threads[i] = new Thread(Friend);
//            threads[i].Start(friends[i]);
//        }

//        foreach (var t in threads)
//        {
//            t.Join();
//        }

//        Console.WriteLine("\nПікнік у самому розпалі");
//    }
//}
// ----------------------------------------------------------------------------------------------
///* - ProcessText: читає текст, знаходить слова (2+ літери, український алфавіт),
//підраховує частоту кожного слова та записує відсортований список у 1.txt.
//- FindRhymes: шукає рими для слів довжиною >7 символів (збіг останніх 4 символів)
//і записує результати в 2.txt.
//- FindPalindromes: виявляє паліндроми (слова, що читаються однаково в обидва боки)
//та записує їх у 3.txt.
//- CreateNewWords: створює нові слова, комбінуючи наявні з перекриттям від 4 символів,
//і записує унікальні комбінації в 4.txt.

//ManualResetEvent dataReady використовується для того, щоб потоки FindRhymes,
//FindPalindromes і CreateNewWords чекали завершення роботи методу ProcessText,
//який ініціалізує масив words і словник wordFrequency. ці дані необхідні для роботи
//інших потоків, тому dataReady.Set() викликається після завершення ProcessText,
//а інші потоки викликають dataReady.WaitOne() перед початком своєї роботи.

//в заголовку показується процес пошуку рим (5888), є проблема з літерою і в тексті, при бажанні можете самі спробувати виправити :)
// */
//class Program
//{
//    static ManualResetEvent dataReady = new ManualResetEvent(false);

//    static ConcurrentDictionary<string, int> wordFrequency = new ConcurrentDictionary<string, int>();
//    static string[] words = Array.Empty<string>();

//    static void Main()
//    {
//        Console.OutputEncoding = Encoding.UTF8;

//        var processingThread = new Thread(ProcessText);
//        var findRhymesThread = new Thread(FindRhymes);
//        var findPalindromesThread = new Thread(FindPalindromes);
//        var createNewWordsThread = new Thread(CreateNewWords);

//        processingThread.Start();
//        findRhymesThread.Start();
//        findPalindromesThread.Start();
//        createNewWordsThread.Start();

//        processingThread.Join();
//        findRhymesThread.Join();
//        findPalindromesThread.Join();
//        createNewWordsThread.Join();

//        // усі потоки завершили роботу
//        Console.WriteLine("Усі потоки завершили роботу!");
//    }

//    static void ProcessText()
//    {
//        try
//        {
//            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
//            string path = "C:\\!Files\\text\\kobzar.txt";
//            if (!File.Exists(path))
//            {
//                // файл не знайдено
//                Console.WriteLine("Файл не знайдено!");
//                return;
//            }

//            string text = File.ReadAllText(path, Encoding.GetEncoding("windows-1251"));
//            var regex = new Regex(@"[а-яіїєґ']{2,}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
//            words = regex.Matches(text.ToLower()).Cast<Match>().Select(m => m.Value).ToArray();
//            if (words.Length == 0)
//            {
//                // не знайдено жодного слова
//                Console.WriteLine("Не знайдено жодного слова!");
//                return;
//            }

//            foreach (var word in words)
//            {
//                wordFrequency.AddOrUpdate(word, 1, (_, count) => count + 1);
//            }

//            var sortedFrequency = wordFrequency.OrderByDescending(kv => kv.Value)
//                                               .Select(kv => $"{kv.Key}: {kv.Value}");
//            File.WriteAllLines("1.txt", sortedFrequency);

//            // оброблено слів у тексті
//            Console.WriteLine($"Оброблено слів у тексті: {words.Length}");
//            // унікальних слів у словнику
//            Console.WriteLine($"Унікальних слів у словнику: {wordFrequency.Count}");
//            // аналіз тексту завершено
//            Console.WriteLine("Аналіз тексту завершено! Файл 1.txt показує список слів, та скільки раз вони зустрілися");
//            dataReady.Set();
//            System.Diagnostics.Process.Start("notepad.exe", "1.txt"); // відкриваємо 1.txt

//        }
//        catch (Exception ex)
//        {
//            // помилка обробки тексту
//            Console.WriteLine($"Помилка обробки тексту: {ex.Message}");
//        }
//    }

//    static void FindRhymes()
//    {
//        dataReady.WaitOne();
//        try
//        {
//            var wordsForRhymes = words.Where(w => w.Length > 7).Distinct().ToList();
//            var rhymes = new List<string>();
//            int totalWords = wordsForRhymes.Count;

//            for (int i = 0; i < totalWords; i++)
//            {
//                var word = wordsForRhymes[i];
//                // обробка слова
//                Console.Title = $"Обробка слова {i + 1} з {totalWords}";
//                var suffix = word[^4..];
//                var foundRhymes = wordsForRhymes.Where(w => w != word && w.EndsWith(suffix)).Distinct().ToList();

//                if (foundRhymes.Any())
//                {
//                    // рими для слова
//                    rhymes.Add($"Рими для {word}: {string.Join(", ", foundRhymes)}\n");
//                }
//            }

//            File.WriteAllLines("2.txt", rhymes);
//            // пошук рим завершено
//            Console.WriteLine("Пошук рим завершено!");
//            System.Diagnostics.Process.Start("notepad.exe", "2.txt"); // відкриваємо 2.txt

//        }
//        catch (Exception ex)
//        {
//            // помилка пошуку рим
//            Console.WriteLine($"Помилка пошуку рим: {ex.Message}");
//        }
//    }

//    static void FindPalindromes()
//    {
//        dataReady.WaitOne();
//        try
//        {
//            var palindromes = words
//                .Where(w => w.Length > 1 && string.Equals(w, new string(w.Reverse().ToArray()), StringComparison.OrdinalIgnoreCase))
//                .Distinct()
//                .OrderBy(w => w)
//                .ToList();

//            File.WriteAllLines("3.txt", palindromes);
//            // пошук паліндромів завершено
//            Console.WriteLine("Пошук паліндромів завершено! Результати у файлі 3.txt");
//            System.Diagnostics.Process.Start("notepad.exe", "3.txt"); // відкриваємо 3.txt

//        }
//        catch (Exception ex)
//        {
//            // помилка пошуку паліндромів
//            Console.WriteLine($"Помилка пошуку паліндромів: {ex.Message}");
//        }
//    }

//    static void CreateNewWords()
//    {
//        dataReady.WaitOne();
//        try
//        {
//            var newWords = new HashSet<string>();
//            var wordSet = new HashSet<string>(wordFrequency.Keys);
//            int len = 4;
//            int totalWords = words.Length;
//            int processedWords = 0;

//            foreach (var word1 in words)
//            {
//                if (word1.Length < len) continue;
//                processedWords++;
//                double progress = (processedWords / (double)totalWords) * 100;

//                foreach (var word2 in words)
//                {
//                    if (word2.Length < len || word1 == word2) continue;
//                    for (int overlap = len; overlap < word1.Length; overlap++)
//                    {
//                        string suffix = word1[^overlap..];
//                        if (word2.StartsWith(suffix))
//                        {
//                            string newWord = word1 + word2[suffix.Length..];
//                            if (!wordSet.Contains(newWord) && newWords.Add(newWord))
//                            {
//                                Console.SetCursorPosition(0, 4);
//                                // нове слово створено
//                                Console.WriteLine($"Нове слово: {newWord} (з {word1} + {word2}) {progress:F2}%                          ");
//                            }
//                        }
//                    }
//                }
//            }

//            File.WriteAllLines("4.txt", newWords);
//            // створення нових слів завершено
//            Console.WriteLine("\nСтворення нових слів завершено!");
//            System.Diagnostics.Process.Start("notepad.exe", "4.txt"); // відкриваємо 4.txt

//        }
//        catch (Exception ex)
//        {
//            // помилка створення нових слів
//            Console.WriteLine($"Помилка створення нових слів: {ex.Message}");
//        }
//    }
//}
// ----------------------------------------------------------------------------------------------

class Program
{
    static ManualResetEvent teaTimeStart = new ManualResetEvent(false);
    static ManualResetEvent teaTimeEnd = new ManualResetEvent(false);

    static bool isWorking = true;

    static void Person(object nameObj)
    {
        string name = (string)nameObj;
        var random = new Random();

        while (true)
        {
            if (isWorking)
            {
                Console.WriteLine($"{name}: Ходить по палацу і займається справами");
            }

            if (teaTimeStart.WaitOne(0))
            {
                Console.WriteLine($"\n{name}: Кидає всі справи і йде пити чай");

                Thread.Sleep(5000);

                Console.WriteLine($"{name}: Повертається до справ\n");
            }
        }
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        string[] people = { "Король", "Королева", "Дворецький" };
        Thread[] threads = new Thread[people.Length];

        for (int i = 0; i < people.Length; i++)
        {
            threads[i] = new Thread(Person);
            threads[i].Start(people[i]);
        }

        
        Console.WriteLine("\n17:00 — час пити чай\n");

        isWorking = false;
        teaTimeStart.Set();

        Console.WriteLine("\n17:30 — чаєпиттіє завершено\n");

        teaTimeEnd.Set();
        isWorking = true;
    }
}
// ----------------------------------------------------------------------------------------------