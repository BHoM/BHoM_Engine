/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using Engine_Explore.BHoM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHG = BHoM.Geometry;

namespace Engine_Explore.Engine.Geometry
{
    public static class Bound
    {
        public static BoundingBox Calculate(Point pt)
        {
            return new BoundingBox(pt, pt);
        }

        /***************************************************/

        public static BoundingBox Calculate(Vector vector)
        {
            Point pt = new Point(vector.X, vector.Y, vector.Z);
            return new BoundingBox(pt, pt);
        }

        /***************************************************/

        public static BoundingBox Calculate(Line line)
        {
            Point s = line.Start;
            Point e = line.End;
            Point min = new Point(Math.Min(s.X, e.X), Math.Min(s.Y, e.Y), Math.Min(s.Z, e.Z));
            Point max = new Point(Math.Max(s.X, e.X), Math.Max(s.Y, e.Y), Math.Max(s.Z, e.Z));
            return new BoundingBox(min, max);
        }

        /***************************************************/

        public static BoundingBox Calculate(Polyline line)
        {
            Point pt = line.ControlPoints[0];
            double minX = pt.X;
            double minY = pt.Y;
            double minZ = pt.Z;
            double maxX = minX;
            double maxY = minY;
            double maxZ = minZ;

            for (int i = line.ControlPoints.Count - 1; i > 0; i--)
            {
                pt = line.ControlPoints[i];
                if (pt.X < minX) minX = pt.X;
                if (pt.Y < minY) minY = pt.Y;
                if (pt.Z < minZ) minZ = pt.Z;
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y > maxY) maxY = pt.Y;
                if (pt.Z > maxZ) maxZ = pt.Z;
            }

            return new BoundingBox(new Point(minX, minY, minZ), new Point(maxX, maxY, maxZ));
        }

        /***************************************************/

        public static BoundingBox Calculate(Circle circle)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static BoundingBox Calculate(Arc arc)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static BoundingBox Calculate(NurbCurve curve)
        {
            Point pt = curve.ControlPoints[0];
            double minX = pt.X;
            double minY = pt.Y;
            double minZ = pt.Z;
            double maxX = minX;
            double maxY = minY;
            double maxZ = minZ;

            for (int i = curve.ControlPoints.Count - 1; i > 0; i--)
            {
                pt = curve.ControlPoints[i];
                if (pt.X < minX) minX = pt.X;
                if (pt.Y < minY) minY = pt.Y;
                if (pt.Z < minZ) minZ = pt.Z;
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y > maxY) maxY = pt.Y;
                if (pt.Z > maxZ) maxZ = pt.Z;
            }

            return new BoundingBox(new Point(minX, minY, minZ), new Point(maxX, maxY, maxZ));
        }

        /***************************************************/

        public static BoundingBox Calculate(NurbCurveB curve)
        {
            double[] points = curve.ControlPoints;
            int[] minMax = BHG.CollectionUtils.MaxMinIndices(points, 3);
            return new BoundingBox(
                new Point(points[minMax[0]], points[minMax[1]], points[minMax[2]]), 
                new Point(points[minMax[3]], points[minMax[4]], points[minMax[5]])
                );
        }

        /***************************************************/

        public static bool InRange(BoundingBox box1, BoundingBox box2, double tolerance = 0)
        {
            return ( box1.Min.X <= box2.Max.X + tolerance && box2.Min.X <= box1.Max.X + tolerance &&
                     box1.Min.Y <= box2.Max.Y + tolerance && box2.Min.Y <= box1.Max.Y + tolerance &&
                     box1.Min.Z <= box2.Max.Z + tolerance && box2.Min.Z <= box1.Max.Z + tolerance );
        }


        /***************************************************/
        /****  Speed Test Alternatives (all worse)      ****/
        /***************************************************/

        public static BoundingBox Calculate2(NurbCurve curve)
        {
            Point min = curve.ControlPoints[0];
            Point max = curve.ControlPoints[0];

            for (int i = curve.ControlPoints.Count - 1; i > 0; i--)
            {
                Point pt = curve.ControlPoints[i];
                min.X = Math.Min(min.X, pt.X);
                min.Y = Math.Min(min.Y, pt.Y);
                min.Z = Math.Min(min.Z, pt.Z);
                max.X = Math.Max(max.X, pt.X);
                max.Y = Math.Max(max.Y, pt.Y);
                max.Z = Math.Max(max.Z, pt.Z);
            }

            return new BoundingBox(min, max);
        }

        /***************************************************/

        public static BoundingBox Calculate3(NurbCurve curve)
        {
            List<Point> pts = curve.ControlPoints;
            return new BoundingBox(
                new Point(pts.Min(pt => pt.X), pts.Min(pt => pt.Y), pts.Min(pt => pt.Z)),
                new Point(pts.Max(pt => pt.X), pts.Max(pt => pt.Y), pts.Max(pt => pt.Z))
                );
        }

        /***************************************************/

        public static BoundingBox Calculate4(NurbCurve curve)
        {
            List<Point> pts = curve.ControlPoints;
            return new BoundingBox(
                pts.Aggregate((a, b) => new Point(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z))),
                pts.Aggregate((a, b) => new Point(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z)))
                );
        }

    }
}
