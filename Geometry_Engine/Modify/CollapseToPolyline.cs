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

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Polyline CollapseToPolyline(this Arc curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return new Polyline { ControlPoints = curve.CollapseToPolylineVertices(angleTolerance, maxSegmentCount) };
        }

        /***************************************************/

        public static Polyline CollapseToPolyline(this Circle curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return new Polyline { ControlPoints = curve.CollapseToPolylineVertices(angleTolerance, maxSegmentCount) };
        }

        /***************************************************/

        public static Polyline CollapseToPolyline(this Line curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return new Polyline { ControlPoints = curve.CollapseToPolylineVertices(angleTolerance, maxSegmentCount) };
        }

        /***************************************************/

        public static Polyline CollapseToPolyline(this Polyline curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.ToList() };
        }

        /***************************************************/

        public static Polyline CollapseToPolyline(this PolyCurve curve, double angleTolerance, int maxSegmentCount = 100)
        {
            List<Point> controlPoints = new List<Point> { curve.StartPoint() };
            foreach (ICurve c in curve.SubParts()) controlPoints.AddRange(c.ICollapseToPolylineVertices(angleTolerance, maxSegmentCount).Skip(1));
            return new Polyline { ControlPoints = controlPoints };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Polyline ICollapseToPolyline(this ICurve curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return CollapseToPolyline(curve as dynamic, angleTolerance, maxSegmentCount);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static Polyline CollapseToPolyline(this ICurve curve, double angleTolerance, int maxSegmentCount = 100)
        {
            Base.Compute.RecordError($"CollapseToPolyline is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<Point> CollapseToPolylineVertices(this Arc curve, double angleTolerance, int maxSegmentCount = 100)
        {
            int segmentCount = curve.CollapseToPolylineCount(angleTolerance, maxSegmentCount);
            double step = 1.0 / segmentCount;
            double param = step;

            List<Point> result = new List<Point> { curve.StartPoint() };
            for (int i = 0; i < segmentCount; i++)
            {
                result.Add(curve.PointAtParameter(param));
                param += step;
            }

            return result;
        }

        /***************************************************/

        private static int CollapseToPolylineCount(this Arc curve, double angleTolerance, int maxSegmentCount = 100)
        {
            double angle = curve.Angle();
            double factor = Math.Min(Math.PI * 0.25, Math.Max(angle * 0.5 / maxSegmentCount, angleTolerance));
            return System.Convert.ToInt32(Math.Ceiling(angle * 0.5 / factor));
        }

        /***************************************************/

        private static List<Point> CollapseToPolylineVertices(this Circle curve, double angleTolerance, int maxSegmentCount = 100)
        {
            int segmentCount = curve.CollapseToPolylineCount(angleTolerance, maxSegmentCount);
            double step = 1.0 / segmentCount;
            double param = step;

            List<Point> result = new List<Point> { curve.StartPoint() };
            for (int i = 0; i < segmentCount; i++)
            {
                result.Add(curve.PointAtParameter(param));
                param += step;
            }

            return result;
        }

        /***************************************************/

        private static int CollapseToPolylineCount(this Circle curve, double angleTolerance, int maxSegmentCount = 100)
        {
            double factor = Math.Min(Math.PI * 0.25, Math.Max(Math.PI / maxSegmentCount, angleTolerance));
            return System.Convert.ToInt32(Math.Ceiling(Math.PI / factor));
        }

        /***************************************************/

        private static List<Point> CollapseToPolylineVertices(this Line curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        private static List<Point> CollapseToPolylineVertices(this Polyline curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return curve.ControlPoints.ToList();
        }

        /***************************************************/

        private static List<Point> ICollapseToPolylineVertices(this ICurve curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return CollapseToPolylineVertices(curve as dynamic, angleTolerance, maxSegmentCount);
        }

        /***************************************************/
    }
}




