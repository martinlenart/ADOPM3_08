using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ADOPM3_08_01
{
    public class IOBoundAsync
    {
        public async Task<int> GetDotNetCountAsync()
        {
            var html = await new HttpClient().GetStringAsync("https://dotnetfoundation.org");
            return Regex.Matches(html, @"\.NET").Count;
        }
    }
    public class CPUBoundAsync
    {
        public async Task DisplayPrimeCountsAsync()
        {
            for (int i = 0; i < 5; i++)
            {
                var t = await GetPrimesCountAsync(i * 1_000_000 + 2, 1_000_000) +
                    " primes between " + (i * 1_000_000) + " and " + ((i + 1) * 1_000_000 - 1);
                
                Console.WriteLine(t);
            }

            Console.WriteLine("Done!");
        }

        public Task<int> GetPrimesCountAsync(int start, int count)
        { 
            return Task.Run(() => GetPrimesCount(start, count));
        }


        public int GetPrimesCount(int start, int count)
        {
            return Enumerable.Range(start, count).Count(n =>
                 Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
        }
    }
    class Program
    {
        public static async Task Main()
        {
            //CPUBound Async - syncronous Method() -> asyncronous MethodAsync() through Task.Run() wrapper 
            //Syncronous calculations of Primes
            var timer = new Stopwatch();
            timer.Start();
            Console.WriteLine("\nSyncronous calculations of Primes");
            var count1 = new CPUBoundAsync().GetPrimesCount(2, 2_000_000);
            var count2 = new CPUBoundAsync().GetPrimesCount(2, 2_000_000);
            Console.WriteLine(count1 + count2);
            timer.Stop();
            Console.WriteLine($"{timer.ElapsedMilliseconds:N0}");  //3 s

            
            //asyncronous calculations of Primes using classical async / await pattern
            timer.Restart();
            Console.WriteLine("\nAsyncronous calculations of Primes using classical async / await pattern");
            var count3 = await new CPUBoundAsync().GetPrimesCountAsync(2, 2_000_000);
            var count4 = await new CPUBoundAsync().GetPrimesCountAsync(2, 2_000_000);
            Console.WriteLine(count3+count4);
            timer.Stop();
            Console.WriteLine($"{timer.ElapsedMilliseconds:N0}"); //3s

            
            //asyncronous calculations of Primes using classical running the two Tasks in parallell
            timer.Restart();
            Console.WriteLine("\nAsyncronous calculations of Primes using classical running the two Tasks in parallell");
            var countTask1 = new CPUBoundAsync().GetPrimesCountAsync(2, 2_000_000);
            var countTask2 = new CPUBoundAsync().GetPrimesCountAsync(2, 2_000_000);

            Task.WaitAll(countTask1, countTask2);
            Console.WriteLine(countTask1.Result + countTask2.Result);
            timer.Stop();
            Console.WriteLine($"{timer.ElapsedMilliseconds:N0}"); //1 s
            
            
            Console.WriteLine("\nInvoking DisplayPrimeCountsAsync");
            await new CPUBoundAsync().DisplayPrimeCountsAsync();
            
            
            // IO Bound Async - using already asyncronous MethodAsync
            Console.WriteLine("\nInvoking GetDotNetCountAsync");
            int count = await new IOBoundAsync().GetDotNetCountAsync();
            Console.WriteLine($"Number of times .Net keyword displayed is {count}");
            
        }
    }
 }
