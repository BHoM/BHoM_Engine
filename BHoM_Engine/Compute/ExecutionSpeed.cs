/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Base.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<int> ExecutionSpeed(this List<IterFunction> functions, int testTime = 2000)
        {
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
            while (stopwatch.ElapsedMilliseconds < 1500)  // A Warmup of 1000-1500 mS stabilizes the CPU cache and pipeline.                
                WarmUpFunction(seed, count++); // Warmup
            stopwatch.Stop();

            List<int> counts = new List<int>();
            foreach (IterFunction function in functions)
            {
                count = 0;
                Console.Write("Test Method " + function.Method.Name + "... ");
                stopwatch.Reset();
                stopwatch.Start();
                while (stopwatch.ElapsedMilliseconds < testTime)
                {
                    function.Invoke(count);
                    count++;
                }
                stopwatch.Stop();
                counts.Add(count);
                Console.Write(count.ToString() + " iteration\n");
            }

            int maxIndex = 0;
            double maxCount = 0;
            for (int i = 0; i < counts.Count; i++)
            {
                if (counts[i] > maxCount)
                {
                    maxCount = counts[i];
                    maxIndex = i;
                }
            }

            List<double> ratios = counts.Select(x => ((double)x) / maxCount).ToList();

            for (int i = 0; i < counts.Count; i++)
            {
                Console.WriteLine("Count ratio: " + ratios[i].ToString("N3") + " -> \t Method " + functions[i].Method.Name);
            }
            Console.WriteLine("\nWinner: " + functions[maxIndex].Method.Name+"\n");

            return counts;
        }


        /***************************************************/
        /**** Private Computation Methods               ****/
        /***************************************************/

        static long WarmUpFunction(long seed, int count)
        {
            long result = seed;
            for (int i = 0; i < count; ++i)
            {
                result ^= i ^ seed; // Some useless bit operations
            }
            return result;
        }

        /***************************************************/
    }
}




