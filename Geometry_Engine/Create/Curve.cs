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
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random ICurve based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("curve", "The generated random ICurve.")]
        public static ICurve RandomCurve(int seed = -1, BoundingBox box = null, bool closed = false)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomCurve(rnd, box, closed);
        }

        /***************************************************/

        [Description("Creates a random ICurve using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("curve", "The generated random ICurve.")]
        public static ICurve RandomCurve(Random rnd, BoundingBox box = null, bool closed = false)
        {
            int nb = rnd.Next(5);
            switch (nb)
            {
                case 0:
                    return RandomArc(rnd, box);
                case 1:
                    return RandomCircle(rnd, box);
                case 2:
                    return RandomEllipse(rnd, box);
                case 3:
                    return RandomLine(rnd, box);
                default:
                    return RandomPolyline(rnd, box);
            }
        }

        /***************************************************/

        [Description("Creates a random ICurve with a set start point based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("from", "The start point of the ICurve.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("curve", "The generated random ICurve.")]
        public static ICurve RandomCurve(Point from, int seed = -1, BoundingBox box = null, bool closed = false)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomCurve(from, rnd, box, closed);
        }

        /***************************************************/

        [Description("Creates a random Arc with a set start point using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("from", "The start point of the ICurve.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("curve", "The generated random ICurve.")]
        public static ICurve RandomCurve(Point from, Random rnd, BoundingBox box = null, bool closed = false)
        {
            int nb = rnd.Next(3);
            switch (nb)
            {
                case 0:
                    return RandomArc(from, rnd, box);
                case 1:
                    return RandomLine(from, rnd, box);
                default:
                    return RandomPolyline(from, rnd, box);
            }
        }

        /***************************************************/
    }
}






