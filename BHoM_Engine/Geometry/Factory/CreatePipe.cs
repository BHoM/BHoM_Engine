using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHoM.Geometry
{
    public static partial class Create
    {
        public static Pipe Pipe(Curve centreline, double radius)
        {
            return new BHoM.Geometry.Pipe(centreline, radius);
        }
    }

}
