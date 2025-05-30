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

namespace BH.Tests.Engine.Structure
{
    public class DatasetUnitTests : UnitTestRunnerBase
    {
        public static IEnumerable<object[]> TestData()
        {
            return GetTestDataInRelativeFolder("Structure_Engine");
        }
    }
}
