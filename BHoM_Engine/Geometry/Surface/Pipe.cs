using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHoM.Geometry
{
    public static class XPipe
    {
        //public override Group<Curve> NakedEdges
        //{
        //    get
        //    {
        //        if (m_NakedEdges == null)
        //        {
        //            m_NakedEdges = new Group<Curve>();
        //            if (!Capped)
        //            {
        //                m_NakedEdges.AddRange(GetEndPerimeters());
        //            }
        //        }

        //        return base.NakedEdges;
        //    }
        //}

        //public override Group<Curve> InternalEdges
        //{
        //    get
        //    {
        //        if (m_InternalEdges == null)
        //        {
        //            m_InternalEdges = new Group<Curve>();
        //            if (Capped)
        //            {
        //                m_InternalEdges.AddRange(GetEndPerimeters());
        //            }
        //        }
        //        return base.InternalEdges;
        //    }
        //}

        private static Group<Curve> GetEndPerimeters(this Pipe pipe)
        {
            Group<Curve> geom = new Group<Curve>();
            Vector t1 = pipe.Centreline.TangentAt(pipe.Centreline.Knots[0]);
            Vector t2 = pipe.Centreline.TangentAt(pipe.Centreline.Knots[pipe.Centreline.Knots.Length - 1]);

            Plane p1 = new Plane(pipe.Centreline.StartPoint, t1);
            Plane p2 = new Plane(pipe.Centreline.EndPoint, t2);
            geom.Add(new Circle(pipe.Radius, p1));
            geom.Add(new Circle(pipe.Radius, p2));
            return geom;
        }

        public static Curve GetProfile(this Pipe pipe)
        {
            return pipe.GetEndPerimeters()[0];
        }

        internal static void Mirror(Pipe pipe, Plane p)
        {
            XBrep.Mirror(pipe,p);
            pipe.Centreline.Mirror(p);
        }

        internal static void Project(Pipe pipe, Plane p)
        {
            XBrep.Project(pipe, p);
            pipe.Centreline.Project(p);
        }

        internal static void Transform(Pipe pipe, Transform t)
        {
            XBrep.Transform(pipe, t);
            pipe.Centreline.Transform(t);
        }

        internal static void Translate(Pipe pipe, Vector v)
        {
            XBrep.Translate(pipe, v);
            pipe.Centreline.Translate(v);
        }
    }
}
