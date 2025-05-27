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

using BH.Engine.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Security.Elements;
using System;
using System.ComponentModel;

namespace BH.Engine.Security
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates CameraDevice objects from eye position, target position and horizontal field of view.")]
        [Input("eyePosition", "The point at which the Camera is installed, known as the eye position.")]
        [Input("targetPosition", "The point at which the Camera is looking at, known as the target position.")]
        [Input("horizontalFieldOfView", "The horizontal field of view of the CameraDevice.")]
        [Output("cameraDevice", "CameraDevice object.")]
        public static CameraDevice CameraDevice(Point eyePosition, Point targetPosition, double horizontalFieldOfView)
        {
            var distance = eyePosition.Distance(targetPosition);

            return new CameraDevice
            {
                EyePosition = eyePosition,
                TargetPosition = targetPosition,
                Angle = Math.Asin(horizontalFieldOfView / (2 * distance)) * 2
            };
        }

        /***************************************************/
    }
}