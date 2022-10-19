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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static double[] IGeometryHash(this IBHoMObject bhomObj)
        {
            IGeometry igeom = bhomObj.IGeometry();

            return GeometryHash(igeom as dynamic);
        }

        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        private static double[] GeometryHash(this ICurve curve)
        {
            Polyline polyLine = curve.IRationalise();

            return polyLine.ControlPoints.SelectMany(p => p.GeometryHash()).ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this IEnumerable<ICurve> curves)
        {
            return curves.SelectMany(c => c.GeometryHash()).ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this Point obj, double typeTranslationFactor = 0)
        {
            return obj.ToDoubleArray(typeTranslationFactor);
        }

        /***************************************************/

        private static double[] GeometryHash(this Plane obj, double typeTranslationFactor = 3)
        {
            return obj.Origin.ToDoubleArray(typeTranslationFactor);
        }

        /***************************************************/

        private static double[] GeometryHash(this ISurface obj, double typeTranslationFactor = 3)
        {
            return GeometryHash(obj as dynamic, typeTranslationFactor);
        }

        /***************************************************/

        private static double[] GeometryHash(this PlanarSurface obj, double typeTranslationFactor = 3)
        {
            return obj.ExternalBoundary.GeometryHash().Concat(obj.InternalBoundaries.SelectMany(ib => ib.GeometryHash())).ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this Extrusion obj, double typeTranslationFactor = 3)
        {
            return obj.Curve.GeometryHash();
        }

        /***************************************************/

        private static double[] GeometryHash(this Loft obj, double typeTranslationFactor = 3)
        {
            return obj.Curves.GeometryHash();
        }

        /***************************************************/

        private static double[] GeometryHash(this NurbsSurface obj, double typeTranslationFactor = 3)
        {
            return obj.ControlPoints.ToDoubleArray(typeTranslationFactor);
        }

        /***************************************************/

        private static double[] GeometryHash(this Pipe obj, double typeTranslationFactor = 3)
        {
            return obj.Centreline.GeometryHash();
        }

        /***************************************************/

        private static double[] GeometryHash(this PolySurface obj, double typeTranslationFactor = 3)
        {
            return obj.Surfaces.SelectMany(s => s.GeometryHash()).ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this SurfaceTrim obj, double typeTranslationFactor = 3)
        {
            return obj.Curve3d.GeometryHash().Concat(obj.Curve2d.GeometryHash()).ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this object obj)
        {
            BH.Engine.Base.Compute.RecordError($"Could not find a {nameof(GeometryHash)} method for type {obj.GetType().FullName}.");
            return new double[] { };
        }

        /***************************************************/

        private static double[] ToDoubleArray(this Point p, double typeTranslationFactor)
        {
            return new double[] { p.X + typeTranslationFactor, p.Y + typeTranslationFactor, p.Z + typeTranslationFactor };
        }

        /***************************************************/

        private static double[] ToDoubleArray(this IEnumerable<Point> points, double typeTranslationFactor)
        {
            return points.SelectMany(p => p.ToDoubleArray(typeTranslationFactor)).ToArray();
        }
    }
}



