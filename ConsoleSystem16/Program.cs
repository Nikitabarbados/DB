using System;
using System.Threading;

class PassengerStop
{
    private int passengers = 0;
    private readonly object lockObj = new object();

    public int GetPassengers()
    {
        lock (lockObj)
            return passengers;
    }

    public void AddPassengers(int count)
    {
        lock (lockObj)
        {
            passengers += count;
            Console.WriteLine($"На зупинку прибуло {count} пасажирів. Тепер їх {passengers}");
        }
    }

    public int TakePassengers(int maxCount)
    {
        lock (lockObj)
        {
            int taken = Math.Min(passengers, maxCount);
            passengers -= taken;
            return taken;
        }
    }
}

class Program
{
    static PassengerStop stop = new PassengerStop();

    static AutoResetEvent passengerEvent = new AutoResetEvent(false);
    static Semaphore semaphore;
    static Barrier barrier;

    static Random rnd = new Random();

    static void Main()
    {
        semaphore = new Semaphore(1, 1);
        barrier = new Barrier(1, b =>
        {
            Console.WriteLine("Посадка завершена автобус вирушає\n");
        });

        new Thread(Dispatcher).Start();
        new Thread(Bus).Start();
    }

    static void Dispatcher()
    {
        while (true)
        {
            Thread.Sleep(rnd.Next(1000, 3000));

            int newPassengers = rnd.Next(1, 10);
            stop.AddPassengers(newPassengers);

            passengerEvent.Set();
        }
    }

    static void Bus()
    {
        int capacity = 20;
        int busNumber = 150;

        while (true)
        {
            passengerEvent.WaitOne();

            Console.WriteLine($"\nАвтобус номер {busNumber} під’їхав");

            int waiting = stop.GetPassengers();
            int toTake = Math.Min(waiting, capacity);

            if (toTake == 0)
            {
                Console.WriteLine("Немає пасажирів");
                continue;
            }

            barrier = new Barrier(toTake + 1);

            for (int i = 0; i < toTake; i++)
            {
                new Thread(BoardPassenger).Start();
            }

            barrier.SignalAndWait();

            int taken = stop.TakePassengers(capacity);

            Console.WriteLine($"Автобус номер{busNumber} забрав {taken} пасажирів. Залишилось {stop.GetPassengers()}");
        }
    }

    static void BoardPassenger()
    {
        semaphore.WaitOne();

        Console.WriteLine($"Пасажир сідає {Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(rnd.Next(200, 600));

        semaphore.Release();

        barrier.SignalAndWait();
    }
}