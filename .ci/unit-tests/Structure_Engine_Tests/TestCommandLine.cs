using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Tests.Engine.Structure
{
    public class TestCommandLine
    {

        [Test]
        public void TestCommandLineParameters()
        {
            int count = TestContext.Parameters.Count;
            TestContext.WriteLine($"There are {count} test parameters");

            var code = TestContext.Parameters.Get("UpdatedAssemblies", "<unknown>");
            var date = TestContext.Parameters.Get("Date", DateTime.MinValue);

            TestContext.WriteLine($"Fetched test parameters {code} and {date}");
        }
    }
}
