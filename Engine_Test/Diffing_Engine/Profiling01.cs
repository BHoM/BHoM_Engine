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
            int totalObjs = 100000;

            Console.WriteLine($"Profiling diffing for {totalObjs} randomly generated and modified objects.");

            // Generate random objects
            List<IBHoMObject> currentObjs = GenerateRandomObjects(typeof(Bar), totalObjs);

            // Assign diffing properties to the original objects
            var delta = Diffing_Engine.Compute.Diffing("Profiling01", currentObjs);

            // Modify randomly half the total of objects.
            var readObjs = delta.ToCreate;

            var allIdxs = Enumerable.Range(0, currentObjs.Count).ToList();
            var randIdxs = allIdxs.OrderBy(g => Guid.NewGuid()).Take(currentObjs.Count / 2).ToList();
            var remainingIdx = allIdxs.Except(randIdxs).ToList();

            List<IBHoMObject> changedList = randIdxs.Select(idx => readObjs.ElementAt(idx)).ToList();
            changedList.ForEach(obj => obj.Name = "ModifiedName");
            changedList.AddRange(remainingIdx.Select(idx => readObjs.ElementAt(idx)).ToList());

            // Actual diffing
            var timer = new Stopwatch();
            timer.Start();

            var delta2 = Diffing_Engine.Compute.Diffing(changedList, readObjs);

            timer.Stop();
            var ms = timer.ElapsedMilliseconds;

            Console.WriteLine($"Total elapsed milliseconds: {ms}");

            Debug.Assert(delta2.ToUpdate.Count == totalObjs / 2, "Diffing didn't work.");
            Debug.Assert(delta2.Unchanged.Count == totalObjs / 2, "Diffing didn't work.");
        }

    }
}
