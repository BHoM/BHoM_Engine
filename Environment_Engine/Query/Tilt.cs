/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the tilt of an Environment Object")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have its tilt queried")]
        [Input("distanceTolerance", "Distance tolerance for calculating discontinuity points, default is set to BH.oM.Geometry.Tolerance.Distance")]
        [Input("angleTolerance", "Angle tolerance for calculating discontinuity points, default is set to the value defined by BH.oM.Geometry.Tolerance.Angle")]
        [Output("tilt", "The tilt of the Environment Object")]
        public static double Tilt(this IEnvironmentObject environmentObject, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            if(environmentObject == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the tilt of a null environment object.");
                return -1;
            }

            return environmentObject.Polyline().Tilt(distanceTolerance, angleTolerance);
        }

        [Description("Returns the tilt of a BHoM Geometry Polyline")]
        [Input("polyline", "The BHoM Geometry Polyline having its tilt queried")]
        [Input("distanceTolerance", "Distance tolerance for calculating discontinuity points, default is set to BH.oM.Geometry.Tolerance.Distance")]
        [Input("angleTolerance", "Angle tolerance for calculating discontinuity points, default is set to the value defined by BH.oM.Geometry.Tolerance.Angle")]
        [Output("tilt", "The tilt of the polyline")]
        public static double Tilt(this Polyline polyline, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            if(polyline == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the tilt of a null polyline.");
                return -1;
            }

            double tilt = 0;

            List<Point> pts = polyline.DiscontinuityPoints(distanceTolerance, angleTolerance);

            if (pts.Count < 3 || !BH.Engine.Geometry.Query.IsClosed(polyline, distanceTolerance) || !BH.Engine.Geometry.Query.IsPlanar(polyline, distanceTolerance))
                return -1; //Error protection on pts having less than 3 elements to create a plane or pLine not being closed


            polyline = BH.Engine.Geometry.Modify.CleanPolyline(polyline, angleTolerance);
            Plane plane = BH.Engine.Geometry.Compute.FitPlane(polyline, distanceTolerance);

            //The polyline can be locally concave. Check if the polyline is clockwise.
            if (!polyline.IsClockwise(plane.Normal, distanceTolerance))
                plane.Normal = -plane.Normal;

            tilt = BH.Engine.Geometry.Query.Angle(plane.Normal, Plane.XY.Normal) * (180 / Math.PI);

            return tilt;
        }
    }
}




