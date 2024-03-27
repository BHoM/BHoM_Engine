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
using System;
using System.Linq;
using System.Collections.Generic;
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsOnCurve(this Line line, Point pt, double tolerance = Tolerance.Distance)
        {
            double distToStart = Query.Distance(pt, line.Start);
            double distToEnd = Distance(line.End, pt);

            double maxTol = (distToStart + distToEnd) + tolerance;
            double minTol = (distToStart + distToEnd) - tolerance;

            return line.Length() >= minTol && line.Length() <= maxTol;
        }

        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsOnCurve(this Point point, Arc curve, double tolerance = Tolerance.Distance)
        {
            return point.Distance(curve) < tolerance;
        }

        /***************************************************/

        public static bool IsOnCurve(this Point point, Circle curve, double tolerance = Tolerance.Distance)
        {
            return point.Distance(curve) < tolerance;
        }

        /***************************************************/

        public static bool IsOnCurve(this Point point, Line curve, double tolerance = Tolerance.Distance)
        {
            return point.Distance(curve) < tolerance;
        }

        /***************************************************/

        public static bool IsOnCurve(this Point point, PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            return point.Distance(curve) < tolerance;
        }

        /***************************************************/

        public static bool IsOnCurve(this Point point, Polyline curve, double tolerance = Tolerance.Distance)
        {
            return point.Distance(curve) < tolerance;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool IIsOnCurve(this Point point, ICurve curve, double tolerance = Tolerance.Distance)
        {
            return IsOnCurve(point, curve as dynamic, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsOnCurve(this Point point, ICurve curve, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException($"IsOnCurve is not implemented for ICurves of type: {curve.GetType().Name}.");
        }

        /***************************************************/
    }
}





