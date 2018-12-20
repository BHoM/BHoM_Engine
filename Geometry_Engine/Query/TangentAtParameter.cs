/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using System;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Vector TangentAtParameter(this Arc curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double paramTol = tolerance / curve.Length();
            if (parameter > 1 + paramTol || parameter < 0 - paramTol)
                return null;
            
            return curve.CoordinateSystem.Y.Rotate(curve.StartAngle + (curve.EndAngle - curve.StartAngle) * parameter, curve.CoordinateSystem.Z);
        }

        /***************************************************/

        public static Vector TangentAtParameter(this Circle curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double paramTol = tolerance / curve.Length();
            if (parameter > 1 + paramTol || parameter < 0 - paramTol)
                return null;

            Vector n = curve.Normal;
            Vector refVector = 1 - Math.Abs(n.DotProduct(Vector.XAxis)) > Tolerance.Angle ? Vector.XAxis : Vector.ZAxis;
            Vector localX = n.CrossProduct(refVector).Normalise();
            return n.CrossProduct(localX).Rotate(parameter * 2 * Math.PI, n);
        }

        /***************************************************/

        public static Vector TangentAtParameter(this Line curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double paramTol = tolerance / curve.Length();
            if (parameter > 1 + paramTol || parameter < 0 - paramTol)
                return null;

            return curve.Direction();
        }

        /***************************************************/

        [NotImplemented]
        public static Vector TangentAtParameter(this NurbsCurve curve, double parameter, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Vector TangentAtParameter(this PolyCurve curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double length = curve.Length();
            double paramTol = tolerance / length;
            if (parameter > 1 + paramTol || parameter < 0 - paramTol)
                return null;

            double cLength = parameter * length;
            foreach (ICurve c in curve.SubParts())
            {
                double l = c.ILength();
                if (l >= cLength) return c.ITangentAtParameter(cLength / l);
                cLength -= l;
            }

            return curve.EndDir();
        }

        /***************************************************/

        public static Vector TangentAtParameter(this Polyline curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double length = curve.Length();
            double paramTol = tolerance / length;
            if (parameter > 1 + paramTol || parameter < 0 - paramTol)
                return null;

            double cLength = parameter * length;
            double sum = 0;
            foreach (Line line in curve.SubParts())
            {
                sum += line.Length();
                if (cLength <= sum)
                    return line.Direction();
            }

            return curve.EndDir();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Vector ITangentAtParameter(this ICurve curve, double parameter, double tolerance = Tolerance.Distance)
        {
            return TangentAtParameter(curve as dynamic, parameter, tolerance);
        }

        /***************************************************/
    }
}
