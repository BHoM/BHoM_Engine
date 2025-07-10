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
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Security
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Returns the camera view cone.")]
        [Input("cameraDevice", "The CameraDevice object to compute the camera view cone for.")]
        [Input("coneAngle", "The angle view of the CameraDevice.")]
        [Output("conePolyCurve", "PolyCurve object that represents camera view cone.")]
        public static PolyCurve ViewCone(this CameraDevice cameraDevice, double coneAngle)
        {
            if (cameraDevice == null)
                return null;

            Point cameraLocation = cameraDevice.EyePosition;
            Point targetLocation = cameraDevice.TargetPosition;
            double coneRadius = targetLocation.Distance(cameraLocation);

            if (double.IsNaN(coneAngle))
                coneAngle = Math.PI;
      
            PolyCurve conePolyCurve = new PolyCurve();

            if (Math.Abs(Math.Abs(coneAngle) - 2* Math.PI) < 1e-6)
            {
                Arc coneArc = (Arc)BH.Engine.Geometry.Create.Circle(cameraLocation, Vector.ZAxis, coneRadius);

                conePolyCurve.Curves = new List<ICurve>() { coneArc };
            }
            else
            {
                Vector viewDirection = BH.Engine.Geometry.Create.Vector(cameraLocation, cameraDevice.TargetPosition);
                Vector startPointDir = viewDirection.Rotate(coneAngle, Vector.ZAxis);
                Point startPoint = cameraLocation + startPointDir;
                double offsetAngle = Vector.XAxis.SignedAngle(viewDirection, Vector.ZAxis);
                Point arcCenterPoint = ArcCenterPoint(cameraLocation, coneRadius, coneAngle / 2 + offsetAngle);

                Arc coneArc = BH.Engine.Geometry.Create.Arc(targetLocation, arcCenterPoint, startPoint);
                Line startLine = BH.Engine.Geometry.Create.Line(startPoint, cameraLocation);
                Line middleLine = BH.Engine.Geometry.Create.Line(cameraLocation, targetLocation);               

                conePolyCurve.Curves = new List<ICurve>() { middleLine, coneArc, startLine };
            }        
            
            return conePolyCurve;
        }

        /***************************************************/

        private static Point ArcCenterPoint(Point centerPoint, double radius, double angle)
        {
            double centerX = centerPoint.X;
            double centerY = centerPoint.Y;
            double centerZ = centerPoint.Z;

            double midpointX = centerX + radius * Math.Cos(angle);
            double midpointY = centerY + radius * Math.Sin(angle);

            Point outPoint = new Point();
            outPoint.X = midpointX;
            outPoint.Y = midpointY;
            outPoint.Z = centerZ;

            return outPoint;
        }
    }
}



