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

        [Description("Gets the furthest possible distance between two vectors confined in respective DomainBoxes.")]
        public static double FurthestSquareDistance(this DomainBox box1, DomainBox box2)
        {
            return box1.Domains.Zip(box2.Domains, (a, b) => Math.Pow(FurdestEdgeDistance(a, b), 2)).Sum();
        }

        [Description("Gets the furthest possible distance between two vectors confined in respective tight DomainBoxes." + 
                     "i.e. assume that the data in the DomainBoxes coincide with every side of the box and get a worst case distance between their data.")]
        public static double FurthestThightSquareDistance(this DomainBox box1, DomainBox box2)
        {
            // Find in which axis the distance between the outer bounderies of the boxes is the least
            int index = 0;
            double min = double.MaxValue;
            for (int i = 0; i < box1.Domains.Length; i++)
            {
                double temp = ShortestEdgeDistance(box1.Domains[i], box2.Domains[i]);
                if (temp < min)
                {
                    min = temp;
                    index = i;
                }
            }

            double sq = min * min;

            for (int i = 0; i < box1.Domains.Length; i++)
            {
                if (i != index)
                    sq += Math.Pow(FurdestEdgeDistance(box1.Domains[i], box2.Domains[i]), 2);
            }

            return sq;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double FurdestEdgeDistance(Domain a, Domain b)
        {
            return Math.Max(
                b.Max - a.Min, 
                a.Max - b.Min);
        }

        /***************************************************/

        private static double ShortestEdgeDistance(Domain a, Domain b)
        {
            double[] values = new double[] 
            {
                a.Min - b.Min,
                a.Min - b.Max,
                a.Max - b.Min,
                a.Max - b.Max,

            };

            return values.Min(x => Math.Abs(x));
        }

        /***************************************************/

    }
}

