/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
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
        [Input("box1", "Box to evaluate furthest possible square distance from.")]
        [Input("box2", "Box to evaluate furthest possible square distance from.")]
        [Output("sqDist", "The furthest possible square distance between two vectors confined in the respective DomainBoxes.")]
        public static double FurthestSquareDistance(this DomainBox box1, DomainBox box2)
        {
            if(box1 == null || box2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the furthest square distance if either domain box is null.");
                return 0;
            }

            Domain[] d1 = box1.Domains;
            Domain[] d2 = box2.Domains;
            int count = Math.Min(d1.Length, d2.Length);

            double sum = 0;
            for (int i = 0; i < count; i++)
            {
                double d = FurthestEdgeDistance(d1[i], d2[i]);
                sum += d * d;
            }
            return sum;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double FurthestEdgeDistance(Domain a, Domain b)
        {
            return Math.Max(
                b.Max - a.Min,
                a.Max - b.Min);
        }

        /***************************************************/

    }
}







