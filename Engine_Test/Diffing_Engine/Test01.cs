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
using BH.Engine.Diffing;
using System.Diagnostics;
using BH.oM.Diffing;

namespace Engine_Test
{
    internal static partial class TestDiffing
    {
        public static void Test01(bool propertyLevelDiffing = true)
        {
            Console.WriteLine("Running Diffing_Engine Test01");

            // 1. Suppose Alessio is creating 5 bars in Grasshopper, representing a Portal frame structure.
            // These will be Alessio's "Current" objects.
            List<IBHoMObject> currentObjs_Alessio = new List<IBHoMObject>();

            for (int i = 0; i < 5; i++)
            {
                Bar obj = BH.Engine.Base.Create.RandomObject(typeof(Bar)) as Bar;
                obj.Fragments = obj.Fragments.Where(fragm => fragm != null).ToList(); // (RandomObject bug workaround: it generates a random number of null fragments)
                obj.Name = "bar_" + i.ToString();
                currentObjs_Alessio.Add(obj as dynamic);
            }

            // 2. Alessio wants these bars to be part of a "Portal frame Stream" that will be tracking the objects for future changes.
            // Alessio does the Push and specifies a StreamName in there.
            string streamName = "Portal Frame Stream";

            // The Push will take care of creating a Stream fragment that will be appended to the objects.
            BH.oM.Diffing.Stream diffStream = new BH.oM.Diffing.Stream(streamName);

            // The Push will calculate and set the hashes for the objects.
            var currentObjs_Alessio_withHashes = BH.Engine.Diffing.Compute.DiffHash(currentObjs_Alessio, null, true, diffStream);

            // The push will now send the objects to the online stream.

            // 3. Eduardo is now asked to do some changes to the "Portal frame Project" created by Alessio.
            List<IBHoMObject> currentObjs_Eduardo = new List<IBHoMObject>();
            List<IBHoMObject> readObjs_Eduardo = new List<IBHoMObject>();

            // 4. On his machine, Eduardo now PULLS the data from the external platform to read the existing objects.
            // E.g. readObjs = someAdapter.Pull("Portal frame Project");
            // (For illustration I simply take the `currentObjs_Alessio_withHashes` objects and copy them in readObjs_Eduardo)
            readObjs_Eduardo = currentObjs_Alessio_withHashes.ToList();

            // Eduardo will now work on the objects output from the Pull component, 
            // which will be a deep copy of the readObjs, in order to preserve immutability.
            currentObjs_Eduardo = readObjs_Eduardo.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();

            // 5. Eduardo now modifies one of the bars, deletes another one, and creates a new one. Left are only 2 unchanged bars.
            currentObjs_Eduardo[0].Name = "modifiedBar_0"; // modifies bar_0

            currentObjs_Eduardo.RemoveAt(1); //deletes bar_1

            Bar newBar = BH.Engine.Base.Create.RandomObject(typeof(Bar)) as Bar;
            newBar.Name = "newBar_1";
            currentObjs_Eduardo.Insert(1, newBar as dynamic); //adds this bar 

            // 6. Eduardo now wants to Push his changes.
            // He has two choices:
            //     6a. Use the diffing component to calculate the delta between his objects and the objects he pulled, 
            //         then input the Delta result in the Push component.
            //     6b. Just input his `currentObjs_Eduardo` into the Push component.
            //         The Push component determines automatically that he should be calculating the diffing for those objects
            //         because those objects have a `Diffing` fragment.
            // (This choice makes the use of the "Diffing component" required only when creating the project for the first time, or when clashes happen).
            Delta delta2 = Compute.Diffing(currentObjs_Eduardo, readObjs_Eduardo, propertyLevelDiffing);

            // 7. Now Eduardo can push his new delta object (like step 3).
            // `delta.ToCreate` will have 1 object; `delta2.ToUpdate` 1 object; `delta2.ToDelete` 1 object; `delta2.Unchanged` 2 objects.
            // You can also see which properties have changed for what objects: check `delta2.ModifiedPropsPerObject`.
            Debug.Assert(delta2.OnlySetA.Count == 1, "Incorrect number of object identified as new/ToBeCreated.");
            Debug.Assert(delta2.Modified.Count == 1, "Incorrect number of object identified as modified/ToBeUpdated.");
            Debug.Assert(delta2.OnlySetB.Count == 1, "Incorrect number of object identified as old/ToBeDeleted.");
            var modifiedPropsPerObj = delta2.ModifiedPropsPerObject.First().Value;
            Debug.Assert(modifiedPropsPerObj.Count == 1, "Incorrect number of changed properties identified by the property-level diffing.");
            Debug.Assert(modifiedPropsPerObj.First().Key == "Name", "Error in property-level diffing");
            Debug.Assert(modifiedPropsPerObj.First().Value.Item1 as string == "modifiedBar_0", "Error in property-level diffing");

            Console.WriteLine("Test01 concluded.");
        }

    }
}
