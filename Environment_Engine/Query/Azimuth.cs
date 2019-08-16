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
using BH.oM.Environment;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the azimuth of a given environmental object")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have its azimuth queried")]
        [Input("referenceVector", "The reference vector for querying the azimuth from the object")]
        [Output("azimuth", "The azimuth of the Environment Object")]
        public static double Azimuth(this IEnvironmentObject environmentObject, Vector refVector)
        {
            return environmentObject.Polyline().Azimuth(refVector);
        }

        [Description("Returns the azimuth of a BHoM Geometry Polyline")]
        [Input("polyline", "A BHoM Geometry Polyline")]
        [Input("referenceVector", "The reference vector for querying the azimuth from the polyline")]
        [Output("azimuth", "The azimuth of the polyline")]
        public static double Azimuth(this Polyline polyline, Vector refVector)
        {
            List<Point> pts = polyline.DiscontinuityPoints();

            if (pts.Count < 3 || !polyline.IsClosed()) return -1; //Protection in case there aren't enough points to make a plane

            Plane plane = BH.Engine.Geometry.Create.Plane(pts[0], pts[1], pts[2]);

            //The polyline can be locally concave. Check if the polyline is clockwise.
            if (!BH.Engine.Geometry.Query.IsClockwise(polyline, plane.Normal))
                plane.Normal = -plane.Normal;

            double azimuth;
            if (Geometry.Modify.Normalise(plane.Normal).Z == 1)
                azimuth = 0;
            else if (Geometry.Modify.Normalise(plane.Normal).Z == -1)
                azimuth = 180;
            else
            {
                Vector v1 = Geometry.Modify.Project(plane.Normal, Plane.XY);
                Vector v2 = (Geometry.Modify.Project(refVector, Plane.XY));
               
                azimuth = (BH.Engine.Geometry.Query.SignedAngle(v1, v2, Vector.ZAxis) * (180 / Math.PI));
                if (azimuth < 0)
                    azimuth = 360 + azimuth;

            }
            return azimuth;
        }
    }

}
