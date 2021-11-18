/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vector                   ****/
        /***************************************************/

        public static double Length(this Vector vector)
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

        /***************************************************/

        public static double SquareLength(this Vector vector)
        {
            return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static double Length(this Arc curve)
        {
            return curve.Angle() * curve.Radius;
        }

        /***************************************************/

        public static double Length(this Circle curve)
        {
            return 2 * Math.PI * curve.Radius;
        }

        /***************************************************/

        public static double Length(this Ellipse curve)
        {
            double a = Math.Max(curve.Radius1, curve.Radius2);
            double b = Math.Min(curve.Radius1, curve.Radius2);

            //Special case circle
            if (a == b)
                return 2 * Math.PI * a;

            //Special case line
            if (b == 0)
                return 4 * a;

            double h = (a - b)/ (a + b);
            h *= h;

            double p = 0;

            //Ratio of ellipse to calculate number of series to evaluate
            int nbSeries = (int)Math.Round(a / b * 10);

            //"Infinite" series evaluated
            List<double> binomialCooefs = SquareBinomialCoefficientsEllipse(nbSeries);

            //Ratio of ellipse to calculate number of series to evaluate
            nbSeries = (int)Math.Round(Math.Min(a / b * 10, binomialCooefs.Count));

            for (int i = 0; i < nbSeries; i++)
            {
                p += Math.Pow(h, i) * binomialCooefs[i];
            }

            double length = Math.PI * (a + b) * p;

            //For ellipses with a very high (over 1:1000) ratio of a and b, the calculated length will be to low
            //The check bellow checks that the returned length is at least that
            if (length < 4 * a)
            {
                Engine.Reflection.Compute.RecordWarning("The aspect ratio of the provided Ellipse is to large to be able to accurately evaluate the length. Approximate value of 4 times largest radius returned.");
                return 4 * a;
            }

            return length; 
        }

        /***************************************************/

        public static double Length(this Line curve)
        {
            return (curve.Start - curve.End).Length();
        }

        /***************************************************/

        public static double SquareLength(this Line curve)
        {
            return (curve.Start - curve.End).SquareLength();
        }

        /***************************************************/

        public static double Length(this PolyCurve curve)
        {
            return curve.Curves.Sum(c => c.ILength());
        }

        /***************************************************/

        public static double Length(this Polyline curve)
        {
            double length = 0;
            List<Point> pts = curve.ControlPoints;

            for (int i = 1; i < pts.Count; i++)
                length += (pts[i] - pts[i - 1]).Length();

            return length;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double ILength(this ICurve curve)
        {
            return Length(curve as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static double Length(this ICurve curve)
        {
            Reflection.Compute.RecordError($"Length is not implemented for ICurves of type: {curve.GetType().Name}.");
            return double.NaN;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<double> SquareBinomialCoefficientsEllipse(int requiredCount)
        {
            lock (m_cooeficientLock)
            {
                if (m_ellipseCoeficients == null)
                    m_ellipseCoeficients = new List<double>();

                if (requiredCount > m_ellipseCoeficients.Count)
                {

                    //Limit the count to 100 000 to avoid being locked for to long time.
                    //For common cases (ratios of less than 1:100) 1000 easily enough, but aloowing a larger limit.
                    int addCount = Math.Min(requiredCount, 100000);
                    int currentCount = m_ellipseCoeficients.Count;
                    for (double i = currentCount; i < addCount; i++)
                    {
                        double binomialCoef = 1;

                        List<double> js = new List<double>();

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


                        foreach (double j in js)
                        {
                            binomialCoef *= (0.5 - (i - j)) / j;
                        }

                        binomialCoef *= binomialCoef;

                        m_ellipseCoeficients.Add(binomialCoef);
                    }
                }
            }
            return m_ellipseCoeficients;
        }

        /***************************************************/

        private static List<double> m_ellipseCoeficients = null;
        private static object m_cooeficientLock = new object();

        /***************************************************/
    }
}
