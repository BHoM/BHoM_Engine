using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine_Explore.BHoM.Base;
using Engine_Explore.BHoM.Geometry;
using Engine_Explore.Engine.Geometry;
using BHG = BHoM.Geometry;
using Engine_Explore.Engine;
using System.Diagnostics;
using System.Threading;
using Engine_Explore.BHoM.Structural.Elements;
using Engine_Explore.Adapter;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using MongoDB.Bson;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Engine_Explore
{
    class A {

        public A(double gVal = 0, double pVal = 0) {
            getProp = gVal;
            privateField = pVal;
        }

        public double a { get; set; }
        public double publicField;
        private double privateField;
        public double getProp { get; private set; }
    }
    class B: A { public double b { get; set; } }
    class C: A { public double c { get; set; } }
    class D : B { public double d { get; set; } }
    class E : C { public double e { get; set; } }


    class Program
    {
        static void Main(string[] args)
        {
            TestBson();
            
            Console.Read();
        }

        /***************************************************/
        /**** Test Functions                            ****/
        /***************************************************/

        public static void TestBson()
        {
            List<BHoMObject> nodes = new List<BHoMObject>
            {
                new Node {Point = new Point(1, 2, 3), Name = "A"},
                new Node {Point = new Point(4, 5, 6), Name = "B"},
                new Node {Point = new Point(7, 8, 9), Name = "C"}
            };


            List<object> items = new List<object>
            {
                new A (-6, -7) { a = 1, publicField = -4 },
                new B { a = 2, b = 45 },
                new C { a = 3, c = 56 },
                new D { a = 4, b = 67, d = 123 },
                new E { a = 5, c = 78, e = 456 },
                new Node {Point = new Point(1, 2, 3), Name = "A"},
                nodes,
                new Dictionary<string, A> {
                    { "A",  new A { a = 1 } },
                    { "C",  new C { a = 3, c = 56 } },
                    { "E",  new E { a = 5, c = 78, e = 456 } }
                }
            };

            List<BsonDocument> docs = items.Select(x => x.ToBsonDocument()).ToList();
            List<object> items2 = docs.Select(x => BsonSerializer.Deserialize(x, typeof(object))).ToList();

            foreach (BsonDocument doc in docs)
            {
                Console.WriteLine(doc.ToJson());
                Console.WriteLine();
            }

            string outputFileRoot = @"C:\Users\adecler\Documents\"; // initialize to the file to write to.
            File.WriteAllLines(@"C:\Users\adecler\Documents\json_Save.txt", docs.Select(x => x.ToJson()));

            FileStream mongoStream = new FileStream(outputFileRoot + "bsonSave_Mongo.txt", FileMode.Create);
            var writer = new BsonBinaryWriter(mongoStream);
            BsonSerializer.Serialize(writer, typeof(object), docs);
            mongoStream.Flush();
            mongoStream.Close();

            FileStream csharpStream = new FileStream(outputFileRoot + "bsonSave_CSharp.txt", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(csharpStream, docs);
            csharpStream.Close();

            FileStream mongoReadStream = File.OpenRead(outputFileRoot + "bsonSave_Mongo.txt");
            var reader = new BsonBinaryReader(mongoReadStream);
            List<BsonDocument> readBson = BsonSerializer.Deserialize(reader, typeof(object)) as List<BsonDocument>;
            List<object> items3 = readBson.Select(x => BsonSerializer.Deserialize(x, typeof(object))).ToList();

            // Directly writing and reading objects to the stream using Bson serializer seems to have a problem when reading back
            //FileStream objectStream = new FileStream(outputFileRoot + "objectSave_Mongo.txt", FileMode.Create);
            //var objectWriter = new BsonBinaryWriter(objectStream);
            //BsonSerializer.Serialize(objectWriter, typeof(List<object>), items);
            //objectStream.Flush();
            //objectStream.Close();

            //FileStream objectReadStream = File.OpenRead(outputFileRoot + "objectSave_Mongo.txt");
            //var objectReader = new BsonBinaryReader(objectReadStream);
            //var readObject = BsonSerializer.Deserialize(reader, typeof(object));


            Console.WriteLine("Done!");

        }

        /***************************************************/

        public static void TestGsaAdapter()
        {
            List<BHoMObject> nodes = new List<BHoMObject>
            {
                new Node {Point = new Point(1, 2, 3), Name = "A"},
                new Node {Point = new Point(4, 5, 6), Name = "B"},
                new Node {Point = new Point(7, 8, 9), Name = "C"}
            };

            //IEnumerable<object> test = nodes as IEnumerable<object>;
            //IEnumerable<Node> n = (IEnumerable<Node>)test;

            CastFunction(nodes);

            //List<object> test = nodes as List<object>;
            //List<Node> n = (List<Node>)test;

            IAdapter adapter = new GsaAdapter(@"C:\Users\adecler\Documents\My Received Files\Gsa54.gwb");

            adapter.Push(nodes);

            List<Node> pulledNodes = (List<Node>)adapter.Pull("Node");
        }

        public static void CastFunction(IEnumerable<object> objects)
        {
            List<Node> n = objects as List<Node>;
        } 

        /***************************************************/

        public void TestOldVsNew()
        {
            Console.WriteLine("\nChanging to 20 Curves...");
            ChangeCurveLength(20);
            TestSpeed(new List<IterFunction>() { IntersectOldLines, IntersectNewLines });

            Console.WriteLine("\nChanging to 100 Curves...");
            ChangeCurveLength(100);
            TestSpeed(new List<IterFunction>() { IntersectOldLines, IntersectNewLines });

            Console.WriteLine("\nChanging to 500 Curves...");
            ChangeCurveLength(500);
            TestSpeed(new List<IterFunction>() { IntersectOldLines, IntersectNewLines });


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
            BHoMGeometry pt = new Point();


            BoundingBox box = Bound.Calculate(pt as dynamic);

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
