using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using BH.Engine.Serialiser;
using BH.oM.Diffing;
using System.Diagnostics;

namespace Engine_Test
{
    internal static partial class TestDiffing
    {
        public static void Profiling01()
        {
            Console.WriteLine("Running Diffing_Engine Profiling01");

            string path = @"C:\temp\Diffing_Engine-ProfilingTask01.txt";
            File.Delete(path);

            List<int> numberOfObjects = new List<int>() { 10, 100, 1000, 5000, 10000 }; //, 12250, 15000, 17250, 20000, 25000, 30000 };

            bool propertyLevelDiffing = false;
            for (int b = 0; b < 2; b++)
            {
                numberOfObjects.ForEach(i =>
                    ProfilingTask(i, propertyLevelDiffing, path));

                propertyLevelDiffing = !propertyLevelDiffing;
            }

            Console.WriteLine("Profiling01 concluded.");
        }

        public static void ProfilingTask(int totalObjs, bool propertyLevelDiffing, string path = null)
        {
            string introMessage = $"Profiling diffing for {totalObjs} randomly generated and modified objects.";
            introMessage += propertyLevelDiffing ? " Includes collection-level and property-level diffing." : " Only collection-level diffing.";
            Console.WriteLine(introMessage);

            if (path != null)
            {
                string fName = Path.GetFileNameWithoutExtension(path);
                string ext = Path.GetExtension(path);
                fName += propertyLevelDiffing ? "_propLevel" : "_onlyCollLevel";
                path = Path.Combine(Path.GetDirectoryName(path), fName + ext);
            }

            // Generate random objects
            List<IBHoMObject> currentObjs = GenerateRandomObjects(typeof(Bar), totalObjs);

            // Create Stream. This assigns the Hashes.
            var stream = BH.Engine.Diffing.Create.Stream(currentObjs, null);

            // Modify randomly half the total of objects.
            var readObjs = stream.Objects.Cast<IBHoMObject>().ToList();

            var allIdxs = Enumerable.Range(0, currentObjs.Count).ToList();
            var randIdxs = allIdxs.OrderBy(g => Guid.NewGuid()).Take(currentObjs.Count / 2);
            var remainingIdx = allIdxs.Except(randIdxs).ToList();

            List<IBHoMObject> changedList = randIdxs.Select(idx => readObjs.ElementAt(idx)).ToList();
            changedList.ForEach(obj => obj.Name = "ModifiedName");
            changedList.AddRange(remainingIdx.Select(idx => readObjs.ElementAt(idx)).Cast<IBHoMObject>().ToList());

            // Update stream revision
            BH.oM.Diffing.Stream updatedStream = BH.Engine.Diffing.Modify.StreamRevision(stream, changedList);

            // Actual diffing
            var timer = new Stopwatch();
            timer.Start();

            Delta delta = BH.Engine.Diffing.Compute.Diffing(stream, updatedStream, propertyLevelDiffing, null, true);

            timer.Stop();

            var ms = timer.ElapsedMilliseconds;

            string endMessage = $"Total elapsed milliseconds: {ms}";
            Console.WriteLine(endMessage);

            Debug.Assert(delta.ModifiedObjects.Count == totalObjs / 2, "Diffing didn't work.");

            if (path != null)
            {
                System.IO.File.AppendAllText(path, Environment.NewLine + introMessage + Environment.NewLine + endMessage);
                Console.WriteLine($"Results appended in {path}");
            }
        }

    }
}
