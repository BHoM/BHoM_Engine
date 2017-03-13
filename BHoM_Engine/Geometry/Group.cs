using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHoM.Geometry
{
    public static class XGroup
    {

        internal static void Mirror(IGroup group, Plane p)
        {
            foreach (BHoMGeometry geom in group.Objects)
            {
                geom.Mirror(p);
            }
        }

        internal static void Project(IGroup group, Plane p) 
        {
            foreach (BHoMGeometry geom in group.Objects)
            {
                geom.Project(p);
            }
        }

        internal static void Transform(IGroup group, Transform t) 
        {
            foreach (BHoMGeometry geom in group.Objects)
            {
                geom.Transform(t);
            }
        }

        internal static void Translate(IGroup group, Vector v)
        {
            foreach (BHoMGeometry geom in group.Objects)
            {
                geom.Translate(v);
            }
        }

        public static void Mirror<T>(this Group<T> group, Plane p) where T : BHoMGeometry
        {
            for (int i = 0; i < group.Geometry.Count; i++)
            {
                group.Geometry[i].Mirror(p);
            }
            group.Update();
        }

        public static void Project<T>(this Group<T> group, Plane p) where T : BHoMGeometry
        {
            for (int i = 0; i < group.Geometry.Count; i++)
            {
                group.Geometry[i].Project(p);
            }
            group.Update();
        }

        public static void Transform<T>(this Group<T> group, Transform t) where T : BHoMGeometry
        {
            for (int i = 0; i < group.Geometry.Count; i++)
            {
                group.Geometry[i].Transform(t);
            }
            group.Update();
        }

        public static void Translate<T>(this Group<T> group, Vector v) where T : BHoMGeometry
        {
            for (int i = 0; i < group.Geometry.Count; i++)
            {
                group.Geometry[i].Translate(v);
            }
            group.Update();
        }

    }
}
