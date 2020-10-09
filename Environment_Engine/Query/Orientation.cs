/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Base;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the angle to north of a given environmental object")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have its orientation queried")]
        [Input("northAngle", "The angle in degrees for north. Default is 0.0.")]
        [Input("azimuthAngle", "Set to true to return the azimuth angle from north, instead of the angle between north and the object normal. Default is false.")]
        [Output("orientation", "The orientation of the Environment Object")]
        [PreviousVersion("4.0", "BH.Engine.Environment.Query.Orientation(BH.oM.Environment.IEnvironmentObject)")]
        public static double? Orientation(this IEnvironmentObject environmentObject, double northAngle = 0.0, bool returnAzimuthAngle = false)
        {
            if (northAngle < 0 || northAngle > 360)
            {
                BH.Engine.Reflection.Compute.RecordError("North angle must be between 0 and 360 degrees");
                return null;
            }

            Vector objectNormal = BH.Engine.Geometry.Query.Normal(environmentObject.Polyline());
            if (objectNormal.X == 0 && objectNormal.Y == 0)
            {
                BH.Engine.Reflection.Compute.RecordError("When an objects normal is either directly up or down, orientation angle cannot be successfully evaluated.");
                return null;
            }
            objectNormal.Z = 0;
            
            double northAngleRadians = -((northAngle * (Math.PI / 180)) - (0.5 * Math.PI));
            Vector northVector = BH.Engine.Geometry.Create.Vector(Math.Cos(northAngleRadians), Math.Sin(northAngleRadians), 0);

            Plane xyPlane = BH.Engine.Geometry.Create.Plane(BH.Engine.Geometry.Create.Point(0, 0, 0), BH.Engine.Geometry.Create.Vector(0, 0, 1));

            double angleBetweenDegrees = 360 - (BH.Engine.Geometry.Query.Angle(northVector, objectNormal, xyPlane) * 180 / Math.PI);

            if (!returnAzimuthAngle)
            {
                if (angleBetweenDegrees > 180)
                {
                    angleBetweenDegrees = 360 - angleBetweenDegrees;
                }
            }

            return angleBetweenDegrees == 360 ? 0 : angleBetweenDegrees;
        }
    }
}
