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
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static bool IsPlanar(this Point pt, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this Vector vector, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this Plane plane, double tolerance = Tolerance.Distance)
        {
            return true;
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsPlanar(this Line line, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this Arc arc, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this Circle circle, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this Ellipse ellipse, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        [NotImplemented]
        public static bool IsPlanar(this NurbsCurve curve, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static bool IsPlanar(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsCoplanar(tolerance);
        }

        /***************************************************/

        public static bool IsPlanar(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints().IsCoplanar(tolerance);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                ****/
        /***************************************************/

        public static bool IsPlanar(this Extrusion surface, double tolerance = Tolerance.Distance)
        {
            return (surface.Direction.Length() <= tolerance || surface.Curve.IIsLinear());
        }

        /***************************************************/

        public static bool IsPlanar(this Loft surface, double tolerance = Tolerance.Distance)
        {
            List<Point> controlPts = new List<Point>();
            foreach (ICurve curve in surface.Curves)
            {
                controlPts.AddRange(curve.IControlPoints());
            }
            return controlPts.IsCoplanar(tolerance);
        }

        /***************************************************/

        [NotImplemented]
        public static bool IsPlanar(this NurbsSurface surface, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static bool IsPlanar(this Pipe surface, double tolerance = Tolerance.Distance)
        {
            return surface.Centreline.ILength() <= tolerance || surface.Radius == 0;
        }

        /***************************************************/

        public static bool IsPlanar(this PolySurface surface, double tolerance = Tolerance.Distance)
        {
            foreach (ISurface s in surface.Surfaces)
            {
                if (!s.IIsPlanar(tolerance))
                    return false;
            }
            return true;
        }


        /***************************************************/
        /**** Public Methods - Others                  ****/
        /***************************************************/

        public static bool IsPlanar(this Mesh mesh, double tolerance = Tolerance.Distance)
        {

            return mesh.Vertices.IsCoplanar(tolerance);
        }

        /***************************************************/

        public static bool IsPlanar(this CompositeGeometry group, double tolerance = Tolerance.Distance)
        {
            foreach (IGeometry element in group.Elements)
            {
                if (!element.IIsPlanar(tolerance))
                    return false;
            }
            return true;
        }


        /***************************************************/
        /**** Public Methods = Interfaces               ****/
        /***************************************************/
        
        public static bool IIsPlanar(this IGeometry geometry, double tolerance = Tolerance.Distance)
        {
            return IsPlanar(geometry as dynamic, tolerance);
        }

        /***************************************************/
    }
}
