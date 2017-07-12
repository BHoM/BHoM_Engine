using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BH.oM.Geometry
{


    public static class XBrep
    {
        public static bool IsPlanar(this Brep brep)
        {
            Plane p = null;// Plane.PlaneFromPoints();
            if (brep.GetExternalEdges().Count > 0)
            {
                p = Create.PlaneFromPointArray(brep.GetExternalEdges()[0].ControlPointVector, 4);
                for (int i = 1; i < brep.GetExternalEdges().Count; i++)
                {
                    if (!p.InPlane(brep.GetExternalEdges()[i].ControlPointVector, 4, 0.001)) return false;
                }
            }
            if (brep.GetInternalEdges().Count > 0)
            {
                if (p == null) p = Create.PlaneFromPointArray(brep.GetInternalEdges()[0].ControlPointVector, 4);
                for (int i = 1; i < brep.GetInternalEdges().Count; i++)
                {
                    if (!p.InPlane(brep.GetInternalEdges()[i].ControlPointVector, 4, 0.001)) return false;
                }
            }
            return true;
        }

        internal static void Mirror(Brep brep, Plane p)
        {
            brep.GetExternalEdges().Mirror(p);
            brep.GetInternalEdges().Mirror(p);
        }

        internal static void Project(Brep brep, Plane p)
        {
            brep.GetExternalEdges().Project(p);
            brep.GetInternalEdges().Project(p);
        }

        internal static void Transform(Brep brep, Transform t)
        {
            brep.GetExternalEdges().Transform(t);
            brep.GetInternalEdges().Transform(t);
        }

        internal static void Translate(Brep brep, Vector v)
        {
            brep.GetExternalEdges().Translate(v);
            brep.GetInternalEdges().Translate(v);
        }

    }
}
