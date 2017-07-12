using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{
    public static partial class Create
    {
        public static Pipe Pipe(Curve centreline, double radius)
        {
            return new BH.oM.Geometry.Pipe(centreline, radius);
        }
    }

}
