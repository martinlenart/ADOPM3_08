﻿using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADOPM3_08_04
{
    internal class CPUBoundAsync
    {
        public Task DisplayPrimeCountsAsync(IProgress<string> onProgressReporting)
        {
            //Notice I can use async in Lambda Expression
            return Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
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

            //Define my progressReporter as an instance of Progress which implements IProgress
            var progressReporter = new Progress<string>(value => Console.WriteLine(value));

            //Create and run the task, but passing the progressReporter as an argument
            var t1 = new CPUBoundAsync().DisplayPrimeCountsAsync(progressReporter);
            t1.Wait();
        }
    }
    //Exercise
    //1.    Implement progress reporting in Example 10_03 when writing to the stream (10%, 20%...)
    //      Notice that you can update myGreeting.Text in the progressReporter instance as creating a Progress object
    //      internally has the UI SyncronizationContext as the inistance was created in the UI context
}
