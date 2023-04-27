using BH.oM.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Tests.Engine.Base.Query
{
    public static class AllTypesIObject
    {
        [Test]
        public static void CheckAllTypes()
        {
            BH.Engine.Base.Compute.LoadAllAssemblies();
            string regexFilter = $"^Revit_.*_20\\d\\d$";
            BH.Engine.Base.Compute.LoadAllAssemblies("", regexFilter);

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
