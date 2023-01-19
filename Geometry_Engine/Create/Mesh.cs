/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Mesh based on a collection of Points and Faces.")]
        [InputFromProperty("vertices")]
        [InputFromProperty("faces")]
        [Output("mesh", "The created Mesh.")]
        public static Mesh Mesh(IEnumerable<Point> vertices, IEnumerable<Face> faces)
        {
            return new Mesh
            {
                Vertices = vertices.ToList(),
                Faces = faces.ToList()
            };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random Mesh based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Input("minNbCPs", "Minimum number of vertices in the random Mesh.")]
        [Input("maxNbCPs", "Maximum number of vertices in the random Mesh.")]
        [Output("mesh", "The generated random Mesh.")]
        public static Mesh RandomMesh(int seed = -1, BoundingBox box = null, int minNbCPs = 4, int maxNbCPs = 20)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomMesh(rnd, box, minNbCPs, maxNbCPs);
        }

        /***************************************************/

        [Description("Creates a random Mesh using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Input("minNbCPs", "Minimum number of vertices in the random Mesh.")]
        [Input("maxNbCPs", "Maximum number of vertices in the random Mesh.")]
        [Output("mesh", "The generated random Mesh.")]
        public static Mesh RandomMesh(Random rnd, BoundingBox box = null, int minNbCPs = 4, int maxNbCPs = 20)
        {
            if (box == null)
                box = new BoundingBox { Min = Point(0, 0, 0), Max = Point(1, 1, 1) };

            int nb1 = rnd.Next(2, 1 + maxNbCPs / 2);
            int nb2 = rnd.Next(Math.Max(2,minNbCPs / nb1), 1 + maxNbCPs / nb1);
            double maxNoise = rnd.NextDouble() * Math.Min(box.Max.X - box.Min.X, Math.Min(box.Max.Y - box.Min.Y, box.Max.Z - box.Min.Z)) / 5;
            Ellipse ellipse = RandomEllipse(rnd, box.Inflate(-maxNoise));  // TODO: Using Ellipse doesn't guarantee the grid will be in the bounding box
            Point start = ellipse.CoordinateSystem.Origin - ellipse.Radius1 * ellipse.CoordinateSystem.X - ellipse.Radius2 * ellipse.CoordinateSystem.Y;
            Vector normal = ellipse.CoordinateSystem.X.CrossProduct(ellipse.CoordinateSystem.Y).Normalise();

            List<Point> points = new List<Point>(); ;

            double maxNormNoise = Math.Max(ellipse.CoordinateSystem.X.Length(), ellipse.CoordinateSystem.Y.Length()) / 2;

            foreach (List<Point> pts in PointGrid(start, ellipse.CoordinateSystem.X / nb1, ellipse.CoordinateSystem.Y / nb2, nb1, nb2))
            {
                points.AddRange(pts.Select(x => x + 2 * maxNormNoise * (rnd.NextDouble() - 0.5) * normal));
            } 

            List<Face> faces = new List<Face>();
            for (int i = 0; i < nb1-1; i++)
            {
                for (int j = 0; j < nb2-1; j++)
                {
                    faces.Add(new Face
                    {
                        A = i * nb2 + j,
                        B = i * nb2 + j + 1,
                        C = (i + 1) * nb2 + j + 1,
                        D = (i + 1) * nb2 + j,
                    }); 
                }
            }

            return new Mesh { Vertices = points, Faces = faces };
        }

        /***************************************************/
    }
}





