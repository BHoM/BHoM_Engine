using BH.oM.Acoustic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Acoustic
{
    public static partial class Measure
    {
        public static double GetLength(this Ray ray)
        {
            return Geometry.Measure.GetLength(ray.Path as dynamic);
        }
    }
}
