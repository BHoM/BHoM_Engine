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

namespace Diffing_Engine
{
    internal static partial class TestDiffing
    {
        public static void Example()
        {
            // 1. Suppose Alessio is creating 5 bars in Grasshopper, representing a Portal frame structure.
            // These will be Alessio's "Current" objects.
            List<IBHoMObject> currentObjs_Alessio = new List<IBHoMObject>();

            for (int i = 0; i < 5; i++)
            {
                Bar obj = BH.Engine.Base.Create.RandomObject(typeof(Bar)) as Bar;
                obj.Fragments = obj.Fragments.Where(fragm => fragm != null).ToList(); // (RandomObject bug workaround: it generates a random number of null fragments)
                currentObjs_Alessio.Add(obj as dynamic);
            }

            // 2. Alessio wants these bars to be part of a "Portal frame Project" that will be tracking the objects for future changes.
            // (The "Project" is what Speckle calls STREAM).
            // Alessio uses the "Diffing" component to create a new Project.
            // The "Diffing" component will attach a `Diffing` fragment to the objects, so for next push that is not needed.
            // Alessio only inputs his current objects in the first input. 
            // The second input is for the "Read" objects (the existing ones) that do not exist, as the Project is new,
            // so Alessio does not pass anything as a second argument.
            Delta delta = Diffing_Engine.Compute.Diffing("Portal frame Project", currentObjs_Alessio);

            // The delta now contains the new objects in the property `delta.ToCreate`.
            // These objects have an hash memorized in their Diffing Fragment that will be used to track their changes.

            // 3. Imagine that Alessio takes the `delta` 
            // and PUSHES it to some external platform (a database, Speckle, 3D repo...).
            // E.g. someAdapter.Push(delta);
            // (The Push will have to allow a Delta obj as input and take care of calling the appropriate methods for its properties: create, update or delete).

            // 4. Eduardo is now asked to do some changes to the "Portal frame Project" created by Alessio.
            List<IBHoMObject> currentObjs_Eduardo = new List<IBHoMObject>();
            List<IBHoMObject> readObjs_Eduardo = new List<IBHoMObject>();

            // 5. On his machine, Eduardo now PULLS the data from the external platform to read the existing objects.
            // E.g. readObjs = someAdapter.Pull("Portal frame Project");
            // (For illustration I simply take the `delta.ToCreate` objects and copy them in readObjs_Eduardo)
            readObjs_Eduardo = delta.ToCreate.Select(obj => obj as IBHoMObject).ToList();
            
            // Eduardo will now work on the objects output from the Pull component, 
            // which will be a deep copy of the readObjs, in order to preserve immutability.
            currentObjs_Eduardo = readObjs_Eduardo.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();

            // 6. Eduardo now modifies one of the bars, deletes another one, and creates a new one. Left are only 2 unchanged bars.
            currentObjs_Eduardo[0].Name = "modifiedBar"; // modifies this bar

            currentObjs_Eduardo.RemoveAt(1); //deletes this bar

            Bar newBar = BH.Engine.Base.Create.RandomObject(typeof(Bar)) as Bar;
            newBar.Name = "newBar";
            currentObjs_Eduardo.Add(newBar as dynamic); //adds this bar 

            // 7. Eduardo now wants to Push his changes.
            // He has two choices:
            //     7a. Use the diffing component to calculate the delta between his objects and the objects he pulled, 
            //         then input the Delta result in the Push component.
            //     7b. Just input his `currentObjs_Eduardo` into the Push component.
            //         The Push component determines automatically that he should be calculating the diffing for those objects
            //         because those objects have a `Diffing` fragment.
            // (This choice makes the use of the "Diffing component" required only when creating the project for the first time, or when clashes happen).
            Delta delta2 = Diffing_Engine.Compute.Diffing(currentObjs_Eduardo, readObjs_Eduardo);

            // 8. Now Eduardo can push his new delta object (like step 3).
            // `delta.ToCreate` will have 1 object; `delta.ToUpdate` 1 object; `delta.ToDelete` 1 object; `delta.Unchanged` 2 objects.
        }

        private static List<object> GenerateObjects()
        {
            List<object> objs = new List<object>();
            List<string> listStringHashes = new List<string>();


            Bar bar = new Bar();
            Bar bar1 = new Bar();

            bar.Name = "bar";
            objs.Add(bar);
            bar1.Name = "bar1";
            objs.Add(bar1);


            //objs.Add(BH.Engine.Base.Create.RandomObject(typeof(BH.oM.Geometry.Point)));

            //objs.Add(BH.Engine.Base.Create.RandomObject(typeof(Bar)));
            //objs.Add(BH.Engine.Base.Create.RandomObject(typeof(Bar)));

            return objs;
        }

        private static List<IBHoMObject> GenerateIBHoMObjects()
        {
            return GenerateObjects().Select(obj => obj as BH.oM.Base.IBHoMObject).Where(obj => obj != null).ToList();
        }
    }
}
