/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Point EndPoint(this Arc arc)
        {
            Vector locSt = arc.CoordinateSystem.X * arc.Radius;
            return arc.CoordinateSystem.Origin + locSt.Rotate(arc.EndAngle, arc.CoordinateSystem.Z);
        }

        /***************************************************/

        public static Point EndPoint(this Circle circle)
        {
            return circle.StartPoint();
        }

        /***************************************************/

        public static Point EndPoint(this Ellipse ellipse)
        {
            return ellipse.StartPoint();
        }

        /***************************************************/

        public static Point EndPoint(this Line line)
        {
            return line.End;
        }

        /***************************************************/

        public static Point EndPoint(this NurbsCurve curve)
        {
            //TODO: This should be based on the basis function?
            if (!curve.IsPeriodic())
                return curve.ControlPoints.LastOrDefault();
            else
            {
                Base.Compute.RecordError("EndPoint is not implemented for periodic NurbsCurves");
                return null;
            }
        }

        /***************************************************/

        public static Point EndPoint(this PolyCurve curve)
        {
            List<ICurve> curves = curve.Curves;

            for (int i = curves.Count -1; i >= 0; i--)
            {
                Point End = curves[i].IEndPoint();
                if (End != null)
                    return End;
            }

            return null;
        }

        /***************************************************/

        public static Point EndPoint(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.Last();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point IEndPoint(this ICurve curve)
        {
            return EndPoint(curve as dynamic);
        }


        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static Point EndPoint(this ICurve curve)
        {
            Base.Compute.RecordError("EndPoint is not implemented for curve of type: " + curve.GetType().Name + ". ");
            return null;
        }

        /***************************************************/
    }
}





