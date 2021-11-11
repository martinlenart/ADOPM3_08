using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADOPM3_08_05
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            IEnumerable<Task<int>> tasks = Enumerable.Range(1, 5).Select(n => Task.Run(async () =>
            {
                int mysleep = random.Next(1000, 5000);
                Console.WriteLine($"I'm task {n} and I am sleeping {mysleep}ms");
                await Task.Delay(mysleep);
                return n;
            }));

            Task<Task<int>> whenAnyTask = Task.WhenAny(tasks);
            Task<int> completedTask = whenAnyTask.Result;
            Console.WriteLine("The winner is: task " + completedTask.Result);

            Task.WhenAll(tasks).Wait();
            Console.WriteLine("All tasks finished!");
        }
    }

    //Exercise:
    //1.    Discuss in the group the Linq statement so you understand the usage. Why am I using Select?
    //2.    Modify Code in Example 10_03 to create these 5 tasks, do WaitAny() and WaitAll() when clicking the Button using async/await 
    //      pattern not to block UI. Write to myGreetings.Text the winner.
}
