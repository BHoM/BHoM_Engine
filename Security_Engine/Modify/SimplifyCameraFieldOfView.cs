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

using System.Collections.Generic;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using BH.oM.Security.Elements;
using System.Linq;
using BH.Engine.Base;
using System;

namespace BH.Engine.Security
{
    public static partial class Modify
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Returns the simplify PolyCurve generated by Camera Field of View method.")]
        [Input("cameraFieldOfView", "Camera Field of View PolyCyrve to simplify.")]
        [Input("cameraDevice", "CameraDevice object.")]
        [Input("distanceTolerance", "Distance tolerance for the method.")]
        [Input("angleTolerance", "Angular tolerance for the method.")]
        [Output("simplifiedPolyCurve", "Simplified PolyCurve object.")]
        public static PolyCurve SimplifyCameraFieldOfView(this PolyCurve cameraFieldOfView, CameraDevice cameraDevice, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            if (cameraFieldOfView == null || cameraDevice == null)
                return null;

            //convert to polyline and simplify
            List<Line> cameraLines = new List<Line>();

            foreach (ICurve curve in cameraFieldOfView.SubParts())
            {
                if (curve is Line)
                    cameraLines.Add(curve as Line);
                else
                {
                    Line line = Geometry.Create.Line(curve.IStartPoint(), curve.IEndPoint());
                    cameraLines.Add(line);
                }
            }

            Polyline cameraPolyline = Geometry.Create.Polyline(cameraLines);
            cameraPolyline = cameraPolyline.Simplify(distanceTolerance, angleTolerance);

            //camera cone arc
            PolyCurve cameraCone = cameraDevice.ViewCone();
            Arc coneArc = cameraCone.Curves[1] as Arc;

            //create simplified polycurve
            PolyCurve simplifiedPolyCurve = new PolyCurve();
            foreach (Line line in cameraPolyline.SubParts())
            {
                Point startPoint = line.Start;
                Point endPoint = line.End;

                if ((startPoint.Distance(coneArc) < distanceTolerance) && (endPoint.Distance(coneArc) < distanceTolerance))
                {
                    double p1Param = coneArc.ParameterAtPoint(startPoint, distanceTolerance);
                    double p2Param = coneArc.ParameterAtPoint(endPoint, distanceTolerance);
                    double p3Param = (p1Param + p2Param) / 2;
                    Point pt3 = coneArc.PointAtParameter(p3Param);

                    if (!cameraFieldOfView.IsContaining(new List<Point>() { pt3 }, true, distanceTolerance))
                    {
                        simplifiedPolyCurve.Curves.Add(line);
                        continue;
                    }

                    double startAngle = coneArc.EndAngle * p1Param;
                    double endAngle = coneArc.EndAngle * p2Param;
                    Arc newArc = Geometry.Create.Arc(coneArc.CoordinateSystem, coneArc.Radius, startAngle, endAngle);

                    simplifiedPolyCurve.Curves.Add(newArc);
                }
                else
                {
                    simplifiedPolyCurve.Curves.Add(line);
                }
            }

            return simplifiedPolyCurve;
        }
    }
}


