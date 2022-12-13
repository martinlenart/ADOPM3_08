using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ADOPM3_08_02
{
    class Program
    {
        public class myTasks
        {
            private async Task<string> TaskOneAsync()
            {
                await Task.Delay(1000);
                return "task one";
            }
            private async Task<string> TaskTwoAsync()
            {
                await Task.Delay(2000);
                return "task two";
            }
            public async Task<long> Run1()
            {
                var watch = new Stopwatch();
                watch.Start();

                await TaskOneAsync();
                await TaskTwoAsync();

                watch.Stop();
                return watch.ElapsedMilliseconds;
            }
            public async Task<long> Run2()
            {
                var watch = new Stopwatch();
                watch.Start();

                var t1 = TaskOneAsync();
                var t2 = TaskTwoAsync();

                await t1;
                await t2;

                watch.Stop();
                return watch.ElapsedMilliseconds;
            }
        }
        static async Task Main(string[] args)
        {
            var mt = new myTasks();
            long exe1 = await mt.Run1();
            Console.WriteLine($"Run1: {exe1}ms");

            long exe2 = await mt.Run2();
            Console.WriteLine($"Run2: {exe2}ms");
        }
    }
    //Exercise:
    //1.    Why is there a difference in execution time between Run1 and Run2. Discuss in the group and explain.
}
