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
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Vector                   ****/
        /***************************************************/

        public static bool IsCoplanar(this List<Point> points, double tolerance = Tolerance.Distance)
        {
            if (points.Count < 4)
                return true;

            Plane fitPlane = points.FitPlane(tolerance);

            // Coincident points can be considered coplanar
            if (fitPlane == null)
                return true;

            return points.All(x => Math.Abs(fitPlane.Normal.DotProduct(x - fitPlane.Origin)) <= tolerance);
        }

        /***************************************************/

        public static bool IsCoplanar(this Plane plane1, Plane plane2, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            return (plane1.Origin.Distance(plane2) <= distanceTolerance && plane1.Normal.IsParallel(plane2.Normal, angleTolerance) != 0);
        }

        /***************************************************/

        public static bool IsCoplanar(this List<Plane> planes, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            for (int i = 1; i < planes.Count; i++)
            {
                if (!planes[0].IsCoplanar(planes[i], distanceTolerance, angleTolerance))
                    return false;
            }

            return true;
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsCoplanar(this Line line1, Line line2, double tolerance = Tolerance.Distance)
        {
            List<Point> cPts = new List<Point> { line1.Start, line1.End, line2.Start, line2.End };
            return cPts.IsCoplanar(tolerance);
        }

        /***************************************************/

        public static bool IsCoplanar(this List<Line> lines, double tolerance = Tolerance.Distance)
        {
            List<Point> cPts = lines.Select(x => x.Start).Union(lines.Select(x => x.End)).ToList();
            return cPts.IsCoplanar(tolerance);
        }

        /***************************************************/

        public static bool IsCoplanar(this Polyline curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> cPts = curve1.ControlPoints.ToList();
            cPts.AddRange(curve2.ControlPoints);
            return cPts.IsCoplanar(tolerance);
        }

        /***************************************************/

        public static bool IsCoplanar(this PolyCurve curve1, PolyCurve curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> pts = new List<Point> { curve1.Curves[0].IStartPoint() };

            foreach (ICurve crv in curve1.SubParts())
            {
                if (crv is Line)
                    pts.Add((crv as Line).End);
                else if (crv is Arc)
                {
                    pts.Add((crv as Arc).PointAtParameter(0.5));
                    pts.Add((crv as Arc).EndPoint());
                }
                else if (crv is Circle)
                {
                    pts.Add((crv as Circle).PointAtParameter(0.3));
                    pts.Add((crv as Circle).PointAtParameter(0.6));
                }
                else
                {
                    throw new NotImplementedException("PolyCurve consisting of type: " + crv.GetType().Name + " is not implemented for IsCoplanar.");
                }
            }

            pts.Add(curve2.Curves[0].IStartPoint());

            foreach (ICurve crv in curve2.ISubParts())
            {
                if (crv is Line)
                    pts.Add((crv as Line).End);
                else if (crv is Arc)
                {
                    pts.Add((crv as Arc).PointAtParameter(0.5));
                    pts.Add((crv as Arc).EndPoint());
                }
                else if (crv is Circle)
                {
                    pts.Add((crv as Circle).PointAtParameter(0.3));
                    pts.Add((crv as Circle).PointAtParameter(0.6));
                }
                else
                {
                    throw new NotImplementedException("PolyCurve consisting of type: " + crv.GetType().Name + " is not implemented for IsCoplanar.");
                }
            }
            
            return pts.IsCoplanar(tolerance);
        }

        /***************************************************/

        public static bool IIsCoplanar(this ICurve curve1, ICurve curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> pts = new List<Point> { curve1.IStartPoint() };

            foreach (ICurve crv in curve1.ISubParts())
            {
                if (crv is Line)
                    pts.Add((crv as Line).End);
                else if (crv is Arc)
                {
                    pts.Add((crv as Arc).PointAtParameter(0.5));
                    pts.Add((crv as Arc).EndPoint());
                }
                else if (crv is Circle)
                {
                    pts.Add((crv as Circle).PointAtParameter(0.3));
                    pts.Add((crv as Circle).PointAtParameter(0.6));
                }
                else
                {
                    throw new NotImplementedException("ICurve of type: " + crv.GetType().Name + " is not implemented for IsCoplanar.");
                }
            }

            pts.Add(curve2.IStartPoint());

            foreach (ICurve crv in curve2.ISubParts())
            {
                if (crv is Line)
                    pts.Add((crv as Line).End);
                else if (crv is Arc)
                {
                    pts.Add((crv as Arc).PointAtParameter(0.5));
                    pts.Add((crv as Arc).EndPoint());
                }
                else if (crv is Circle)
                {
                    pts.Add((crv as Circle).PointAtParameter(0.3));
                    pts.Add((crv as Circle).PointAtParameter(0.6));
                }
                else
                {
                    throw new NotImplementedException("ICurve of type: " + crv.GetType().Name + " is not implemented for IsCoplanar.");
                }
            }

            return pts.IsCoplanar(tolerance);
        }

        /***************************************************/        
    }
}






