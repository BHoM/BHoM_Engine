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

using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /******************************************************************/
        /*** Those methods were written to work with offset method only ***/
        /***        They were not tested with anything else!            ***/
        /******************************************************************/

        /***************************************************/
        /**** Private Methods - Curves                  ****/
        /***************************************************/

        private static Line Trim(this Line curve, double start = 0.0, double end = 0.0)
        {
            return curve.Extend(-start, -end);
        }

        /***************************************************/

        private static Line Trim(this Line curve, Point startPoint, Point endPoint)
        {
            return new Line { Start = startPoint, End = endPoint };
        }

        /***************************************************/

        private static Arc Trim(this Arc curve, Point startPoint, Point endPoint)
        {
            Cartesian coordinateSystem = new Cartesian(curve.CoordinateSystem.Origin,
                (startPoint - curve.CoordinateSystem.Origin).Normalise(),
                (startPoint - curve.CoordinateSystem.Origin).Normalise().CrossProduct(-(curve.CoordinateSystem.Z)),
                curve.CoordinateSystem.Z);
            Vector stVec = startPoint - curve.Centre();
            Vector enVec = endPoint - curve.Centre();
            double endAngle = stVec.Angle(enVec, (Plane)coordinateSystem);
            return new Arc
            {
                CoordinateSystem = coordinateSystem,
                Radius = curve.Radius,
                StartAngle = 0,
                EndAngle = endAngle
            };
        }

        /***************************************************/

        private static Circle Trim(this Circle curve, Point startPoint, Point endPoint)
        {
            Reflection.Compute.RecordNote("Can't Trim closed models.");
            return curve;
        }

        /***************************************************/

        private static Circle Trim(this Circle curve, double startPoint, double endPoint)
        {
            Reflection.Compute.RecordNote("Can't Trim closed models.");
            return curve;
        }

        /***************************************************/

        private static Ellipse Trim(this Ellipse curve, Point startPoint, Point endPoint)
        {
            Reflection.Compute.RecordNote("Can't Trim closed models.");
            return curve;
        }

        /***************************************************/

        private static Ellipse Trim(this Ellipse curve, double startPoint, double endPoint)
        {
            Reflection.Compute.RecordNote("Can't Trim closed models.");
            return curve;
        }

        /***************************************************/

        [NotImplemented]
        private static NurbsCurve Trim(this NurbsCurve curve, Point startPoint, Point endPoint)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        private static NurbsCurve Trim(this NurbsCurve curve, double startPoint, double endPoint)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static Polyline Trim(this Polyline curve, Point startPoint, Point endPoint)
        {
            if (curve.IsClosed())
            {
                Reflection.Compute.RecordNote("Can't Trim closed models.");
                return curve;
            }
            List<Point> result = new List<Point>();
            result = curve.ControlPoints;
            result[0] = startPoint;
            result[result.Count - 1] = endPoint;
            return new Polyline
            {
                ControlPoints = result
            };
        }

        /***************************************************/

        private static Polyline Trim(this Polyline curve, double start, double end)
        {
            if (curve.IsClosed())
            {
                Reflection.Compute.RecordNote("Can't Trim closed models.");
                return curve;
            }
            Line first = new Line { Start = curve.ControlPoints[0], End = curve.ControlPoints[1] };
            Line last = new Line { Start = curve.ControlPoints[curve.ControlPoints.Count - 2], End = curve.ControlPoints[curve.ControlPoints.Count - 1] };
            first.Extend(start);
            last.Extend(0, end);
            List<Point> result = new List<Point>();
            result = curve.ControlPoints;
            result[0] = first.Start;
            result[result.Count - 1] = last.End;
            return new Polyline
            {
                ControlPoints = result
            };
        }

        /***************************************************/

        private static PolyCurve Trim(this PolyCurve curve, Point startPoint, Point endPoint)
        {
            if (curve.IsClosed())
            {
                Reflection.Compute.RecordNote("Can't Trim closed models.");
                return curve;
            }
            List<ICurve> result = new List<ICurve>();
            result = curve.Curves;
            result[0] = result[0].ITrim(startPoint, result[0].IEndPoint());
            result[result.Count - 1] = result[result.Count - 1].ITrim(result[result.Count - 1].IStartPoint(), endPoint);
            return new PolyCurve
            {
                Curves = result
            };

        }

        /***************************************************/

        private static PolyCurve Trim(this PolyCurve curve, double start, double end)
        {
            if (curve.IsClosed())
            {
                Reflection.Compute.RecordNote("Can't Trim closed models.");
                return curve;
            }
            List<ICurve> result = new List<ICurve>();
            result = curve.Curves;
            result[0] = result[0].ITrim(start, 0);
            result[result.Count - 1] = result[result.Count - 1].ITrim(0, end);
            return new PolyCurve
            {
                Curves = result
            };

        }


        /***************************************************/
        /**** private Methods - Interfaces               ****/
        /***************************************************/

        private static ICurve ITrim(this ICurve curve, Point startPoint, Point endPoint)
        {
            return Trim(curve as dynamic, startPoint, endPoint);
        }

        /***************************************************/

        private static ICurve ITrim(this ICurve curve, double start, double end)
        {
            return Trim(curve as dynamic, start, end);
        }
     
        /***************************************************/

    }
}
