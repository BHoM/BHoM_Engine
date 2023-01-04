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

using System.ComponentModel;
using BH.oM.Geometry;
using BH.oM.Security.Elements;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using BH.Engine.Geometry;
using System;

namespace BH.Engine.Security
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Returns the camera view cone.")]
        [Input("cameraDevice", "The CameraDevice object to compute the camera view cone for.")]
        [Output("conePolyCurve", "PolyCurve object that represents camera view cone.")]
        public static PolyCurve ViewCone(this CameraDevice cameraDevice)
        {
            if (cameraDevice == null)
                return null;

            Point cameraLocation = cameraDevice.EyePosition;
            Point targetLocation = cameraDevice.TargetPosition;
            double radius = targetLocation.Distance(cameraLocation);
            double horizontal = cameraDevice.HorizontalFieldOfView;
            double angle = 2 * Math.Atan(horizontal / 2 / radius);

            Vector direction = BH.Engine.Geometry.Create.Vector(cameraLocation, cameraDevice.TargetPosition);
            Vector startPointDir = direction.Rotate(-angle / 2, Vector.ZAxis);
            Vector endPointDir = direction.Rotate(angle / 2, Vector.ZAxis);
            Point startPoint = cameraLocation + startPointDir;
            Point endPoint = cameraLocation + endPointDir;

            Arc coneArc = BH.Engine.Geometry.Create.Arc(startPoint, targetLocation, endPoint);
            Line startLine = BH.Engine.Geometry.Create.Line(cameraLocation, startPoint);
            Line endLine = BH.Engine.Geometry.Create.Line(endPoint, cameraLocation);

            PolyCurve conePolyCurve = new PolyCurve();
            conePolyCurve.Curves = new List<ICurve>() { startLine, coneArc, endLine };

            return conePolyCurve;
        }

        /***************************************************/

    }
}

