using BH.Engine.Base;
using BH.Engine.Test;
using BH.oM.Base.Attributes;
using BH.oM.Data.Library;
using BH.oM.Test.NUnit;
using BH.oM.Test.Results;
using BH.oM.Test.UnitTests;
using BH.Tests.Setup;
using ICSharpCode.Decompiler.TypeSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Tests.Engine.Structure
{
    public class Compliance : ComplianceTestBase
    {
        public static IEnumerable<string> TestFiles()
        {
            return GetCsFiles("Structure_Engine");
        }
    }
}
