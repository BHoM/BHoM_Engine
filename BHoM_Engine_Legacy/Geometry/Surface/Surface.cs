using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{
  


    public static class XSurface
    {
        //internal static void Transform(Surface surface, Transform t)
        //{
        //    XBrep.Transform(surface, t);
        //    surface.ControlPointVector = ArrayUtils.MultiplyMany(t, surface.ControlPointVector);
        //    surface.Update();
        //}

        //internal static void Translate(Surface surface, Vector v)
        //{
        //    XBrep.Translate(surface, v);
        //    surface.ControlPointVector = ArrayUtils.Add(surface.ControlPointVector, v);
        //    surface.Update();
        //}

        //internal static void Mirror(Surface surface, Plane p)
        //{
        //    XBrep.Mirror(surface, p);
        //    surface.ControlPointVector = ArrayUtils.Add(ArrayUtils.Multiply(p.ProjectionVectors(surface.ControlPointVector), 2), surface.ControlPointVector);
        //    surface.Update();
        //}

        //internal static void Project(Surface surface, Plane p)
        //{
        //    XBrep.Project(surface, p);
        //    surface.ControlPointVector = ArrayUtils.Add(p.ProjectionVectors(surface.ControlPointVector), surface.ControlPointVector);
        //    surface.Update();
        //}
    }
}
