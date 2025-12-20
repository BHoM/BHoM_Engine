/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using System;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Solids                   ****/
        /***************************************************/

        [Description("Gets the enclosed volume created by the BoundaryRepresentation Surfaces. This value is retrieved from the immutable property stored on the object itself.")]
        [Input("solid","The solid BoundaryRepresentaion to query the volume from.")]
        [Output("volume", "The volume of the solid.", typeof(Volume))]
        public static double Volume(this BoundaryRepresentation solid)
        {
            return solid.Volume;
        }

        /***************************************************/

        [Description("Calculates the analytical solid volume.")]
        [Input("cone", "The solid cone to query the volume from.")]
        [Output("volume", "The volume of the solid.", typeof(Volume))]
        public static double Volume(this Cone cone)
        {
            return (1.0 / 3.0) * Math.PI * Math.Pow(cone.Radius, 2) * cone.Height;
        }

        /***************************************************/

        [Description("Calculates the analytical solid volume.")]
        [Input("cuboid", "The cuboid to query the volume from.")]
        [Output("volume", "The volume of the solid.", typeof(Volume))]
        public static double Volume(this Cuboid cuboid)
        {
            return cuboid.Length * cuboid.Depth * cuboid.Height;
        }

        /***************************************************/

        [Description("Calculates the analytical solid volume.")]
        [Input("cylinder", "The cylinder to query the volume from.")]
        [Output("volume", "The volume of the solid.", typeof(Volume))]
        public static double Volume(this Cylinder cylinder)
        {
            return Math.PI * Math.Pow(cylinder.Radius, 2) * cylinder.Height;
        }

        /***************************************************/

        [Description("Calculates the analytical solid volume.")]
        [Input("sphere", "The sphere to query the volume from.")]
        [Output("volume", "The volume of the solid.", typeof(Volume))]
        public static double Volume(this Sphere sphere)
        {
            return (4.0 / 3.0) * Math.PI * Math.Pow(sphere.Radius, 3);
        }


        /***************************************************/

        [Description("Calculates the analytical solid volume.")]
        [Input("torus", "The torus to query the volume from.")]
        [Output("volume", "The volume of the solid.", typeof(Volume))]
        public static double Volume(this Torus torus)
        {
            return 2.0 * Math.Pow(Math.PI, 2) * Math.Pow(torus.RadiusMinor, 2) * torus.RadiusMajor;
        }

        /***************************************************/

        [Description("Calculates the enclosed volume of a mesh using the divergence theorem. The mesh must be closed and manifold for the result to be valid.")]
        [Input("mesh", "The mesh to query the volume from. Must be closed and manifold.")]
        [Output("volume", "", typeof(Volume))]
        public static double Volume(this Mesh mesh)
        {
            if (!mesh.IsManifold())
                Base.Compute.RecordWarning("The input mesh is nonmanifold, therefore volume computation may yield incorrect results.");

            double volume = 0.0;
            foreach (Face face in mesh.Faces)
            {
                if (face.D == -1) // Triangular face
                {
                    Point p0 = mesh.Vertices[face.A];
                    Point p1 = mesh.Vertices[face.B];
                    Point p2 = mesh.Vertices[face.C];

                    // Compute the signed volume of the tetrahedron formed by the triangle and the origin
                    double v = (p0.X * (p1.Y * p2.Z - p2.Y * p1.Z) -
                                p0.Y * (p1.X * p2.Z - p2.X * p1.Z) +
                                p0.Z * (p1.X * p2.Y - p2.X * p1.Y)) / 6.0;

                    volume += v;
                }
                else // Quadrilateral face - split into two triangles
                {
                    Point p0 = mesh.Vertices[face.A];
                    Point p1 = mesh.Vertices[face.B];
                    Point p2 = mesh.Vertices[face.C];
                    Point p3 = mesh.Vertices[face.D];

                    // First triangle: A, B, C
                    double v1 = (p0.X * (p1.Y * p2.Z - p2.Y * p1.Z) -
                                 p0.Y * (p1.X * p2.Z - p2.X * p1.Z) +
                                 p0.Z * (p1.X * p2.Y - p2.X * p1.Y)) / 6.0;

                    // Second triangle: A, C, D
                    double v2 = (p0.X * (p2.Y * p3.Z - p3.Y * p2.Z) -
                                 p0.Y * (p2.X * p3.Z - p3.X * p2.Z) +
                                 p0.Z * (p2.X * p3.Y - p3.X * p2.Y)) / 6.0;

                    volume += v1 + v2;
                }
            }

            return Math.Abs(volume); // Return absolute value to ensure positive volume
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the enclosed volume of a solid.")]
        [Input("solid", "The solid to query the volume from.")]
        [Output("volume", "The volume of the solid.", typeof(Volume))]
        public static double IVolume(this ISolid solid)
        {
            return Volume(solid as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static double Volume(this ISolid solid)
        {
            Base.Compute.RecordError($"Volume is not implemented for ISolids of type: {solid.GetType().Name}.");
            return double.NaN;
        }

        /***************************************************/
    }
}
