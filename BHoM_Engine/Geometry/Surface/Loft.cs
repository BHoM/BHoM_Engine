using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHoM.Geometry
{
    public static class XLoft
    {
        internal static void Mirror(Loft loft, Plane p)
        {
            XBrep.Mirror(loft, p);
            loft.Curves.Mirror(p);
        }

        internal static void Project(Loft loft, Plane p)
        {
            XBrep.Project(loft, p);
            loft.Curves.Project(p);
        }

        internal static void Transform(Loft loft, Transform t)
        {
            XBrep.Transform(loft, t);
            loft.Curves.Transform(t);
        }

        internal static void Translate(Loft loft, Vector v)
        {
            XBrep.Translate(loft, v);
            loft.Curves.Translate(v);
        }
    }
}
