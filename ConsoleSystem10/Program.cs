using System;
using System.Threading;

class MyThreads
{
    static int gotovkachas;

    static void Main()
    {
        Console.WriteLine("микита ставить качку в духовку");

        Thread duckThread = new Thread(gotovkaduck);
        duckThread.Start();

        Console.WriteLine("микита робе домашки");
        Console.WriteLine("микита дивится тіктоки");
        Console.WriteLine("микита гладить кішку");

        Thread.Sleep(10000);

        Console.WriteLine("микита згадав про качку і йде перевірити");

        if (gotovkachas <= 10)
        {
            Console.WriteLine("сорі тепер це вугілля а не качка");
        }
        else
        {
            Console.WriteLine("качка ще готується");
            Console.WriteLine("СМАЧНОГО!!");
        }

        duckThread.Join();
    }

    static void gotovkaduck()
    {
        Random rand = new Random();
        gotovkachas = rand.Next(3, 15);
            
        Console.WriteLine("качка готується час приготування: " + gotovkachas + " секунд");

        Thread.Sleep(gotovkachas * 1000);

        Console.WriteLine("качка готова!");
    }
}