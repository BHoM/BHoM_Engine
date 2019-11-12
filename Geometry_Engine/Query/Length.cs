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
            return curve.Angle() * curve.Radius();
        }

        /***************************************************/

        public static double Length(this Circle curve)
        {
            return 2 * Math.PI * curve.Radius;
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

        [NotImplemented]
        public static double Length(this NurbsCurve curve)
        {
            throw new NotImplementedException();
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

        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static double Length(this IElement1D element1D)
        {
            return element1D.IGeometry().ILength();
        }
        
        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double ILength(this ICurve curve)
        {
            return Length(curve as dynamic);
        }

        /***************************************************/
    }
}
