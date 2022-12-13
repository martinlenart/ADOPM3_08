using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace ADOPM3_08_06
{
    public class PrimeBatch
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
        static ConcurrentDictionary<(int, int), PrimeBatch> _primeNumberCache = new ConcurrentDictionary<(int, int), PrimeBatch>();
        
        //Add an event to indicate cache is filled
        static public event EventHandler<PrimeBatch> CacheFilled; //Broadcaster event
        static protected void OnCacheFilled(PrimeBatch e)
        {
            CacheFilled?.Invoke(null, e); // Invoke if not null
        }

        static public async Task DisplayPrimeCountsAsync()
        {
            for (int i = 0; i < 3; i++)
            {
                //Create my Cache Key
                int start = i * 1_000_000 + 2;
                int count = 1_000_000;

                //Without Cache
                //PrimeBatch pResponse = new PrimeBatch { start = start, count = count };
                //pResponse.NrofPrimes = await GetPrimesCountAsync(start, count);


                //With Cache
                //pSuite is null as I first want to check if it exists in the cache
                PrimeBatch pResponse = null;

                //I need a unique Key that represents one calculation value
                var key = (start, count);

                //Check if Cache already contains the value
                if (!_primeNumberCache.TryGetValue(key, out pResponse))
                {
                    //the value is not in the cache - get the value the slow way 
                    pResponse = new PrimeBatch { start = start, count = count };

                    //It did not exist in the cache, calculate and add it to the cache
                    pResponse.NrofPrimes = await GetPrimesCountAsync(start, count);

                    _primeNumberCache[key] = pResponse;
                    
                    //serialize the response and serialize to disk
                    await SerializeCacheAsync(pResponse);

                    //fire the event
                    OnCacheFilled(pResponse);

                }

                //Regardless if nrOfPrimes where in the cache - I now have it in nrOfPrimes
                var t = $"{pResponse.NrofPrimes} primes between {pResponse.start} and {pResponse.start + pResponse.count}";
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

        static public Task SerializeCacheAsync(PrimeBatch batch) {
            return Task.Run(() => SerializeCache(batch));}

        static public void SerializeCache (PrimeBatch batch)
        {
           var xs = new XmlSerializer(typeof(PrimeBatch));

            using (Stream s = File.Create(fname($"PrimeNumbers_from_{batch.start}_to_{batch.start+batch.count}.xml")))
                xs.Serialize(s, batch);
        }
        static string fname(string name)
        {
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            documentPath = Path.Combine(documentPath, "Nisse");
            if (!Directory.Exists(documentPath)) Directory.CreateDirectory(documentPath);
            return Path.Combine(documentPath, name);
        }

    }
    class Program
    {
        public static async Task Main()
        {           
            Console.WriteLine("\nInvoking DisplayPrimeCountsAsync - first time cache is empty");
            PrimeNumberService.CacheFilled += ReportCacheFilled;
            
            await PrimeNumberService.DisplayPrimeCountsAsync();

            Console.WriteLine("\nInvoking DisplayPrimeCountsAsync - Now from cache - check speed difference");
            await PrimeNumberService.DisplayPrimeCountsAsync();

            Console.WriteLine("\nInvoking DisplayPrimeCountsAsync - and a third time from cache");
            await PrimeNumberService.DisplayPrimeCountsAsync();
        }

        static void ReportCacheFilled(object sender, PrimeBatch e) //Subscriber eventhandler implementation
        {
            Console.WriteLine($"Cache filled: start:{e.start}, count: {e.count}, NrOfPrimes: {e.NrofPrimes}");
        }
    }
}
