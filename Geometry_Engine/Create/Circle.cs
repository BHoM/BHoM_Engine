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
using System;
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

        [Description("Creates a Circle aligned with the XY plane based on its core properties.")]
        [InputFromProperty("centre")]
        [InputFromProperty("radius")]
        [Output("circle", "The created Circle.")]
        public static Circle Circle(Point centre, double radius = 0)
        {
            return new Circle
            {
                Centre = centre,
                Radius = radius
            }; 
        }

        /***************************************************/

        [Description("Create a Circle based on its core properties.")]
        [InputFromProperty("centre")]
        [InputFromProperty("normal")]
        [InputFromProperty("radius")]
        [Output("circle", "The created Circle.")]
        public static Circle Circle(Point centre, Vector normal, double radius = 0)
        {
            return new Circle
            {
                Centre = centre,
                Normal = normal,
                Radius = radius
            };
        }

        /***************************************************/

        [Description("Create a Circle that is passing through the three provided points.")]
        [Input("pt1", "First point on the circle.")]
        [Input("pt2", "Second point on the circle.")]
        [Input("pt3", "Third point on the circle.")]
        [Input("tolerance", "Tolerance to be used in the method.", typeof(Length))]
        [Output("circle", "The created Circle.")]
        public static Circle Circle(Point pt1, Point pt2, Point pt3, double tolerance = Tolerance.Distance)
        {
            Vector v1 = pt1 - pt3;
            Vector v2 = pt2 - pt3;
            Vector normal = v1.CrossProduct(v2).Normalise();

            Point centre = Query.LineIntersection(
                Create.Line(pt3 + v1 / 2, v1.CrossProduct(normal)),
                Create.Line(pt3 + v2 / 2, v2.CrossProduct(normal)),
                true,
                tolerance
            );

            return new Circle { Centre = centre, Normal = normal, Radius = pt1.Distance(centre) };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random Circle based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("circle", "The generated random Circle.")]
        public static Circle RandomCircle(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomCircle(rnd, box);
        }

        /***************************************************/

        [Description("Creates a random Circle using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("circle", "The generated random Circle.")]
        public static Circle RandomCircle(Random rnd, BoundingBox box = null)
        {
            if (box == null)
            {
                return new Circle
                {
                    Centre = RandomPoint(rnd),
                    Normal = RandomVector(rnd).Normalise(),
                    Radius = rnd.NextDouble()
                };
            }
            else
            {
                Point centre = RandomPoint(rnd, box);
                double maxRadius = new double[]
                {
                    box.Max.X - centre.X,
                    box.Max.Y - centre.Y,
                    box.Max.Z - centre.Z,
                    centre.X - box.Min.X,
                    centre.Y - box.Min.Y,
                    centre.Z - box.Min.Z
                }.Min();

                return new Circle
                {
                    Centre = centre,
                    Normal = RandomVector(rnd).Normalise(),
                    Radius = maxRadius * rnd.NextDouble()
                };
            }
        }

        /***************************************************/
    }
}






