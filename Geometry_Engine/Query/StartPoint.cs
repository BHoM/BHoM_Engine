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
        /**** Public Methods - Curves                   ****/
        /***************************************************/
        
        public static Point StartPoint(this Arc arc)
        {
            Vector locSt = arc.CoordinateSystem.X * arc.Radius;
            return arc.CoordinateSystem.Origin + locSt.Rotate(arc.StartAngle, arc.CoordinateSystem.Z);
        }

        /***************************************************/

        public static Point StartPoint(this Circle circle)
        {
            Vector refVector = 1 - Math.Abs(circle.Normal.DotProduct(Vector.XAxis)) > Tolerance.Angle ? Vector.XAxis : Vector.ZAxis;
            Vector localX = circle.Normal.CrossProduct(refVector).Normalise() * circle.Radius;
            return circle.Centre + circle.Radius * localX.Normalise();
        }

        /***************************************************/

        public static Point StartPoint(this Line line)
        {
            return line.Start;
        }

        /***************************************************/

        [NotImplemented]
        public static Point StartPoint(this NurbsCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Point StartPoint(this PolyCurve curve)
        {
            foreach (ICurve c in curve.Curves)
            {
                Point start = c.IStartPoint();
                if (start != null)
                    return start;
            }

            return null;
        }

        /***************************************************/

        public static Point StartPoint(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.First();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point IStartPoint(this ICurve curve)
        {
            return StartPoint(curve as dynamic);
        }

        /***************************************************/
    }
}
