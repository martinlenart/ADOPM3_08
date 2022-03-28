using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ADOPM3_08_06
{
    internal class PrimeNumberService
    {
        //In my cache I use a Dictionary.
        // - Key is start number and count as a tuple
        // - Value is the number of primes in the span start - start+count
        ConcurrentDictionary<(int, int), int> _primeNumberCache = new ConcurrentDictionary<(int, int), int>();

        public async Task DisplayPrimeCountsAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                //Create my Cache Key
                int start = i * 1_000_000 + 2;
                int count = 1_000_000;
                var key = (start, count);

                //Check if Cache already contains the value
                int nrOfPrimes = 0;
                if (!_primeNumberCache.TryGetValue(key, out nrOfPrimes))
                {
                    //It did not exist in the cache, calculate and add it to the cache
                    nrOfPrimes = await GetPrimesCountAsync(i * 1_000_000 + 2, 1_000_000);
                    _primeNumberCache[key] = nrOfPrimes;
                }

                //Regardless if nrOfPrimes where in the cache - I now have it in nrOfPrimes
                var t = $"{nrOfPrimes} primes between {start} and {start + count}";
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
            //Instantiating PrimeService so the Cache is valid for as long as the instance is on the heap
            var PrimesService = new PrimeNumberService();
            
            Console.WriteLine("\nInvoking DisplayPrimeCountsAsync - first time cache is empty");
            await PrimesService.DisplayPrimeCountsAsync();

            Console.WriteLine("\nInvoking DisplayPrimeCountsAsync - Now from cache - check speed difference");
            await PrimesService.DisplayPrimeCountsAsync();

            Console.WriteLine("\nInvoking DisplayPrimeCountsAsync - and a third time from cache");
            await PrimesService.DisplayPrimeCountsAsync();
        }
    }
}
