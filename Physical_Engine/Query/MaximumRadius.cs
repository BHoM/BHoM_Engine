/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the maximum radius based on the diameter of the reinforcement bar using the values given in BS 8666:2020 Table 8.")]
        [Input("diameter", "The diameter of the reinforcement bar to determine the maximum bending radius.", typeof(Length))]
        [Output("radius", "The maximum scheduling radius based on the diameter of the reinforcement bar", typeof(Length))]
        public static double MaximumRadius(this double diameter)
        {
            if(diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than 0. The maximum radius cannot be calculated.");
                return 0;
            }

            List<double> diameters = new List<double>() { 6, 8, 10, 12, 16, 20, 25, 32, 40 }.Select(x => x / 1000).ToList();
            List<double> radii = new List<double>() { 2.5, 2.75, 3.5, 4.25, 7.5, 14, 30, 43, 58 };

            Dictionary<double, double> maximumBendingRadius = diameters.Zip(radii, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

            return maximumBendingRadius.LinearInterpolate(diameter);
        }

        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private static double LinearInterpolate(this Dictionary<double, double> reference, double x )
        {
            double y = 0;

            reference.TryGetValue(x, out y);

            if(x < reference.ElementAt(0).Key)
            {
                Reflection.Compute.RecordError("The value is less than the lowest reference key and therefore cannot be interpolated.");
                return 0;
            }
            if (x > reference.ElementAt(reference.Count-1).Key)
            {
                Reflection.Compute.RecordError("The value is greater than the largest reference key and therefore cannot be interpolated.");
                return 0;
            }

            if (y == 0)
            {
                for (int i = 0; i < reference.Count-1; i++)
                {
                    double key = reference.ElementAt(i + 1).Key;
                    if(key - x > 0)
                    {
                        y = (reference.ElementAt(i + 1).Value - reference.ElementAt(i).Value)/ (reference.ElementAt(i + 1).Key - reference.ElementAt(i).Key)
                            *(x - reference.ElementAt(i).Key) + reference.ElementAt(i).Value;

                        return y;
                    }
                }
            }

            return y;
        }

        /***************************************************/

    }
}

