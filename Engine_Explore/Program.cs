using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine_Explore.BHoM.Geometry;
using Engine_Explore.Engine.Geometry;
using BHG = BHoM.Geometry;
using Engine_Explore.Engine;
using System.Diagnostics;
using System.Threading;

namespace Engine_Explore
{
    class Program
    {
        static void Main(string[] args)
        {
            BHG.Point pt = new BHG.Point();

            Console.WriteLine("\nChanging Curve length to 20 points...");
            ChangeCurveLength(20);
            TestSpeed(new List<IterFunction>() { IntersectOldLines, IntersectNewLines });

            Console.WriteLine("\nChanging Curve length to 100 points...");
            ChangeCurveLength(100);
            TestSpeed(new List<IterFunction>() { IntersectOldLines, IntersectNewLines });

            Console.WriteLine("\nChanging Curve length to 500 points...");
            ChangeCurveLength(500);
            TestSpeed(new List<IterFunction>() { IntersectOldLines, IntersectNewLines });

            /*
            TestSpeed(new List<IterFunction>() { BoundOldNurbCurve, BoundNewNurbCurve, BoundNewNurbCurveB });

            Console.WriteLine("\nChanging Curve length to 20 points...");
            ChangeCurveLength(20);
            TestSpeed(new List<IterFunction>() { BoundOldNurbCurve, BoundNewNurbCurve, BoundNewNurbCurveB });

            Console.WriteLine("\nChanging Curve length to 50 points...");
            ChangeCurveLength(50);
            TestSpeed(new List<IterFunction>() { BoundOldNurbCurve, BoundNewNurbCurve, BoundNewNurbCurveB });


            Console.WriteLine("\nChanging Curve length to 600 points...");
            ChangeCurveLength(600);
            TestSpeed(new List<IterFunction>() { BoundOldNurbCurve, BoundNewNurbCurve, BoundNewNurbCurveB });
            */

            Console.Read();
        }

        /***************************************************/
        /**** Iter Functions                            ****/
        /***************************************************/

        public delegate void IterFunction(int x);

        /***************************************************/

        static void BoundOldPoint(int iter)
        {
            new BHG.Point(0, 0, iter).Bounds();
        }

        /***************************************************/

        static void BoundNewPoint(int iter)
        {
            Bound.Calculate(new Point(0, 0, iter));
        }

        /***************************************************/

        static void BoundOldLine(int iter)
        {
            new BHG.Line(new BHG.Point(0, 0, iter), new BHG.Point(0, 0, iter)).Bounds();
        }

        /***************************************************/

        static void BoundNewLine(int iter)
        {
            Bound.Calculate(new Line(new Point(0, 0, -iter), new Point(0, 0, iter)));
        }

        /***************************************************/

        static List<BHG.Point> oldControlPoints = new List<BHG.Point>
        {
            new BHG.Point(0, 0, 0),
            new BHG.Point(0, 1, 0),
            new BHG.Point(1, 1, 0),
            new BHG.Point(1, 0, 0)
        };

        static double[] oldNurbsWeights = new double[] { 1, 1, 1, 1 };
        static double[] oldNurbsKnots = new double[] { 0, 0, 0, 1, 1, 1 };
        static BHG.NurbCurve oldNurbsCurve = new BHG.NurbCurve(oldControlPoints, 3, oldNurbsKnots, oldNurbsWeights);

        /***************************************************/

        static void BoundOldNurbCurve(int iter)
        {
            new BHG.NurbCurve(oldControlPoints, 3, oldNurbsKnots, oldNurbsWeights).Bounds();
            //oldNurbsCurve.Bounds();
        }

        /***************************************************/

        static List<Point> newControlPoints = new List<Point>
        {
            new Point(0, 0, 0),
            new Point(0, 1, 0),
            new Point(1, 1, 0),
            new Point(1, 0, 0)
        };

        static List<double> newNurbsWeights = new List<double> { 1, 1, 1, 1 };
        static List<double> newNurbsKnots = new List<double> { 0, 0, 0, 1, 1, 1 };
        static NurbCurve newNurbsCurve = new NurbCurve(newControlPoints, newNurbsWeights, newNurbsKnots);
        static NurbCurveB newNurbsCurveB = new NurbCurveB(newControlPoints, newNurbsWeights, newNurbsKnots);

        /***************************************************/

        static void ChangeCurveLength(int nbPts)
        {
            Random rnd = new Random();

            List<Point> points = new List<Point>();
            for (int i = 0; i < nbPts; i++)
                points.Add(new Point(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble()));

            newControlPoints = points;
            newNurbsCurve = new NurbCurve(points);
            newNurbsCurveB = new NurbCurveB(points);

            oldControlPoints = points.Select(pt => new BHG.Point(pt.X, pt.Y, pt.Z)).ToList();
            oldNurbsWeights = newNurbsCurve.Weights.ToArray();
            oldNurbsKnots = newNurbsCurve.Knots.ToArray();
            oldNurbsCurve = new BHG.NurbCurve(oldControlPoints, 3, oldNurbsKnots, oldNurbsWeights);
        }

