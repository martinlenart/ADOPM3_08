using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ADOPM3_08_02
{
    class Program
    {
        internal class myTasks
        {
            private async Task<string> TaskOne()
            {
                await Task.Delay(1000);
                return "task one";
            }
            private async Task<string> TaskTwo()
            {
                await Task.Delay(2000);
                return "task two";
            }
            public async Task<long> Run1()
            {
                var watch = new Stopwatch();
                watch.Start();

                await TaskOne();
                await TaskTwo();

                watch.Stop();
                return watch.ElapsedMilliseconds;
            }
            public async Task<long> Run2()
            {
                var watch = new Stopwatch();
                watch.Start();

                var t1 = TaskOne();
                var t2 = TaskTwo();

                await t1;
                await t2;

                watch.Stop();
                return watch.ElapsedMilliseconds;
            }
        }
        static void Main(string[] args)
        {
            var mt = new myTasks();
            long exe1 = mt.Run1().Result;
            Console.WriteLine($"Run1: {exe1}ms");

            long exe2 = mt.Run2().Result;
            Console.WriteLine($"Run2: {exe2}ms");
        }
    }
    //Exercise:
    //1.    Why is there a difference in execution time between Run1 and Run2. Discuss in the group and explain.
}
