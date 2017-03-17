using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine_Explore.Geometry;
using Engine_Explore.Engine;
using System.Diagnostics;
using System.Threading;

namespace Engine_Explore
{
    class Program
    {
        static void Main(string[] args)
        {
            TestSpeed();

            Console.Read();
        }


        static void TestSpeed()
        {
            int testTime = 2000;

            Stopwatch stopwatch = new Stopwatch();
            long seed = Environment.TickCount; 	// Prevents the JIT Compiler 
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2); // Use only the second core 
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            //Warm up 
            Console.WriteLine("Warming up...");
            int count = 0;
            stopwatch.Reset();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < 1200)  // A Warmup of 1000-1500 mS stabilizes the CPU cache and pipeline.                
                WarmUpFunction(seed, count++); // Warmup
            stopwatch.Stop();

            //Test function 1 
            Console.WriteLine("Test Method 1...");
            int count1 = 0;
            stopwatch.Reset();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < testTime)
            {
                new BHoM.Geometry.Point(0, 0, testTime).Bounds();
                count1++;
            }
            stopwatch.Stop();

            //Test function 2 
            Console.WriteLine("Test Method 2...");
            int count2 = 0;
            stopwatch.Reset();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < testTime)
            {
                Bounds.Calculate(new Point(0, 0, testTime));
                count2++;
            }
            stopwatch.Stop();

            Console.WriteLine("count1 / count2 = {0}", ((double)count1)/count2);
            if (count1 < count2)
                Console.WriteLine("Method 2 wins!");
            else
                Console.WriteLine("Method 1 wins!");
        }

        static void TestPointSpeed()
        {
            List<dynamic> stuff = new List<dynamic>();
            BHoMGeometry pt = new Point();

            BoundingBox box = Bounds.Calculate(pt);

        }


        static long WarmUpFunction(long seed, int count)
        {
            long result = seed;
            for (int i = 0; i < count; ++i)
            {
                result ^= i ^ seed; // Some useless bit operations
            }
            return result;
        }
    }
}
