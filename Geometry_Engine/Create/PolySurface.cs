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

        [Description("Creates a PolyCurve based on a collection of surfaces. Note that there is no requirement of a BHoM PolySurface to be contructed of joined surfaces.")]
        [InputFromProperty("surfaces")]
        [Output("pSurface", "The created PolySurface.")]
        public static PolySurface PolySurface(IEnumerable<ISurface> surfaces)
        {
            return new PolySurface { Surfaces = surfaces.ToList() };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random PolySurface based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box. The resulting PolySurface can be disjointed, e.g. it can be made of disconnected surfaces.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Input("minNbSurfaces", "Minimun number of surfaces in the random PolySurface.")]
        [Input("maxNbSurfaces", "Maximum number of surfaces in the random PolySurface.")]
        [Output("pSurface", "The generated random PolySurface.")]
        public static PolySurface RandomPolySurface(int seed = -1, BoundingBox box = null, int minNbSurfaces = 2, int maxNbSurfaces = 10)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomPolySurface(rnd, box, minNbSurfaces, maxNbSurfaces);
        }

        /***************************************************/

        [Description("Creates a random PolySurface using the provided Random class. If Box is provided, the resulting geometry will be contained within the box. The resulting PolySurface can be disjointed, e.g. it can be made of disconnected surfaces.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Input("minNbSurfaces", "Minimun number of surfaces in the random PolySurface.")]
        [Input("maxNbSurfaces", "Maximum number of surfaces in the random PolySurface.")]
        [Output("pSurface", "The generated random PolySurface.")]
        public static PolySurface RandomPolySurface(Random rnd, BoundingBox box = null, int minNbSurfaces = 2, int maxNbSurfaces = 10)
        {
            List<ISurface> surfaces = new List<ISurface>();
            for (int i = 0; i < rnd.Next(minNbSurfaces, maxNbSurfaces + 1); i++)
            {
                surfaces.Add(RandomSurface(rnd, box));
            }

            return new PolySurface { Surfaces = surfaces };
        }

        /***************************************************/
    }
}





