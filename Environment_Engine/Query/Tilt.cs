/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Deprecated("3.0", "Deprecated to expose tolerance as optional parameter", null, "Tilt(this IEnvironmentObject environmentObject, double distanceTolarance = BH.oM.Geometry.Tolerance.Distance, double AngleTolarance = BH.oM.Geometry.Tolerance.Angle)")]
        public static double Tilt(this IEnvironmentObject environmentObject)
        {
            return environmentObject.Tilt(BH.oM.Geometry.Tolerance.Distance, BH.oM.Geometry.Tolerance.Angle);
        }

        [Deprecated("3.0", "Deprecated to expose tolerance as optional parameter", null, "Tilt(this polyline polyline, double distanceTolarance = BH.oM.Geometry.Tolerance.Distance, double angleTolarance = BH.oM.Geometry.Tolerance.Angle")]
        public static double Tilt(this Polyline polyline)
        {
            return polyline.Tilt(BH.oM.Geometry.Tolerance.Distance, BH.oM.Geometry.Tolerance.Angle);
        }

        [Description("Returns the tilt of an Environment Object")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have its tilt queried")]
        [Input("distanceTolerance", "distanceTolerance, default is set to BH.oM.Geometry.Tolerance.Distanc")]
        [Input("angleTolerance", "The tolerance of the angle that defines a straight line. Default is set to the value defined by BH.oM.Geometry.Tolerance.Angle")]
        [Output("tilt", "The tilt of the Environment Object")]
        public static double Tilt(this IEnvironmentObject environmentObject, double distanceTolarance = BH.oM.Geometry.Tolerance.Distance, double angleTolarance = BH.oM.Geometry.Tolerance.Angle)
        {
            return environmentObject.Polyline().Tilt(distanceTolarance, angleTolarance);
        }

        [Description("Returns the tilt of a BHoM Geometry Polyline")]
        [Input("polyline", "The BHoM Geometry Polyline having its tilt queried")]
        [Input("distanceTolerance", "distanceTolerance, default is set to BH.oM.Geometry.Tolerance.Distanc")]
        [Input("angleTolerance", "The tolerance of the angle that defines a straight line. Default is set to the value defined by BH.oM.Geometry.Tolerance.Angle")]
        [Output("tilt", "The tilt of the polyline")]
        public static double Tilt(this Polyline polyline, double distanceTolarance = BH.oM.Geometry.Tolerance.Distance, double angleTolarance = BH.oM.Geometry.Tolerance.Angle)
        {
            double tilt;

            List<Point> pts = polyline.DiscontinuityPoints(distanceTolarance, angleTolarance);

            if (pts.Count < 3 || !BH.Engine.Geometry.Query.IsClosed(polyline) || !BH.Engine.Geometry.Query.IsPlanar(polyline, 0.001)) return -1; //Error protection on pts having less than 3 elements to create a plane or pLine not being closed


            polyline = Geometry.Modify.CleanPolyline(polyline, 0.01);
            Plane plane = BH.Engine.Geometry.Compute.FitPlane(polyline);

            //The polyline can be locally concave. Check if the polyline is clockwise.
            if (!polyline.IsClockwise(plane.Normal))
                plane.Normal = -plane.Normal;

            tilt = BH.Engine.Geometry.Query.Angle(plane.Normal, Plane.XY.Normal) * (180 / Math.PI);

            return tilt;
        }
    }
}
