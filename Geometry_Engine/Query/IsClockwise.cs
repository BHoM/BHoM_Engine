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
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("5.2", "BH.Engine.Geometry.Query.IsClockwise(BH.oM.Geometry.Polyline, BH.oM.Geometry.Vector, System.Double)")]
        [Description("Checks if the segments of the IPolyline are defined in a clockwise order around the provided normal vector. Curve needs to be closed for the method to function.")]
        [Input("polyline", "The closed IPolyline to check if it is defined in a clockwise manner in relation to the Vector.")]
        [Input("normal", "The normal vector to check against.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("isClockwise", "Returns true if the IPolyline is defined clockwise around the provided normal.")]
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
        [Description("Checks if the segments of the IPolyCurve are defined in a clockwise order around the provided normal vector. Curve needs to be closed for the method to function.")]
        [Input("curve", "The closed IPolyCurve to check if it is defined in a clockwise manner in relation to the Vector.")]
        [Input("normal", "The normal vector to check against.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("isClockwise", "Returns true if the IPolyCurve is defined clockwise around the provided normal.")]
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

        [Description("Checks if the segments of the Polyline are defined in a clockwise if inspected from the provided view point.\n" + 
                     "This is done by projecting the point to the plane of the curve and creating a normal vector from the view point to the projected point. The segments of the Polyline are then checked if they are defined clockwise around this vector.\n" + 
                     "Curve needs to be closed and the point needs to be outside the plane of the curve for the method to function.")]
        [Input("polyline", "The closed Polyline to check if it is defined in a clockwise manner in relation to the view Point.")]
        [Input("viewPoint", "The the point from where the the curve is to be checked. Requires the curve to not be in the plane of the curve.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("isClockwise", "Returns true if the Polyline is defined clockwise when inspected from the provided viewpoint.")]
        public static bool IsClockwise(this Polyline polyline, Point viewPoint, double tolerance = Tolerance.Distance)
        {
            Plane plane = polyline.FitPlane(tolerance);
            Point projectedPoint = viewPoint.Project(plane);
            Vector vector = (projectedPoint - viewPoint).Normalise();

            return IsClockwise(polyline, vector);
        }

        /***************************************************/

        [Description("Checks if the segments of the PolyCurve are defined in a clockwise if inspected from the provided view point.\n" +
             "This is done by projecting the point to the plane of the curve and creating a normal vector from the view point to the projected point. The segments of the PolyCurve are then checked if they are defined clockwise around this vector.\n" +
             "Curve needs to be closed and the point needs to be outside the plane of the curve for the method to function.")]
        [Input("curve", "The closed PolyCurve to check if it is defined in a clockwise manner in relation to the view Point.")]
        [Input("viewPoint", "The the point from where the the curve is to be checked. Requires the curve to not be in the plane of the curve.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("isClockwise", "Returns true if the PolyCurve is defined clockwise when inspected from the provided viewpoint.")]
        public static bool IsClockwise(this PolyCurve curve, Point viewPoint, double tolerance = Tolerance.Distance)
        {
            Plane plane = curve.FitPlane(tolerance);
            Point projectedPoint = viewPoint.Project(plane);
            Vector vector = (projectedPoint - viewPoint).Normalise();

            return IsClockwise(curve, vector);
        }

        /***************************************************/

        [Description("Checks if the Arc is defined in a clockwise order around the provided axis Vector.")]
        [Input("arc", "The Arc to check if it is defined in a clockwise manner in relation to the Vector.")]
        [Input("axis", "The axis vector to check against.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("isClockwise", "Returns true if the Arc is defined clockwise around the provided axis.")]
        public static bool IsClockwise(this Arc arc, Vector axis, double tolerance = Tolerance.Distance)
        {
            Vector normal = arc.CoordinateSystem.Z;
            return ((normal.DotProduct(axis) < 0) != (arc.Angle() > Math.PI));       
        }

        /***************************************************/

        [Description("Checks if the Circle is defined in a clockwise order around the provided axis Vector.")]
        [Input("curve", "The Circle to check if it is defined in a clockwise manner in relation to the Vector.")]
        [Input("axis", "The axis vector to check against.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("isClockwise", "Returns true if the Circle is defined clockwise around the provided axis.")]
        public static bool IsClockwise(this Circle curve, Vector axis, double tolerance = Tolerance.Distance)
        {
            return axis.DotProduct(curve.Normal()) > 0;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Checks if the ICurve is defined in a clockwise order around the provided axis Vector.")]
        [Input("curve", "The ICurve to check if it is defined in a clockwise manner in relation to the Vector.")]
        [Input("axis", "The axis vector to check against.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("isClockwise", "Returns true if the ICurve is defined clockwise around the provided axis.")]
        public static bool IIsClockwise(this ICurve curve, Vector axis, double tolerance = Tolerance.Distance)
        {
            return IsClockwise(curve as dynamic, axis, tolerance);
        }

        /***************************************************/
    }
}


