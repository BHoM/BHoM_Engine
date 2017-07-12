using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{


    public static class XExtrusion
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
        //                m_NakedEdges.Add(Curve);
        //                Curve other = Curve.DuplicateCurve();
        //                other.Translate(Direction);
        //                m_NakedEdges.Add(other);
        //            }
        //            if (!Curve.IsClosed())
        //            {
        //                m_NakedEdges.Add(new Line(Curve.StartPoint, Curve.StartPoint + Direction));
        //                m_NakedEdges.Add(new Line(Curve.EndPoint, Curve.EndPoint + Direction));
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
        //                m_InternalEdges.Add(Curve);
        //                Curve other = Curve.DuplicateCurve();
        //                other.Translate(Direction);
        //                m_InternalEdges.Add(other);
        //            }
        //            if (Curve.IsClosed())
        //            {
        //                m_InternalEdges.Add(new Line(Curve.StartPoint, Curve.StartPoint + Direction));
        //                m_InternalEdges.Add(new Line(Curve.EndPoint, Curve.EndPoint + Direction));
        //            }
        //            int degree = Curve.Degree;
        //            int sameValue = 0;
        //            for (int i = 0; i < Curve.Knots.Length-1; i++)
        //            {
        //                sameValue = Curve.Knots[i] == Curve.Knots[i + 1] ? sameValue + 1 : 0;
        //                if (sameValue == degree)
        //                {
        //                    Point p = Curve.PointAt(Curve.Knots[i]);
        //                    m_InternalEdges.Add(new Line(p, p + Direction));
        //                }
        //            }
        //        }

        //        return base.InternalEdges;
        //    }
        //}
    }
}
