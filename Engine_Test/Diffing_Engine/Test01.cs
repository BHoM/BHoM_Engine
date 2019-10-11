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
using System.Security.Cryptography;
using BH.Engine.Diffing;
using System.Diagnostics;
using BH.oM.Diffing;
using BH.Engine.Diffing;

namespace Engine_Test
{
    internal static partial class TestDiffing
    {
        public static void Test01(bool propertyLevelDiffing = true)
        {
            Console.WriteLine("Running Diffing_Engine Test01");

            // 1. Suppose Alessio is creating 3 bars in Grasshopper, representing a Portal frame structure.
            // These will be Alessio's "Current" objects.
            List<IBHoMObject> currentObjs_Alessio = new List<IBHoMObject>();

            for (int i = 0; i < 3; i++)
            {
                Bar obj = BH.Engine.Base.Create.RandomObject(typeof(Bar)) as Bar;
                //obj.Fragments = obj.Fragments.Where(fragm => fragm != null).ToList(); // (RandomObject bug workaround: it generates a random number of null fragments)
                obj.Name = "bar_" + i.ToString();
                currentObjs_Alessio.Add(obj as dynamic);
            }

            // 2. Alessio wants these bars to be part of a "Portal frame Stream" that will be tracking the objects for future changes.
            // Alessio creates a stream
            string comment = "Portal Frame Stream";
            Stream stream_Alessio = Create.Stream(currentObjs_Alessio, comment); // this will add the hash fragments to the objects

            // Alessio can now push the Stream.

            // 3. Eduardo is now asked to do some changes to the "Portal frame Project" created by Alessio.
            // On his machine, Eduardo now PULLS the Stream from the external platform to read the existing objects.
            IEnumerable<IBHoMObject> readObjs_Eduardo = stream_Alessio.Objects.Select(obj => BH.Engine.Base.Query.DeepClone(obj) as IBHoMObject).ToList();

            // Eduardo will now work on these objects.
            List<IBHoMObject> currentObjs_Eduardo = readObjs_Eduardo.ToList();

            // 5. Eduardo now modifies one of the bars, deletes another one, and creates a new one. 
            // modifies bar_0
            currentObjs_Eduardo[0].Name = "modifiedBar_0";

            // deletes bar_1
            currentObjs_Eduardo.RemoveAt(1);

            // adds a new bar 
            Bar newBar = BH.Engine.Base.Create.RandomObject(typeof(Bar)) as Bar;
            newBar.Name = "newBar_1";
            currentObjs_Eduardo.Insert(1, newBar as dynamic);

            // 6. Eduardo updates the Stream Revision.
            Stream stream_Eduardo = Modify.StreamRevision(stream_Alessio, currentObjs_Eduardo);

            // Eduardo can now push this Stream.

            // -------------------------------------------------------- //

            // Eduardo can also manually check the differences.

            Delta delta = Compute.Diffing(stream_Alessio, stream_Eduardo, propertyLevelDiffing, null, true);

            // 7. Now Eduardo can push his new delta object (like step 3).
            // `delta.ToCreate` will have 1 object; `delta2.ToUpdate` 1 object; `delta2.ToDelete` 1 object; `delta2.Unchanged` 2 objects.
            // You can also see which properties have changed for what objects: check `delta2.ModifiedPropsPerObject`.
            Debug.Assert(delta.NewObjects.Count == 1, "Incorrect number of object identified as new/ToBeCreated.");
            Debug.Assert(delta.ModifiedObjects.Count == 1, "Incorrect number of object identified as modified/ToBeUpdated.");
            Debug.Assert(delta.OldObjects.Count == 1, "Incorrect number of object identified as old/ToBeDeleted.");
            var modifiedPropsPerObj = delta.ModifiedPropsPerObject.First().Value;
            Debug.Assert(modifiedPropsPerObj.Count == 1, "Incorrect number of changed properties identified by the property-level diffing.");
            Debug.Assert(modifiedPropsPerObj.First().Key == "Name", "Error in property-level diffing");
            Debug.Assert(modifiedPropsPerObj.First().Value.Item1 as string == "modifiedBar_0", "Error in property-level diffing");

            Console.WriteLine("Test01 concluded.");
        }

    }
}
