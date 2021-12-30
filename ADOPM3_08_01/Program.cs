using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADOPM3_08_01
{
    internal class IOBoundAsync
    {
        public async Task<int> GetDotNetCountAsync()
        {
            var html = await new HttpClient().GetStringAsync("https://dotnetfoundation.org");
            return Regex.Matches(html, @"\.NET").Count;
        }
    }
    internal class CPUBoundAsync
    {
        public async Task DisplayPrimeCountsAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                var t = await GetPrimesCountAsync(i * 1000000 + 2, 1000000) +
                    " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1);
                
                Console.WriteLine(t);
            }

            Console.WriteLine("Done!");
        }

        public Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() =>
               Enumerable.Range(start, count).Count(n =>
                 Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        }
    }
    class Program
    {
        //   - Main is abit special in Console Applications to make async, pls see
        //   - https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-7.1/async-main
        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            Console.WriteLine("Invoking GetDotNetCountAsync");
            int count = await new IOBoundAsync().GetDotNetCountAsync();
            Console.WriteLine($"Number of times .Net keyword displayed is {count}");

            Console.WriteLine("\nInvoking GetPrimesCountAsync");
            count = await new CPUBoundAsync().GetPrimesCountAsync(2, 1000_000);
            Console.WriteLine(count);

            Console.WriteLine("\nInvoking DisplayPrimeCountsAsync");
            await new CPUBoundAsync().DisplayPrimeCountsAsync();
        }
    }
    //Exercises:
    //1. Modify code to first invoke both GetDotNetCountAsync, GetPrimesCountAsync and DisplayPrimeCountsAsync, then wait for all tasks to complete. 
}
