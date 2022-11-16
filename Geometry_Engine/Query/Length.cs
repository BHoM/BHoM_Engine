/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vector                   ****/
        /***************************************************/

        [Description("Calculates the length of a Vector.")]
        [Input("vector", "The vector to calculate the length of.")]
        [Output("length", "The length of the Vector.", typeof(Length))]
        public static double Length(this Vector vector)
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

        /***************************************************/

        [Description("Calculates the square length of a Vector. Faster to compute than the length and can be usefull for example where only relative lengths between vectors are sought.")]
        [Input("vector", "The vector to calculate the square length of.")]
        [Output("sqLength", "The square length of the Vector.")]
        public static double SquareLength(this Vector vector)
        {
            return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Calculates the length of an Arc.")]
        [Input("curve", "The Arc to calculate the length of.")]
        [Output("length", "The length of the Arc.", typeof(Length))]
        public static double Length(this Arc curve)
        {
            if (curve == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query length as the geometry is null.");
                return double.NaN;
            }
            return curve.Angle() * curve.Radius;
        }

        /***************************************************/

        [Description("Calculates the length of an Circle.")]
        [Input("curve", "The Circle to calculate the length of.")]
        [Output("length", "The length of the Circle.", typeof(Length))]
        public static double Length(this Circle curve)
        {
            if (curve == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query length as the geometry is null.");
                return double.NaN;
            }
            return 2 * Math.PI * curve.Radius;
        }

        /***************************************************/

        [Description("Calculates the length of an Ellipse. Evaluated as an infinite series, utilising 10 times the ratio of the long and short radius number of terms.\n" +
                     "Gives very close to exact results for ellipses with an ratio of up to 1:20 000 between the long and short radius.")]
        [Input("curve", "The Ellipse to calculate the length of.")]
        [Output("length", "The length of the Ellipse.", typeof(Length))]
        public static double Length(this Ellipse curve)
        {
            if (curve == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query length as the geometry is null.");
                return double.NaN;
            }

            //Get out a as the long radius and b as the short radius
            double a = Math.Max(curve.Radius1, curve.Radius2);
            double b = Math.Min(curve.Radius1, curve.Radius2);

            double h = (a - b)/ (a + b);
            h *= h;

            //When h is equal to 1, the ellipse is a line
            //The algorithm below will not be able to handle to elongated ellipses, hence 
            //pointless to evaluate.
            if (1 - h < 1e-6)
            {
                //Raise a warning when b is not exactly equal to 0
                if (b != 0)
                {
                    Base.Compute.RecordWarning("The aspect ratio of the provided Ellipse is to large to be able to accurately evaluate the length. Approximate value of 4 times length of line between vertex and co-vertex returned.");
                    double hypotenus = Math.Sqrt(a * a + b * b);
                    return 4 * hypotenus;
                }
                else
                    return 4 * a;
            }

            double p = 0;

            //Ratio of ellipse to calculate number of series to evaluate
            int nbSeries = (int)Math.Round(a / b * 10);

            //"Infinite" series evaluated
            List<double> binomialCooefs = SquareBinomialCoefficientsEllipse(nbSeries);

            //Ratio of ellipse to calculate number of series to evaluate
            nbSeries = (int)Math.Round(Math.Min(a / b * 10, binomialCooefs.Count));

            //Evaluated as the "infinite" series listed in here:
            //https://en.wikipedia.org/wiki/Ellipse#Circumference
            //noted on the wikipedia page as Ivory and Bessel.
            for (int i = 0; i < nbSeries; i++)
            {
                p += Math.Pow(h, i) * binomialCooefs[i];
            }

            double length = Math.PI * (a + b) * p;

            //For ellipses with an extremely high ratio (over 1:1000 000) the length from the evaluated series will be to low.
            //The check bellow checks that the returned length is at least that of 4 times the longest radius.
            if (length < 4 * a)
            {
                Base.Compute.RecordWarning("The aspect ratio of the provided Ellipse is to large to be able to accurately evaluate the length. Approximate value of 4 times length of line between vertex and co-vertex. ");
                double hypotenus = Math.Sqrt(a * a + b * b);
                return 4 * hypotenus;
            }

            return length; 
        }

        /***************************************************/

        [Description("Calculates the length of an Line.")]
        [Input("curve", "The Line to calculate the length of.")]
        [Output("length", "The length of the Line.", typeof(Length))]
        public static double Length(this Line curve)
        {
            return (curve.Start - curve.End).Length();
        }

        /***************************************************/

        [Description("Calculates the square length of an Line. Faster to calculate than the length.")]
        [Input("curve", "The Line to calculate the square length of.")]
        [Output("sqLength", "The square length of the Line.")]
        public static double SquareLength(this Line curve)
        {
            return (curve.Start - curve.End).SquareLength();
        }

        /***************************************************/

        [Description("Calculates the length of an PolyCurve. Calculated as the sum of the length of its parts.")]
        [Input("curve", "The PolyCurve to calculate the length of.")]
        [Output("length", "The length of the PolyCurve.", typeof(Length))]
        public static double Length(this PolyCurve curve)
        {
            if (curve == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query length as the geometry is null.");
                return double.NaN;
            }
            return curve.Curves.Sum(c => c.ILength());
        }

        /***************************************************/

        [Description("Calculates the length of a Polyline.")]
        [Input("curve", "The Polyline to calculate the length of.")]
        [Output("length", "The length of the Polyline.", typeof(Length))]
        public static double Length(this Polyline curve)
        {
            if (curve == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query length as the geometry is null.");
                return double.NaN;
            }
            double length = 0;
            List<Point> pts = curve.ControlPoints;

            for (int i = 1; i < pts.Count; i++)
                length += (pts[i] - pts[i - 1]).Length();

            return length;
        }

        [Description("Calculates the length of a Polyline.")]
        [Input("curve", "The Polyline to calculate the length of.")]
        [Output("length", "The length of the Polyline.", typeof(Length))]
        public static double Length(this NurbsCurve curve, int multi = 4)
        {
            if (curve == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query length as the geometry is null.");
                return double.NaN;
            }

            var divs = curve.CurvePointsByParameterStep(curve.ControlPoints.Count * multi + 1);

            var pts = divs.Item1;
            var ts = divs.Item2;
            double length_a = 0.0;

            for (int i = 0; i < pts.Count-2; i+=2)
            {
                length_a += curve.GetSegmentLength(ts[0], ts[1], ts[2], pts[0], pts[1], pts[2]);
            }

            return length_a;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Calculates the length of a Curve.")]
        [Input("curve", "The ICurve to calculate the length of.")]
        [Output("length", "The length of the Arc.", typeof(Length))]
        public static double ILength(this ICurve curve)
        {
            if (curve == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query length as the geometry is null.");
                return double.NaN;
            }
            return Length(curve as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static double Length(this ICurve curve)
        {
            Base.Compute.RecordError($"Length is not implemented for ICurves of type: {curve.GetType().Name}.");
            return double.NaN;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double GetSegmentLength(this NurbsCurve curve, double t0, double t1, double t2, Point p0, Point p1, Point p2, double tolerance = Tolerance.Distance, double angleTol = 1e-2, double ratioTol = 1.1)
        {
            double d1 = p0.Distance(p2);
            double da = p0.Distance(p1);
            double db = p1.Distance(p2);

            double d2 = da + db;

            double tol2 = tolerance / 10;

            if (d2 < tolerance)
            {
                return d2 + (d2 - d1) / 3;
            }
            else if (d1 < tolerance || d2 / d1 < angleTol || da < tol2 || db / da < ratioTol || db < tol2 || da / db < ratioTol)
            {
                double t01 = (t0 + t1) / 2;
                Point p01 = curve.PointAtParameter(t01);
                double d3 = curve.GetSegmentLength(t0, t01, t1, p0, p01, p1, tolerance, angleTol, ratioTol);

                double t12 = (t1 + t2) / 2;
                Point p12 = curve.PointAtParameter(t12);
                double d4 = curve.GetSegmentLength(t1,t12,t2,p1,p12,p2,tolerance,angleTol,ratioTol);

                return d3 + d4;
            }
            else
                return d2 + (d2 - d1) / 3;

        }

        [Description("Calculates the square binomial cooefficients used in the infinite series of the ellipse length calculation.")]
        private static List<double> SquareBinomialCoefficientsEllipse(int requiredCount)
        {
            //Only allow for one thread to modify the cache at once.
            lock (m_CooeficientLock)
            {
                //Calcuation of coefficients only needs to be done once.
                //Coefficients are being stored for next ellipse to be evaluated

                //Check if coefficients up to the count required have already been added
                if (requiredCount > m_EllipseCoeficients.Count)
                {
                    //Limit the count to 100 000 to avoid being locked for to long time.
                    //For common cases (ratios of less than 1:100) 1000 easily enough, but allowing a larger limit.
                    //The set limit of 100 000 should give accurate results for ellipses with a ratio of up to 1:20 000 between the long and short radius.
                    int addCount = Math.Min(requiredCount, 100000);
                    int currentCount = m_EllipseCoeficients.Count;
                    for (double i = currentCount; i < addCount; i++)
                    {
                        double binomialCoef = 1;

                        List<double> js = new List<double>();

                        //Construction a series to evaluate, counting first, last, send first, second last etc.
                        //This is done to avoid numeric overflow of the binomial cooefficient, as it for get to big if this is done
                        //For i over 1000 without this technique.
                        if (i % 2 == 0)
                        {
                            double j = 1;
                            while (js.Count < i)
                            {
                                js.Add(j);
                                js.Add(i - j + 1);
                                j++;
                            }
                        }
                        else
                        {
                            double j = 1;
                            while (js.Count < i - 1)
                            {
                                js.Add(j);
                                js.Add(i - j + 1);
                                j++;
                            }
                            js.Add(i / 2 + 0.5);
                        }

                        //Calculate the cooeficient for the current i
                        foreach (double j in js)
                        {
                            binomialCoef *= (0.5 - (i - j)) / j;
                        }

                        //The square of the cooeficcient is used for the length calculation, hence storing the square cooeficient
                        binomialCoef *= binomialCoef;

                        m_EllipseCoeficients.Add(binomialCoef);
                    }
                }
            }
            return m_EllipseCoeficients;
        }

        /***************************************************/

        private static List<double> m_EllipseCoeficients = new List<double>();
        private static object m_CooeficientLock = new object();

        /***************************************************/
    }
}



