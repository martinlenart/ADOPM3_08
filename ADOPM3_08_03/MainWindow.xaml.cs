using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Threading;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace ADOPM3_08_03
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
        public Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() =>
               ParallelEnumerable.Range(start, count).Count(n =>
                 Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        }
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            myCounter.Content = (int.Parse((string)myCounter.Content) + 1).ToString();
        }
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            myGreetings.Text = "";
            int nrwords = await new IOBoundAsync().GetDotNetCountAsync();
            myGreetings.Text = nrwords.ToString();
        }
        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            myGreetings.Text = "";
            int nrPrimes = await new CPUBoundAsync().GetPrimesCountAsync(2, 10_000_000);
            myGreetings.Text = nrPrimes.ToString();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //Write your stream here in async/await pattern
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //Read your stream here in async/await pattern
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            //This is where you would do Exercise 10_05
        }
    }
    //Exercise:
    //1.    Use asyc/await pattern to write the nr of primes to a text file using StreamWriter. Nr of primes from 0 to 9999999 as done in
    //      DisplayPrimeCountsAsync() in Example 10_01. Once written, update myGreetings.Text to "File Written"
}
