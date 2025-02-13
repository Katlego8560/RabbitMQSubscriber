using System.Threading;
using System;

class Program
{
    static async Task Main(string[] args)
    {
        // Start the subscriber in a separate thread
        var subscriberThread = new Thread(() =>
        {
            var subscriber = new Subscriber();
            Task task = subscriber.Start();
        });
        subscriberThread.Start();

        // Keep the console app running
        Console.ReadKey();
    }
}