using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHoM.Geometry
{
    public static class XPolySurface
    {
        internal static void Mirror(PolySurface surface , Plane p)
        {
            XBrep.Mirror(surface, p);
            surface.Surfaces.Mirror(p);
        }

        internal static void Project(PolySurface surface, Plane p)
        {
            XBrep.Project(surface,p);
            surface.Surfaces.Project(p);
        }

        internal static void Transform(PolySurface surface, Transform t)
        {
            XBrep.Transform(surface, t);
            surface.Surfaces.Transform(t);
        }

        internal static void Translate(PolySurface surface, Vector v)
        {
            XBrep.Translate(surface, v);
            surface.Surfaces.Translate(v);
        }
    }
}
