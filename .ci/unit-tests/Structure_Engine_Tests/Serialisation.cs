using BH.oM.Test.Results;
using BH.oM.Test.UnitTests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.Tests.Setup;
using BH.oM.Structure.Elements;

namespace BH.Tests.Engine.Structure
{
    public class Serialisation : SerialisationTestBase
    {
        public static IEnumerable<Type> OmTypes()
        {
            return oMTypesToTest(typeof(Bar).Assembly);
        }

        public static IEnumerable<MethodBase> EngineMethods()
        {
            return EngineMethodsToTest(typeof(BH.Engine.Structure.Query).Assembly);
        }
    }
}
