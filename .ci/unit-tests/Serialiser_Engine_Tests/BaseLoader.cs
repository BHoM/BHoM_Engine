using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Tests.Engine.Serialiser
{
    public class BaseLoader
    {
        [OneTimeSetUp]
        public static void EnsureFolderExist()
        {
            if (!Directory.Exists(Helpers.TemporaryLogFolder()))
            {
                Directory.CreateDirectory(Helpers.TemporaryLogFolder());
            }
        }

        [OneTimeSetUp]
        public static void EnsureAssembliesLoaded()
        {
            BH.Engine.Base.Compute.LoadAllAssemblies();
            //string regexFilter = $"^Revit_.*_20\\d\\d$";
            //BH.Engine.Base.Compute.LoadAllAssemblies("", regexFilter);
        }
    }
}
