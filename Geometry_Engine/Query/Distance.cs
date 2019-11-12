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

using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Distance(this Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /***************************************************/

        public static double SquareDistance(this Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        /***************************************************/

        public static double Distance(this Vector a, Vector b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /***************************************************/

        public static double SquareDistance(this Vector a, Vector b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        /***************************************************/

        public static double Distance(this Point point, Plane plane)
        {
            Vector normal = plane.Normal.Normalise();
            return Math.Abs(normal.DotProduct(point - plane.Origin));
        }


        /***************************************************/
        /****       Public Methods - Point/Curve        ****/
        /***************************************************/

        public static double Distance(this Point point, Line line, bool infiniteSegment = false)
        {
            return point.Distance(line.ClosestPoint(point, infiniteSegment));
        }

        /***************************************************/

        public static double SquareDistance(this Point point, Line line, bool infiniteSegment = false)
        {
            return point.SquareDistance(line.ClosestPoint(point, infiniteSegment));
        }

        /***************************************************/

        public static double Distance(this Point point, Arc arc)
        {
            return point.Distance(arc.ClosestPoint(point));
        }

        /***************************************************/

        public static double Distance(this Point point, Circle circle)
        {
            return point.Distance(circle.ClosestPoint(point));
        }

        /***************************************************/

        [NotImplemented]
        public static double Distance(this Point point, Ellipse ellipse)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static double Distance(this Point point, Polyline curve)
        {
            return point.Distance(curve.ClosestPoint(point));
        }

        /***************************************************/

        public static double Distance(this Point point, PolyCurve curve)
        {
            return point.Distance(curve.ClosestPoint(point));
        }

        /***************************************************/

        [NotImplemented]
        public static double Distance(this Point point, NurbsCurve curve)
        {
            throw new NotImplementedException();
        }

        /******************************************/
        /****            IElement              ****/
        /******************************************/

        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        public static double Distance(this IElement0D element0D, IElement0D refElement)
        {
            return Distance(element0D.IGeometry(), refElement.IGeometry());
        }

        /******************************************/

        public static double Distance(this IElement0D element0D, IElement1D refElement)
        {
            Point point = element0D.IGeometry();
            return Distance(point, refElement.ClosestPointOn(point));
        }

        /******************************************/

        public static double Distance(this IElement0D element0D, IElement2D refElement)
        {
            Point point = element0D.IGeometry();
            return Distance(point, refElement.ClosestPointOn(point));
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static double Distance(this IElement1D element1D, IElement0D refElement)
        {
            return refElement.Distance(element1D);
        }

        /******************************************/

        [NotImplemented]
        public static double Distance(this IElement1D element1D, IElement1D refElement)
        {
            throw new NotImplementedException();
        }

        /******************************************/

        [NotImplemented]
        public static double Distance(this IElement1D element1D, IElement2D refElement)
        {
            throw new NotImplementedException();
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static double Distance(this IElement2D element2D, IElement0D refElement)
        {
            return refElement.Distance(element2D);
        }

        /******************************************/

        public static double Distance(this IElement2D element2D, IElement1D refElement)
        {
            return refElement.Distance(element2D);
        }

        /******************************************/

        [NotImplemented]
        public static double Distance(this IElement2D element2D, IElement2D refElement)
        {
            throw new NotImplementedException();
        }


        /******************************************/
        /****        Interface methods         ****/
        /******************************************/

        public static double IDistance(this IElement element1, IElement element2)
        {
            return Distance(element1 as dynamic, element2 as dynamic);
        }


        /***************************************************/
        /****       Public Methods - Curve/Curve        ****/
        /***************************************************/

        public static double Distance(this ICurve curve1, ICurve curve2, double tolerance = Tolerance.Distance)
        {
            BH.oM.Reflection.Output<Point, Point> results = curve1.ICurveProximity(curve2, tolerance);
            return results.Item1.Distance(results.Item2);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IDistance(this Point point, ICurve curve)
        {
            return Distance(point, curve as dynamic);
        }

        /***************************************************/
    }
}
