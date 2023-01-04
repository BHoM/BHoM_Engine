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

            double[,] vMatrix = new double[points.Count - 1, 3];
            for (int i = 0; i < points.Count - 1; i++)
            {
                vMatrix[i, 0] = points[i + 1].X - points[0].X;
                vMatrix[i, 1] = points[i + 1].Y - points[0].Y;
                vMatrix[i, 2] = points[i + 1].Z - points[0].Z;
            }

            double REFTolerance = vMatrix.REFTolerance(tolerance);
            double[,] rref = vMatrix.RowEchelonForm(true, REFTolerance);
            int nonZeroRows = rref.CountNonZeroRows(REFTolerance);
            return nonZeroRows < 3;
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

        public static bool IsCoplanar(this Polyline curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> cPts = curve1.DeepClone().ControlPoints;
            cPts.AddRange(curve2.DeepClone().ControlPoints);
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




