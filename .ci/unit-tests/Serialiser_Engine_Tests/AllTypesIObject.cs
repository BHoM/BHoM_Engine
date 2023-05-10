using BH.oM.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Tests.Engine.Serialiser
{
    [Ignore("Ignoring NUnit versioning tests as no 100% clear action for every single class to be IObject. Usefull to be able to get this information out, but not required to be run automatically. Test kept for debugging purposes.")]
    public class AllTypesIObject : BaseLoader
    {
        [Test]
        public static void CheckAllTypes()
        {
            List<Type> allTypes = BH.Engine.Base.Query.AllTypeList().Where(x => x.Namespace.StartsWith("BH.oM") || x.Namespace.StartsWith("BH.Revit.oM")).Where(x => !x.IsEnum).ToList();

            List<Type> nonIObjects = allTypes.Where(x => !typeof(IObject).IsAssignableFrom(x)).ToList();

            List<string> interfaceTypes = nonIObjects.Where(x => x.IsInterface).Select(x => x.FullName).Distinct().ToList();
            List<string> classTypes = nonIObjects.Where(x => !x.IsInterface).Select(x => x.FullName).Distinct().ToList();

            Console.WriteLine("Failing interface types:");
            foreach (string type in interfaceTypes) 
            {
                Console.WriteLine(type);
            }

            Console.WriteLine();
            Console.WriteLine("Failing class types:");
            foreach (string type in classTypes)
            {
                Console.WriteLine(type);
            }
        }
    }
}
