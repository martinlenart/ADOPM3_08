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
                Console.WriteLine(await GetPrimesCountAsync(i * 1000000 + 2, 1000000) +
                    " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1));

            Console.WriteLine("Done!");
        }

        Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() =>
               ParallelEnumerable.Range(start, count).Count(n =>
                 Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Invoking GetDotNetCountAsync");
            int count = new IOBoundAsync().GetDotNetCountAsync().Result;
            Console.WriteLine($"Number of times .Net keyword displayed is {count}");

            Console.WriteLine("Invoking DisplayPrimeCountsAsync");
            var t1 = new CPUBoundAsync().DisplayPrimeCountsAsync();
            t1.Wait();
        }
    }
    //Exercises:
    //1.  Investigate ParallelEnumerable and explain the major difference to Enumerable
    //2.  Modify code to first invoke both GetDotNetCountAsync and DisplayPrimeCountsAsync, then wait for all tasks to complete. 
}
