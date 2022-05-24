using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ADOPM3_08_06
{
    public class PrimeSuite
    {
        public int start { get; set; }
        public int count { get; set; }  
        public int NrofPrimes { get; set; }    
    }
    internal class PrimeNumberService
    {
        //In my cache I use a Dictionary.
        // - Key is start number and count as a tuple
        // - Value is the number of primes in the span start - start+count
        static ConcurrentDictionary<(int, int), PrimeSuite> _primeNumberCache = new ConcurrentDictionary<(int, int), PrimeSuite>();

        static public async Task DisplayPrimeCountsAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                //Create my Cache Key
                int start = i * 1_000_000 + 2;
                int count = 1_000_000;

                //Without Cache
                PrimeSuite pSuite = new PrimeSuite { start = start, count = count };
                pSuite.NrofPrimes = await GetPrimesCountAsync(start, count);

                /*
                //With Cache
                //pSuite is null as I first want to check if it exists in the cache
                PrimeSuite pSuite = null;

                //I need a unique Key that represents one calculation value
                var key = (start, count);

                //Check if Cache already contains the value
                if (!_primeNumberCache.TryGetValue(key, out pSuite))              
                {
                    //the value is not in the cache - get the value the slow way 
                    pSuite = new PrimeSuite { start = start, count = count };

                    //It did not exist in the cache, calculate and add it to the cache
                    pSuite.NrofPrimes = await GetPrimesCountAsync(start, count);

                    _primeNumberCache[key] = pSuite;
                }
                */

                //Regardless if nrOfPrimes where in the cache - I now have it in nrOfPrimes
                var t = $"{pSuite.NrofPrimes} primes between {pSuite.start} and {pSuite.start + pSuite.count}";
                Console.WriteLine(t);
            }

            Console.WriteLine("Done!");
        }

        static public Task<int> GetPrimesCountAsync(int start, int count)
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
            //Instantiating PrimeService so the Cache is valid for as long as the instance is on the heap
            //var PrimesService = new PrimeNumberService();
            
            Console.WriteLine("\nInvoking DisplayPrimeCountsAsync - first time cache is empty");
            await PrimeNumberService.DisplayPrimeCountsAsync();

            Console.WriteLine("\nInvoking DisplayPrimeCountsAsync - Now from cache - check speed difference");
            await PrimeNumberService.DisplayPrimeCountsAsync();

            Console.WriteLine("\nInvoking DisplayPrimeCountsAsync - and a third time from cache");
            await PrimeNumberService.DisplayPrimeCountsAsync();
        }
    }
}
