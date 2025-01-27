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

        [Description("Creates a PolyCurve based on a collection of curves. Note that there is no requirement of a BHoM polycurve to be contructed of joined segments.")]
        [InputFromProperty("curves")]
        [Output("pCurve", "The created PolyCurve.")]
        public static PolyCurve PolyCurve(IEnumerable<ICurve> curves)
        {
            return new PolyCurve { Curves = curves.ToList() };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random PolyCurve based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Input("minNbCurves", "Minimun number of curves in the random PolyCurve.")]
        [Input("maxNbCurves", "Maximum number of curves in the random PolyCurve.")]
        [Output("pCurve", "The generated random PolyCurve.")]
        public static PolyCurve RandomPolyCurve(int seed = -1, BoundingBox box = null, int minNbCurves = 2, int maxNbCurves = 10)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomPolyCurve(rnd, box, minNbCurves, maxNbCurves);
        }

        /***************************************************/

        [Description("Creates a random PolyCurve using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Input("minNbCurves", "Minimun number of curves in the random PolyCurve.")]
        [Input("maxNbCurves", "Maximum number of curves in the random PolyCurve.")]
        [Output("pCurve", "The generated random PolyCurve.")]
        public static PolyCurve RandomPolyCurve(Random rnd, BoundingBox box = null, int minNbCurves = 2, int maxNbCurves = 10)
        {
            List<ICurve> curves = new List<ICurve>();
            Point start = RandomPoint(rnd, box);
            for (int i = 0; i < rnd.Next(minNbCurves, maxNbCurves + 1); i++)
            {
                ICurve crv = RandomCurve(start, rnd, box);
                curves.Add(crv);
                start = crv.IEndPoint();
            }
            return new PolyCurve { Curves = curves };
        }

        /***************************************************/
    }
}






