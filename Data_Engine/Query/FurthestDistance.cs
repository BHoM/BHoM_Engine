/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.oM.Data.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the furthest possible distance between two points confined in respective NBounds.")]
        public static double FurthestSquareDistance(this NBound box1, NBound box2)
        {
            double sqDist = 0;
            for (int i = 0; i < box1.Min.Length; i++)
            {
                sqDist += FurDist(box1.Min[i], box1.Max[i], box2.Min[i], box2.Max[i]);
            }
            return sqDist;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double FurDist(double min1, double max1, double min2, double max2)
        {
            double maxSide = min1 - max2;
            double minSide = min2 - max2;

            double max = Math.Max(maxSide, minSide);
            return max > 0 ? Math.Pow(max, 2) : 0;
        }

        /***************************************************/

    }
}

