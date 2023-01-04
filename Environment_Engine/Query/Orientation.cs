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
using System.ComponentModel;

using BH.oM.Environment;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the angle to north of a given environmental object on an xyPlane")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have its orientation queried")]
        [Input("northAngle", "The angle in radians for north. Default is 0.")]
        [Input("returnAzimuthAngle", "Set to true to return the azimuth angle from north, instead of the angle between north and the object normal. Default is false.")]
        [Output("orientation", "The orientation of the Environment Object")]
        public static double? Orientation(this IEnvironmentObject environmentObject, double northAngle = 0.0, bool returnAzimuthAngle = false)
        {
            if(environmentObject == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the orientation of a null environment object.");
                return null;
            }

            northAngle += Math.PI / 2; // Correct northAngle to be 0 at North, rather than East

            Plane xyPlane = BH.Engine.Geometry.Create.Plane(BH.Engine.Geometry.Create.Point(0, 0, 0), BH.Engine.Geometry.Create.Vector(0, 0, 1));
            Vector northVector = BH.Engine.Geometry.Create.Vector(Math.Cos(northAngle), Math.Sin(northAngle), 0);
            
            Vector objectNormal = BH.Engine.Geometry.Query.Normal(environmentObject.Polyline());
            if (objectNormal.X == 0 && objectNormal.Y == 0)
            {
                BH.Engine.Base.Compute.RecordError("When an objects normal is either directly up or down, orientation angle cannot be successfully evaluated.");
                return null;
            }

            objectNormal.Z = 0;

            return returnAzimuthAngle ? Math.PI * 2 - BH.Engine.Geometry.Query.Angle(northVector, objectNormal, xyPlane) : BH.Engine.Geometry.Query.Angle(northVector, objectNormal);
        }
    }
}



