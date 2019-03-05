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
using System.Collections.Generic;
using System.Linq;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsClosed(this Arc arc, double tolerance = Tolerance.Distance)
        {
            return (arc.Angle() - Math.PI * 2)*arc.Radius > -tolerance;
        }

        /***************************************************/

        public static bool IsClosed(this Circle circle, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        public static bool IsClosed(this Line line, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        public static bool IsClosed(this Ellipse ellipse, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        [NotImplemented]
        public static bool IsClosed(this NurbsCurve curve, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static bool IsClosed(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            List<ICurve> curves = curve.Curves;
            double sqTol = tolerance * tolerance;
            if (curves[0].IStartPoint().SquareDistance(curves.Last().IEndPoint()) > sqTol)
                return false;

            for (int i = 1; i < curves.Count; i++)
            {
                if (curves[i - 1].IEndPoint().SquareDistance(curves[i].IStartPoint()) > sqTol)
                    return false;
            }

            return true;
        }

        /***************************************************/

        public static bool IsClosed(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return false;

            return pts.First().SquareDistance(pts.Last()) < tolerance * tolerance;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool IIsClosed(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            return IsClosed(curve as dynamic, tolerance);
        }

        /***************************************************/
    }
}
