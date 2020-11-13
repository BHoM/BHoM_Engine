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

using System.ComponentModel;
using System;
using System.Collections.Generic;
using BH.oM.Geometry;
using BH.oM.MEP.System;
using BH.oM.MEP.Fixtures;
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /****             Public Methods                ****/
        /***************************************************/
        
        public static IGeometry Geometry(this IFlow flowObj)
        {
            return new Line { Start = flowObj.StartPoint, End = flowObj.EndPoint};
        }

        /***************************************************/

        [Description("Gets the camera's geometry as a closed ICurve cone-shape. Method required for automatic display in UI packages.")]
        [Input("cameraDevice", "Camera to get the ICurve from.")]
        [Output("icurve", "The geometry of the Camera.")]
        public static ICurve Geometry(this CameraDevice cameraDevice)
        {
            Vector direction = BH.Engine.Geometry.Create.Vector(
                BH.Engine.Geometry.Create.Point(cameraDevice.EyePosition.X, cameraDevice.EyePosition.Y, 0),
                BH.Engine.Geometry.Create.Point(cameraDevice.TargetPosition.X, cameraDevice.TargetPosition.Y, 0)).Normalise();

            Vector perpendicular = direction.Rotate((Math.PI / 180) * 90, Vector.ZAxis);

            List<Point> vertices = new List<Point>();
            Point point1 = cameraDevice.EyePosition.Clone();
            Point point2 = cameraDevice.TargetPosition.Clone().Translate(perpendicular * (cameraDevice.HorizontalFieldOfView / 2));
            Point point3 = cameraDevice.TargetPosition.Clone().Translate(perpendicular * ((cameraDevice.HorizontalFieldOfView / 2)) * -1);
            vertices.Add(point1);
            vertices.Add(point2);
            vertices.Add(point3);
            vertices.Add(point1);

            Polyline polyline = BH.Engine.Geometry.Create.Polyline(vertices);

            return polyline;
        }

        /***************************************************/
    }
}