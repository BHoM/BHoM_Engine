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
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PolyCurve PolyCurve(IEnumerable<ICurve> curves)
        {
            return new PolyCurve { Curves = curves.ToList() };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static PolyCurve RandomPolyCurve(int seed = -1, BoundingBox box = null, int minNbCurves = 2, int maxNbCurves = 10)
        {
            if (seed == -1)
                seed = NextSeed();
            Random rnd = new Random(seed);
            return RandomPolyCurve(rnd, box, minNbCurves, maxNbCurves);
        }

        /***************************************************/

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



