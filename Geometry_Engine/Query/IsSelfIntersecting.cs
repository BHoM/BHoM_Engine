/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsSelfIntersecting(this Line curve, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        public static bool IsSelfIntersecting(this Arc curve, double tolerance = Tolerance.Distance)
        {
            return Math.Abs(curve.StartAngle - curve.EndAngle) > 2 * Math.PI;
        }

        /***************************************************/

        public static bool IsSelfIntersecting(this Circle curve, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        public static bool IsSelfIntersecting(this Ellipse curve, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        public static bool IsSelfIntersecting(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            double sqTolerance = tolerance * tolerance;

            List<Line> curves = curve.SubParts().Where(x => x.SquareLength() > sqTolerance).ToList();
            if (curves.Count < 2)
                return false;

            List<BoundingBox> boxes = curves.Select(x => x.Bounds()).ToList();
            bool closed = curve.IsClosed();
            for (int i = 0; i < curves.Count - 1; i++)
            {
                for (int j = i + 1; j < curves.Count; j++)
                {
                    if (boxes[i].IsInRange(boxes[j]))
                    {
                        foreach (Point intPt in curves[i].LineIntersections(curves[j]))
                        {
                            if (j == i + 1 && intPt.SquareDistance(curves[i].End) <= tolerance && intPt.SquareDistance(curves[j].Start) <= tolerance)
                                continue;
                            else if (closed && i == 0 && j == curves.Count - 1 && intPt.SquareDistance(curves[i].Start) <= tolerance && intPt.SquareDistance(curves[j].End) <= tolerance)
                                continue;
                            else
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        /***************************************************/

        public static bool IsSelfIntersecting(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            List<ICurve> curves = curve.SubParts().Where(x => x.ILength() > tolerance).ToList();
            if (curves.Count == 0)
                return false;

            if (curves.Count == 1)
                return curves[0].IIsSelfIntersecting(tolerance);

            List<BoundingBox> boxes = curves.Select(x => x.IBounds()).ToList();
            bool closed = curve.IsClosed();
            double sqTolerance = tolerance * tolerance;
            for (int i = 0; i < curves.Count - 1; i++)
            {
                for (int j = i + 1; j < curves.Count; j++)
                {
                    if (boxes[i].IsInRange(boxes[j]))
                    {
                        foreach (Point intPt in curves[i].ICurveIntersections(curves[j], tolerance))
                        {
                            if (j == i + 1 && intPt.SquareDistance(curves[i].IEndPoint()) <= tolerance && intPt.SquareDistance(curves[j].IStartPoint()) <= tolerance)
                                continue;
                            else if (closed && i == 0 && j == curves.Count - 1 && intPt.SquareDistance(curves[i].IStartPoint()) <= tolerance && intPt.SquareDistance(curves[j].IEndPoint()) <= tolerance)
                                continue;
                            else
                                return true;
                        }
                    }
                }
            }

            return false;
        }


        /***************************************************/
        /**** Public Methods = Interfaces               ****/
        /***************************************************/

        public static bool IIsSelfIntersecting(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            return IsSelfIntersecting(curve as dynamic, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsSelfIntersecting(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException($"IsSelfIntersecting is not implemented for ICurves of type: {curve.GetType().Name}.");
        }

        /***************************************************/
    }
}

