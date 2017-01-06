using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace PrimeNumbers
{
    class Program
    {
        public delegate void OnTimedEvent(long maxPrime);
      

        static void Main(string[] args)
        {
            Console.WriteLine("Started");
            int timerSec = 0;
            OnTimedEvent ote;

           
            
            ote = OutPutMaxPrime;

            PrimeGenerator generator = new PrimeGenerator();
            foreach (long value in generator)
            {
                
                Thread.Sleep(1000);
                timerSec++;
                if (timerSec == 30)
                {
                    ote(value);
                }
                
            }
            
           
        }


        static void OutPutMaxPrime(long maxPrime)
        {
            var pathToLogFile = Path.Combine(Directory.GetCurrentDirectory(), "Log.txt");
            File.AppendAllText(pathToLogFile, maxPrime + Environment.NewLine);
            Console.WriteLine("30 sec left");
            Environment.Exit(0);
        }

       

    }
    public class PrimeGenerator : IEnumerable<long>
    {
        // Return true if the value is prime.
        private bool IsOddPrime(long value)
        {
            long sqrt = (long)Math.Sqrt(value);
            for (long i = 3; i <= sqrt; i += 2)
                if (value % i == 0) return false;

            // If we get here, value is prime.
            return true;
        }

        public IEnumerator<long> GetEnumerator()
        {
            // Start with 2.
            yield return 2;

            // Generate odd primes.
            for (long i = 3; ; i += 2)
                if (IsOddPrime(i)) yield return i;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
