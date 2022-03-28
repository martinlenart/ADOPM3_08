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
                var t = await GetPrimesCountAsync(i * 1_000_000 + 2, 1_000_000) +
                    " primes between " + (i * 1_000_000) + " and " + ((i + 1) * 1_000_000 - 1);
                
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
        public static async Task Main()
        {
            /*
            Console.WriteLine("Invoking GetDotNetCountAsync");
            int count = await new IOBoundAsync().GetDotNetCountAsync();
            Console.WriteLine($"Number of times .Net keyword displayed is {count}");
            */

            Console.WriteLine("\nInvoking GetPrimesCountAsync");
            int count = await new CPUBoundAsync().GetPrimesCountAsync(2, 5_000_000);
            Console.WriteLine(count);

            Console.WriteLine("\nInvoking DisplayPrimeCountsAsync");
            await new CPUBoundAsync().DisplayPrimeCountsAsync();
        }
    }
    //Exercises:
    //1. Modify code to first invoke both GetDotNetCountAsync, GetPrimesCountAsync and DisplayPrimeCountsAsync, then wait for all tasks to complete using await. 
}
