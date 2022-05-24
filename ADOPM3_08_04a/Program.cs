using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ADOPM3_08_04a
{
    internal class CPUBoundAsync
    {
        public Task DisplayPrimeCountsAsync(IProgress<string> onProgressReporting, CancellationToken cancellationToken)
        {
            //Notice I can use async in Lambda Expression
            return Task.Run(async () =>
            {
                for (int i = 0; i < 20 && !cancellationToken.IsCancellationRequested; i++)
                {
                    int nrprimes = await GetPrimesCountAsync(i * 1000000 + 2, 1000000);
                    onProgressReporting.Report($"{nrprimes} primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1));
                }
           });
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
            Console.WriteLine("Invoking DisplayPrimeCountsAsync");

            //Define a cancellation token to allow Task to cancel
            var cancellationSource = new CancellationTokenSource();
            var cancellationToken = cancellationSource.Token;

            //Define my progressReporter as an instance of Progress which implements IProgress
            //var count = 0;
            var progressReporter = new Progress<string>(value =>
            {
                Console.WriteLine(value);

                /*
                if (++count >= 5)
                {
                    cancellationSource.Cancel();
                }
                */
                
            });

            //Create and run the task, but passing the progressReporter as an argument
            var t1 = new CPUBoundAsync().DisplayPrimeCountsAsync(progressReporter, cancellationToken);

            Console.WriteLine("Q To terminate:");
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Q)
                cancellationSource.Cancel();

            t1.Wait();
        }
    }
}

//Exercises:
//1.     Modify the code in Main() so the Task cancellation is done after a user input, through
//       Console.ReadKey() instead of in the ProgressReporter.
//       Hint: The Console.ReadKey() and the task cancellation will have to be done
//       before the t1.Wait()
