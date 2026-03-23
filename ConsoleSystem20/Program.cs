using System;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static readonly object fileLock = new object();

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Введіть початок діапазону:");
        int start = int.Parse(Console.ReadLine());

        Console.WriteLine("Введіть кінець діапазону:");
        int end = int.Parse(Console.ReadLine());

        string path = "multiplication_table.txt";

        File.WriteAllText(path, "");

        Parallel.For(start, end + 1, i =>
        {
            string line = "";

            for (int j = 1; j <= 10; j++)
            {
                line += $"{i} x {j} = {i * j}    ";
            }

            lock (fileLock)
            {
                File.AppendAllText(path, line + Environment.NewLine);
            }

            Console.WriteLine($"Рядок для {i} записано потоком {Task.CurrentId}");
        });

        Console.WriteLine("\nТаблиця множення згенерована у файл");
    }
}