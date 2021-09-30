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
using BH.oM.Physical.Reinforcement;
using BH.oM.Quantities.Attributes;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the maximum radius based on the diameter of the reinforcement bar. The standard is determined from the namespace of the ShapeCode.")]
        [Input("reinforcement", "The reinforcement bar to determine the maximum bending radius.", typeof(Length))]
        [Output("maximumRadius", "The maximum scheduling radius based on the diameter of the reinforcement bar", typeof(Length))]
        public static double MaximumRadius(this Reinforcement reinforcement)
        {
            return reinforcement.IsNull() ? 0 : MaximumRadius(reinforcement.ShapeCode);
        }

        /***************************************************/

        [Description("Gets the maximum radius based on the diameter of the reinforcement bar. The standard is determined from the namespace of the ShapeCode.")]
        [Input("shapeCode", "The shape code to determine the maximum bending radius.", typeof(Length))]
        [Output("maximumRadius", "The maximum scheduling radius based on the diameter of the reinforcement bar", typeof(Length))]
        public static double MaximumRadius(this IShapeCode shapeCode)
        {
            if (shapeCode.IsNull())
                return 0;

            string standard = ReinforcementStandard(shapeCode);

            switch (standard)
            {
                case "BS8666":
                    return m_BS8666MaximumRadiusBendingRadius.LinearInterpolate(shapeCode.Diameter);
                default:
                    Reflection.Compute.RecordError("Standard not recognised or supported, the scheduling radius could not be calculated.");
                    return 0;
            }
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
        /**** Private Fields                            ****/
        /***************************************************/

        private static readonly Dictionary<double, double> m_BS8666MaximumRadiusBendingRadius = new Dictionary<double, double>
        {
            { 0.006,2.6 },{0.008, 2.75}, {0.01,3.5}, {0.012,4.25}, {0.016, 7.5}, {0.020, 14},{0.025, 30}, {0.032, 43}, {0.040, 58}
        };

    }
}

