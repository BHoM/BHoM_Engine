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
                    Line line = BH.Engine.Geometry.Create.Line(curve.IStartPoint(), curve.IEndPoint());
                    cameraLines.Add(line);
                }
            }

            Polyline cameraPolyline = BH.Engine.Geometry.Create.Polyline(cameraLines);

            cameraPolyline = cameraPolyline.Simplify(distanceTolerance, angleTolerance);

            //camera cone arc
            PolyCurve cameraCone = cameraDevice.ViewCone(2 * Math.PI);

            Arc coneArc = cameraCone.Curves[0] as Arc;

            //create simplified polycurve
            PolyCurve simplifiedPolyCurve = new PolyCurve();

            ICurve lastCurve = null;

            Circle circle = BH.Engine.Geometry.Create.Circle(cameraDevice.EyePosition, coneArc.Radius);

            foreach (Line line in cameraPolyline.SubParts())
            {
                Point startPoint = line.Start;
                Point endPoint = line.End;

                if ((startPoint.Distance(coneArc) < distanceTolerance) && (endPoint.Distance(coneArc) < distanceTolerance))
                {
                    //check if line is length is equal to the diameter of the cone arc, if so, add it as it is; apply only for 180 degree angles
                    if (Math.Abs(cameraDevice.Angle - Math.PI) < angleTolerance && Math.Abs(startPoint.Distance(endPoint) - 2 * coneArc.Radius) < distanceTolerance)
                    {
                        simplifiedPolyCurve.Curves.Add(line);
                        lastCurve = line;
                        continue;
                    }                    

                    List<ICurve> newArcList = circle.SplitAtPoints(new List<Point> { startPoint, endPoint }, distanceTolerance);

                    double p1Param = (newArcList[0] as Arc).ParameterAtPoint(startPoint, distanceTolerance);
                    double p2Param = (newArcList[0] as Arc).ParameterAtPoint(endPoint, distanceTolerance);

                    Arc newArc;

                    if (p1Param < distanceTolerance && p2Param - 1 < distanceTolerance)
                        newArc = (newArcList[0] as Arc);
                    else
                        newArc = (newArcList[1] as Arc);
                    
                    if (lastCurve is Arc && !(cameraPolyline.SubParts().Count == 2))
                    {
                        simplifiedPolyCurve.Curves.Add(line);
                        lastCurve = line;
                    }
                    else
                    {
                        simplifiedPolyCurve.Curves.Add(newArc);
                        lastCurve = newArc;
                    }                                                           
                }
                else
                {
                    lastCurve = line;
                    simplifiedPolyCurve.Curves.Add(line);
                }
            }

            //if polyCurve is not closed, try to close it
            EnsurePolyCurveIsClosed(simplifiedPolyCurve);

            //check if simplifiedPolyCurve has two curves only, if so make sure to convert smaller arc to a line
            CheckForTwoCurvesOnly(simplifiedPolyCurve);

            return simplifiedPolyCurve;
        }

        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        private static void CheckForTwoCurvesOnly(PolyCurve simplifiedPolyCurve)
        {
            if (simplifiedPolyCurve.Curves.Count == 2)
            {
                if (simplifiedPolyCurve.Curves[0] is Arc && simplifiedPolyCurve.Curves[1] is Arc)
                {
                    Arc arc1 = simplifiedPolyCurve.Curves[0] as Arc;
                    Arc arc2 = simplifiedPolyCurve.Curves[1] as Arc;

                    if (arc1.Length() > arc2.Length())
                    {
                        Point startPoint = arc1.EndPoint();
                        Point endPoint = arc1.StartPoint();

                        Line line = BH.Engine.Geometry.Create.Line(startPoint, endPoint);

                        simplifiedPolyCurve.Curves.Clear();

                        simplifiedPolyCurve.Curves.Add(arc1);
                        simplifiedPolyCurve.Curves.Add(line);
                    }
                    else
                    {
                        Point startPoint = arc2.EndPoint();
                        Point endPoint = arc2.StartPoint();

                        Line line = BH.Engine.Geometry.Create.Line(startPoint, endPoint);

                        simplifiedPolyCurve.Curves.Clear();

                        simplifiedPolyCurve.Curves.Add(line);
                        simplifiedPolyCurve.Curves.Add(arc2);
                    }
                }
            }
        }

        /***************************************************/
        private static void EnsurePolyCurveIsClosed(PolyCurve simplifiedPolyCurve, double distanceTolerance = Tolerance.Distance)
        {
            if (simplifiedPolyCurve.IsClosed())
                return;

            for (int i = 0; i < simplifiedPolyCurve.Curves.Count - 1; i++)
            {
                ICurve currentCurve = simplifiedPolyCurve.Curves[i];
                ICurve nextCurve = simplifiedPolyCurve.Curves[i + 1];

                Point currentEnd = currentCurve.IEndPoint();
                Point nextStart = nextCurve.IStartPoint();

                if (currentEnd.Distance(nextStart) > distanceTolerance)
                {
                    if (nextCurve is Arc)
                        simplifiedPolyCurve.Curves[i] = BH.Engine.Geometry.Create.Line(currentCurve.IStartPoint(), nextStart);

                    if (currentCurve is Arc)
                        simplifiedPolyCurve.Curves[i + 1] = BH.Engine.Geometry.Create.Line(nextStart, nextCurve.IEndPoint());
                }
            }

            if (simplifiedPolyCurve.Curves[0] is Arc)
            {
                Point startPoint = ((Arc)simplifiedPolyCurve.Curves[simplifiedPolyCurve.Curves.Count - 1]).StartPoint();
                Point endPoint = simplifiedPolyCurve.Curves[0].IStartPoint();
                simplifiedPolyCurve.Curves[simplifiedPolyCurve.Curves.Count - 1] = BH.Engine.Geometry.Create.Line(startPoint, endPoint);
            }

            if (simplifiedPolyCurve.Curves[simplifiedPolyCurve.Curves.Count - 1] is Arc)
            {
                Point startPoint = ((Arc)simplifiedPolyCurve.Curves[simplifiedPolyCurve.Curves.Count - 1]).EndPoint();
                Point endPoint = simplifiedPolyCurve.Curves[0].IEndPoint();
                simplifiedPolyCurve.Curves[0] = BH.Engine.Geometry.Create.Line(startPoint, endPoint);
            }
        }
    }
}


