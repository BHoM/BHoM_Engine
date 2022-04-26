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

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static ICurve RandomCurve(int seed = -1, BoundingBox box = null, bool closed = false)
        {
            if (seed == -1)
                seed = NextSeed();
            Random rnd = new Random(seed);
            return RandomCurve(rnd, box, closed);
        }

        /***************************************************/

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

        public static ICurve RandomCurve(Point from, int seed = -1, BoundingBox box = null, bool closed = false)
        {
            if (seed == -1)
                seed = NextSeed();
            Random rnd = new Random(seed);
            return RandomCurve(from, rnd, box, closed);
        }

        /***************************************************/

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



