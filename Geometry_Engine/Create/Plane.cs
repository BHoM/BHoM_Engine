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

using BH.Engine.Base;
using BH.oM.Geometry;
using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/
        [Description("Creates a Plane based on its origin and normal vector.")]
        [InputFromProperty("origin")]
        [InputFromProperty("normal")]
        [Output("plane", "The created Plane.")]
        public static Plane Plane(Point origin, Vector normal)
        {
            return new Plane{Origin = origin, Normal = normal};
        }

        /***************************************************/
        [Description("Creates a Plane passing through the three points. Origin of the plane will be set to the first point.")]
        [Input("p1", "First point that the plane passes through. Will be used as the origin of the created plane.")]
        [Input("p2", "Second point that the plane passes through.")]
        [Input("p3", "Third point that the plane passes through.")]
        [Output("plane", "The created Plane.")]
        public static Plane Plane(Point p1, Point p2, Point p3)
        {
            Vector normal = Query.CrossProduct(p2 - p1, p3 - p1).Normalise();
            return new Plane{Origin = p1, Normal = normal};
        }

        /***************************************************/
        /****             Random Geometry               ****/
        /***************************************************/
        [Description("Creates a random Plane based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The origin created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("plane", "The generated random Plane.")]
        public static Plane RandomPlane(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomPlane(rnd, box);
        }

        /***************************************************/
        [Description("Creates a random Plane using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The origin created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("plane", "The generated random Plane.")]
        public static Plane RandomPlane(Random rnd, BoundingBox box = null)
        {
            return new Plane{Origin = RandomPoint(rnd, box), Normal = RandomVector(rnd).Normalise()};
        }
    /***************************************************/
    }
}