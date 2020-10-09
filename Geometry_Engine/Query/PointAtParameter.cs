/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Point PointAtParameter(this Arc curve, double t)
        {
            if (t < 0)
                t = 0;
            if (t > 1)
                t = 1;

            double alfa = curve.Angle() * t + curve.StartAngle;
            Vector localX = curve.CoordinateSystem.X;
            return curve.CoordinateSystem.Origin + localX.Rotate(alfa, curve.FitPlane().Normal) * curve.Radius;
        }

        /***************************************************/

        public static Point PointAtParameter(this Circle curve, double t)
        {
            if (t < 0)
                t = 0;
            if (t > 1)
                t = 1;

            return PointAtLength(curve, t * curve.Length());
        }

        /***************************************************/

        public static Point PointAtParameter(this Line curve, double t)
        {
            if (t < 0)
                t = 0;
            if (t > 1)
                t = 1;

            Vector vector = curve.End - curve.Start;
            return (curve.Start + vector * t);
        }

        /***************************************************/

        public static Point PointAtParameter(this PolyCurve curve, double parameter)
        {
            if (parameter == 1)
                return curve.IEndPoint();

            double cLength = parameter * curve.Length();
            foreach (ICurve c in curve.SubParts())
            {
                double l = c.ILength();
                if (l >= cLength)
                    return c.IPointAtParameter(cLength / l);

                cLength -= l;
            }

            return null;
        }

        /***************************************************/

        public static Point PointAtParameter(this Polyline curve, double parameter)
        {
            if (parameter == 1)
                return curve.IEndPoint();

            double cLength = parameter * curve.Length();
            foreach (Line line in curve.SubParts())
            {
                double l = line.Length();
                if (l >= cLength)
                    return line.PointAtParameter(cLength / l);

                cLength -= l;
            }

            return null;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point IPointAtParameter(this ICurve curve, double t)
        {
            return PointAtParameter(curve as dynamic, t);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static Point PointAtParameter(this ICurve curve, double t)
        {
            Reflection.Compute.RecordError($"PointAtParameter is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}