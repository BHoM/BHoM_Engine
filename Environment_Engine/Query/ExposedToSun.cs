using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        public static bool ExposedToSun(string surfaceType)
        {
            if (String.IsNullOrEmpty(surfaceType)) return false;

            surfaceType = surfaceType.Replace(" ", String.Empty).ToLower();

            return surfaceType == "raisedFloor" || surfaceType == "exteriorwall" || surfaceType == "roof";
        }
    }
}
