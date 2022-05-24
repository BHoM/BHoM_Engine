/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using System;
using BH.oM.Base.Attributes;
using BH.Engine.Base;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("5.2", "BH.Engine.Geometry.Query.IsClockwise(BH.oM.Geometry.Polyline, BH.oM.Geometry.Vector, System.Double)")]
        public static bool IsClockwise(this IPolyline polyline, Vector normal, double tolerance = Tolerance.Distance)
        {
            if (!polyline.IIsClosed(tolerance))
                throw new Exception("The polyline is not closed. IsClockwise method is relevant only to closed curves.");

            List<Point> cc = polyline.DiscontinuityPoints(tolerance);
            Vector dir1 = (cc[0] - cc.Last()).Normalise();
            Vector dir2;
            double angleTot = 0;

            for (int i = 1; i < cc.Count; i++)
            {
                dir2 = (cc[i] - cc[i - 1]).Normalise();
                double signedAngle = dir1.SignedAngle(dir2, normal);
                dir1 = dir2.DeepClone();

                if (Math.PI - Math.Abs(signedAngle) <= Tolerance.Angle)
                {
                    dir1 *= -1;
                    continue;
                }
                else
                    angleTot += signedAngle;
            }

            return angleTot > 0;
        }

        /***************************************************/

        [PreviousVersion("5.2", "BH.Engine.Geometry.Query.IsClockwise(BH.oM.Geometry.PolyCurve, BH.oM.Geometry.Vector, System.Double)")]
        public static bool IsClockwise(this IPolyCurve curve, Vector normal, double tolerance = Tolerance.Distance)
        {
            if (!curve.IIsClosed(tolerance))
                throw new Exception("The curve is not closed. IsClockwise method is relevant only to closed curves.");
            
            List<Point> cPts = new List<Point> { curve.IStartPoint() };
            foreach (ICurve c in curve.ISubParts())
            {
                if (c is Line)
                    cPts.Add(c.IEndPoint());
                else if (c is Arc)
                {
                    cPts.Add((c as Arc).PointAtParameter(0.25));
                    cPts.Add((c as Arc).PointAtParameter(0.5));
                    cPts.Add((c as Arc).PointAtParameter(0.75));
                    cPts.Add((c as Arc).EndPoint());
                }
                else if (c is Circle)
                {
                    cPts.Add((c as Circle).PointAtParameter(0.25));
                    cPts.Add((c as Circle).PointAtParameter(0.5));
                    cPts.Add((c as Circle).PointAtParameter(0.75));
                    cPts.Add((c as Circle).EndPoint());
                }
                else
                    throw new NotImplementedException("PolyCurve consisting of type: " + c.GetType().Name + " is not implemented for IsClockwise.");
            }

            return IsClockwise(new Polyline { ControlPoints = cPts }, normal, tolerance);
        }

        /***************************************************/

        public static bool IsClockwise(this Polyline polyline, Point viewPoint, double tolerance = Tolerance.Distance)
        {
            Plane plane = polyline.FitPlane(tolerance);
            Point projectedPoint = viewPoint.Project(plane);
            Vector vector = (projectedPoint - viewPoint).Normalise();

            return IsClockwise(polyline, vector);
        }

        /***************************************************/

        public static bool IsClockwise(this PolyCurve curve, Point viewPoint, double tolerance = Tolerance.Distance)
        {
            Plane plane = curve.FitPlane(tolerance);
            Point projectedPoint = viewPoint.Project(plane);
            Vector vector = (projectedPoint - viewPoint).Normalise();

            return IsClockwise(curve, vector);
        }

         /***************************************************/

        public static bool IsClockwise(this Arc arc, Vector axis, double tolerance = Tolerance.Distance)
        {
            Vector normal = arc.CoordinateSystem.Z;
            return ((normal.DotProduct(axis) < 0) != (arc.Angle() > Math.PI));       
        }

        /***************************************************/

        public static bool IsClockwise(this Circle curve, Vector axis, double tolerance = Tolerance.Distance)
        {
            return axis.DotProduct(curve.Normal()) > 0;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool IIsClockwise(this ICurve curve, Vector axis, double tolerance = Tolerance.Distance)
        {
            return IsClockwise(curve as dynamic, axis, tolerance);
        }

        /***************************************************/
    }
}


