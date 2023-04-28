using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Tests.Engine.Serialiser
{
    public static partial class Helpers
    {
        public static string TemporaryLogFolder()
        {
            return "C:\\Temp\\SerialiserTests";
        }

        public static string TemporaryLogPath(string fileName, bool clear) 
        {
            string filePath = System.IO.Path.Combine(Helpers.TemporaryLogFolder(), fileName);
            if (clear)
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            return filePath;
        }
    }
}
