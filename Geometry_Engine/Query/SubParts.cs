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

using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static List<Line> SubParts(this Polyline curve)
        {
            List<Line> result = new List<Line>();
            List<Point> pts = curve.ControlPoints;

            for (int i = 1; i < pts.Count; i++)
                result.Add(new Line { Start = pts[i - 1], End = pts[i] });

            return result;
        }

        /***************************************************/

        public static List<ICurve> SubParts(this PolyCurve curve)
        {
            List<ICurve> exploded = new List<ICurve>();
            List<ICurve> curves = curve.Curves;

            for (int i = 0; i < curves.Count; i++)
                exploded.AddRange(curves[i].ISubParts());
            
            return exploded;
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static List<ISurface> SubParts(this PolySurface surface)
        {
            List<ISurface> exploded = new List<ISurface>();
            List<ISurface> surfaces = surface.Surfaces;

            for (int i = 0; i < surfaces.Count; i++)
                exploded.AddRange(surfaces[i].ISubParts());

            return exploded;
        }

        /***************************************************/
        /**** Public Methods - Meshes                   ****/
        /***************************************************/

        public static List<Mesh> SubParts(this Mesh mesh)
        {
            List<Mesh> explodedMeshes = new List<Mesh>();
            List<Face> faces = mesh.Faces;
            List<Point> vertices = mesh.Vertices;

            for (int i = 0; i < faces.Count; i++)
            {
                Face localFace = new Face { A = 0, B = 1, C = 2 };
                List<Point> localVertices = new List<Point>();
                localVertices.Add(vertices[faces[i].A]);
                localVertices.Add(vertices[faces[i].B]);
                localVertices.Add(vertices[faces[i].C]);

                if (faces[i].IsQuad())
                {
                    localVertices.Add(vertices[faces[i].D]);
                    localFace.D = 3;
                }
                explodedMeshes.Add(new Mesh { Vertices = localVertices.ToList(), Faces = new List<Face>() { localFace } });
            }

            return explodedMeshes;
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static List<IGeometry> SubParts(this CompositeGeometry group)
        {
            List<IGeometry> exploded = new List<IGeometry>();
            List<IGeometry> elements = group.Elements;

            for (int i = 0; i < elements.Count; i++)
                exploded.AddRange(elements[i].ISubParts());

            return exploded;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IEnumerable<IGeometry> ISubParts(this IGeometry geometry)
        {
            return SubParts(geometry as dynamic);
        }

        /***************************************************/

        public static IEnumerable<ICurve> ISubParts(this ICurve geometry)
        {
            return SubParts(geometry as dynamic);
        }

        /***************************************************/

        public static IEnumerable<ISurface> ISubParts(this ISurface geometry)
        {
            return SubParts(geometry as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IEnumerable<IGeometry> SubParts(this IGeometry geometry)
        {
            return new List<IGeometry> { geometry };
        }

        /***************************************************/

        private static IEnumerable<ICurve> SubParts(this ICurve geometry)
        {
            return new List<ICurve> { geometry };
        }

        /***************************************************/

        private static IEnumerable<ISurface> SubParts(this ISurface geometry)
        {
            return new List<ISurface> { geometry };
        }

        /***************************************************/
    }
}






