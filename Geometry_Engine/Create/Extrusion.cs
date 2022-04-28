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
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Extrusion based on its core properties.")]
        [InputFromProperty("curve")]
        [InputFromProperty("direction")]
        [InputFromProperty("capped")]
        [Output("extrusion", "The created Extrusion.")]
        public static Extrusion Extrusion(ICurve curve, Vector direction, bool capped = true)
        {
            return new Extrusion
            {
                Curve = curve,
                Direction = direction,
                Capped = capped
            };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random Extrusion based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resuling geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the goemetry.")]
        [Output("extrusion", "The generated random Extrusion.")]
        public static Extrusion RandomExtrusion(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextSeed();
            Random rnd = new Random(seed);
            return RandomExtrusion(rnd, box);
        }

        /***************************************************/

        [Description("Creates a random Extrusion using the provided Random class. If Box is provided, the resuling geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the goemetry.")]
        [Output("extrusion", "The generated random Extrusion.")]
        public static Extrusion RandomExtrusion(Random rnd, BoundingBox box = null)
        {
            if (box == null)
            {
                return new Extrusion
                {
                    Curve = RandomCurve(rnd),
                    Direction = RandomVector(rnd),
                    Capped = rnd.NextDouble() >= 0.5
                };
            }
            else
            {
                ICurve curve = RandomCurve(rnd, box);
                return new Extrusion
                {
                    Curve = curve,
                    Direction = RandomPoint(rnd, box) - curve.IBounds().Centre(),
                    Capped = rnd.NextDouble() >= 0.5
                };
            }  
        }

        /***************************************************/
    }
}



