using System;

using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Point PointAtParameter(this Arc curve, double t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            return PointAtLength(curve, t * curve.Length());
        }

        /***************************************************/

        public static Point PointAtParameter(this Circle curve, double t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            return PointAtLength(curve, t * curve.Length());
        }

        /***************************************************/

        public static Point PointAtParameter(this Line curve, double t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            Vector vector = curve.End - curve.Start;
            return (curve.Start + vector * t);
        }

        /***************************************************/

        //TODO: Testing needed!!
        public static Point PointAtParameter(this NurbCurve curve, double t)
        {

            Point sumNwP = new Point { X = 0, Y = 0, Z = 0 };
            double sumNw = 0;
            if (t == 0) return curve.ControlPoints[0];
            else if (t >= curve.Knots[curve.Knots.Count - 1]) return curve.ControlPoints[curve.ControlPoints.Count-1];

            int order = curve.Degree() + 1;
            for (int i = 0; i < curve.ControlPoints.Count ; i++)
            {
                double Nt = BasisFunction(curve, i, order - 1, t);
                if (Nt == 0) continue;
                sumNwP += curve.ControlPoints[i] * Nt * curve.Weights[i];
                sumNw += Nt * curve.Weights[i];
            }
            return sumNwP / sumNw;
           

            //double[] sumNwP = new double[m_Dimensions];
            //double sumNw = 0;
            //if (t == 0) return Common.Utils.SubArray<double>(m_ControlPoints, 0, 3);
            //else if (t >= m_Knots[m_Knots.Length - 1]) return Common.Utils.SubArray<double>(m_ControlPoints, m_ControlPoints.Length - 4, 3);
            //for (int i = 0; i < m_ControlPoints.Length / (m_Dimensions + 1); i++)
            //{
            //    double Nt = BasisFunction(i, m_Order - 1, t);
            //    if (Nt == 0) continue;
            //    sumNwP = VectorUtils.Add(sumNwP, VectorUtils.Multiply(m_ControlPoints, Nt * m_Weights[i], i * (m_Dimensions + 1), m_Dimensions));
            //    sumNw += Nt * m_Weights[i];
            //}
            //return VectorUtils.Divide(sumNwP, sumNw);



            throw new NotImplementedException(); // TODO NurbCurve.PointAtParameter()
        }



        /***************************************************/

        public static Point PointAtParameter(this PolyCurve curve, double t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            throw new NotImplementedException(); // TODO Polycurve.PointAtParameter() Relies on NurbCurve PointAt method
        }

        /***************************************************/

        public static Point PointAtParameter(this Polyline curve, double t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            return PointAtLength(curve, t * curve.Length());
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point IPointAtParameter(this ICurve curve, double t)
        {
            return PointAtParameter(curve as dynamic, t);
        }

        /***************************************************/
    }
}
