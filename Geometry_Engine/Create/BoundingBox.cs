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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an BoundingBox based on its core properties.")]
        [InputFromProperty("min")]
        [InputFromProperty("max")]
        [Output("box", "The created BoundingBox.")]
        public static BoundingBox BoundingBox(Point min, Point max)
        {
            return new BoundingBox
            {
                Min = min,
                Max = max
            };
        }

        /***************************************************/

        [Description("Creates a bounding box based on a centre point and vector controling the extents of the bounding box.")]
        public static BoundingBox BoundingBox(Point centre, Vector extent)
        {
            //TODO: SHould this be changed to
            //1. Use absolute values
            //2. Use half vector length instead of full.?
            return new BoundingBox
            {
                Min = new Point { X = centre.X - extent.X, Y = centre.Y - extent.Y, Z = centre.Z - extent.Z },
                Max = new Point { X = centre.X + extent.X, Y = centre.Y + extent.Y, Z = centre.Z + extent.Z }
            };
        }

        /***************************************************/

        [Description("Create a BoundingBox using the properties of a cuboid. The resulting BoundingBox will be centred on the global origin, and have the same length, depth and height as the cuboid.")]
        [Input("globallyAlignedCuboid", "A Cuboid that will be treated as if it is globally aligned, local Cuboid orientation will be ignored." +
            " The cuboid's Length, Depth and Height parameters are used to define the maximum and minimum points of the resulting BoundingBox.")]
        [Output("boundingBox", "BoundingBox based on the properties of the cuboid.")]
        public static BoundingBox BoundingBox(this Cuboid globallyAlignedCuboid)
        {
            return new BoundingBox
            {
                Min = new Point { X = -globallyAlignedCuboid.Length / 2, Y = -globallyAlignedCuboid.Depth / 2, Z = -globallyAlignedCuboid.Height / 2 },
                Max = new Point { X =  globallyAlignedCuboid.Length / 2, Y =  globallyAlignedCuboid.Depth / 2, Z =  globallyAlignedCuboid.Height / 2 }
            };
        }

        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random BoundingBox based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("box", "The generated random BoundingBox.")]
        public static BoundingBox RandomBoundingBox(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomBoundingBox(rnd, box);
        }

        /***************************************************/

        [Description("Creates a random BoundingBox using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("box", "The generated random BoundingBox.")]
        public static BoundingBox RandomBoundingBox(Random rnd, BoundingBox box = null)
        {
            Point p1 = RandomPoint(rnd, box);
            Point p2 = RandomPoint(rnd, box);
            return new BoundingBox
            {
                Min = new Point()
                {
                    X = Math.Min(p1.X, p2.X),
                    Y = Math.Min(p1.Y, p2.Y),
                    Z = Math.Min(p1.Z, p2.Z),
                },
                Max = new Point()
                {
                    X = Math.Max(p1.X, p2.X),
                    Y = Math.Max(p1.Y, p2.Y),
                    Z = Math.Max(p1.Z, p2.Z),
                },
            };
        }

        /***************************************************/
    }
}