        /***************************************************/

        static void BoundNewNurbCurve(int iter)
        {
            Bound.Calculate(new NurbCurve(newControlPoints, newNurbsWeights, newNurbsKnots));
            //Bound.Calculate(newNurbsCurve);
        }

        /***************************************************/

        static void BoundNewNurbCurveB(int iter)
        {
            Bound.Calculate(new NurbCurve(newControlPoints, newNurbsWeights, newNurbsKnots));
            //Bound.Calculate(newNurbsCurveB);
        }

        /***************************************************/

        static void BoundNewNurbCurve2(int iter)
        {
            //Bound.Calculate2(new NurbCurve(newControlPoints, newNurbsWeights, newNurbsKnots));
            Bound.Calculate2(newNurbsCurve);
        }

        /***************************************************/

        static void BoundNewNurbCurve3(int iter)
        {
            //Bound.Calculate3(new NurbCurve(newControlPoints, newNurbsWeights, newNurbsKnots));
            Bound.Calculate3(newNurbsCurve);
        }

        /***************************************************/

        static void BoundNewNurbCurve4(int iter)
        {
            //Bound.Calculate4(new NurbCurve(newControlPoints, newNurbsWeights, newNurbsKnots));
            Bound.Calculate4(newNurbsCurve);
        }

        /***************************************************/

        static BHG.Line oldLine1 = new BHG.Line(new BHG.Point(1.1, 1.2, 0.4), new BHG.Point(4.2, 4.1, 0.4));
        static BHG.Line oldLine2 = new BHG.Line(new BHG.Point(1.1, 3.2, 0.4), new BHG.Point(4.2, 2.1, 0.4));
        static Line newLine1 = new Line(new Point(1.1, 1.2, 0.4), new Point(4.2, 4.1, 0.4));
        static Line newLine2 = new Line(new Point(1.1, 3.2, 0.4), new Point(4.2, 2.1, 0.4));

        /***************************************************/

        static void IntersectOldLineLine(int iter)
        {
            int i = iter % (oldControlPoints.Count - 3);
            //BHG.Intersect.LineLine(new BHG.Line(oldControlPoints[i], oldControlPoints[i + 1]), new BHG.Line(oldControlPoints[i + 2], oldControlPoints[i + 3]));
            BHG.Intersect.LineLine(oldLine1, oldLine2);
        }

        /***************************************************/

        static void IntersectNewLineLine(int iter)
        {
            int i = iter % (newControlPoints.Count - 3);
            //Intersect.LineLine(new Line(newControlPoints[i], newControlPoints[i + 1]), new Line(newControlPoints[i + 2], newControlPoints[i + 3]));
            Intersect.LineLine(newLine1, newLine2);
        }

        /***************************************************/

        static void IntersectOldLines(int iter)
        {
            List<BHG.Line> lines = new List<BHG.Line>();
            for (int i = 0; i < oldControlPoints.Count - 1; i++)
                lines.Add(new BHG.Line(oldControlPoints[i], oldControlPoints[i + 1]));

            for (int i = lines.Count - 1; i >= 0; i--)     // We should use an octoTree/point matrix instead of using bounding boxes
                for (int j = lines.Count - 1; j > i; j--)
                    BHG.Intersect.LineLine(lines[i], lines[j]);
        }

        /***************************************************/

        static void IntersectNewLines(int iter)
        {
            List<Line> lines = new List<Line>();
            for (int i = 0; i < newControlPoints.Count - 1; i++)
                lines.Add(new Line(newControlPoints[i], newControlPoints[i + 1]));
            Intersect.Lines(lines);
        }

        /***************************************************/
        /**** Speed Test                                ****/
        /***************************************************/

        static void TestSpeed(List<IterFunction> functions, int testTime = 2000)
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
            foreach( IterFunction function in functions)
            {
                count = 0;
                Console.WriteLine("Test Method " + function.Method.Name + "...");
                stopwatch.Reset();
                stopwatch.Start();
                while (stopwatch.ElapsedMilliseconds < testTime)
                {
                    function.Invoke(count);
                    count++;
                }
                stopwatch.Stop();
                counts.Add(count);
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
            Console.WriteLine("\nWinner: " + functions[maxIndex].Method.Name);
        }

        /***************************************************/

        static void TestDynamicPolymorphism()
        {
            List<dynamic> stuff = new List<dynamic>();
            BHoMGeometry pt = new Point();

            BoundingBox box = Bound.Calculate(pt);

        }

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


        
    }
}
