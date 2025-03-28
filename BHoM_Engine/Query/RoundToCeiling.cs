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

using BH.oM.Base.Attributes;
using System;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Rounds a number using the given tolerance, rounding to ceiling to the nearest tolerance multiplier." +
            "Supports any fractional, integer, positive or negative numbers." +
            "\nSome examples:" +
            "\n\t RoundToCeiling(12, 20) ==> 20" +
            "\n\t RoundToCeiling(121, 2) ==> 122" +
            "\n\t RoundToCeiling(1.2345, 1.1) ==> 2.2" +
            "\n\t RoundToCeiling(0.014, 0.01) ==> 0.02" +
            "\n\t RoundToCeiling(-0.014, 0.01) ==> -0.01" +
            "\n\t RoundToCeiling(0.015, 0.01) ==> 0.02" +
            "\n\t RoundToCeiling(0.014, 0.02) ==> 0.02" +
            "\nand so on.")]
        [Input("number", "Number to be rounded.")]
        [Input("tolerance", "Tolerance to use for rounding.")]
        public static double RoundToCeiling(this double number, double tolerance)
        {
            if (tolerance < 0)
            {
                BH.Engine.Base.Compute.RecordError("Tolerance cannot be less than 0.");
                return default(double);
            }

            // If the tolerance is the smallest possible double, or if the inputs are invalid, just return.
            if (number == 0 || tolerance == 0 || Double.IsNaN(tolerance) || Double.IsNaN(number) || Double.IsInfinity(number) || Double.IsInfinity(tolerance))
                return number;

            // Otherwise, perform the approximation with the given tolerance.
            return tolerance * Math.Ceiling(number / tolerance);
        }

        /***************************************************/

        // NOTE: Although we could be satisfied with just one method "RoundToCeiling" from the UI perspective,
        // we also need a dedicated method for Integers, to avoid a performance hit when using this method from other parts of the code, e.g. in Hash().

        [Description("Rounds an integer number using the given tolerance, rounding to ceiling to the nearest tolerance multiplier." +
            "\nSome examples:" +
            "\n\t RoundToCeiling(12, 20) ==> 20" +
            "\n\t RoundToCeiling(121, 2) ==> 122" +
            "\n\t RoundToCeiling(-35, 20) ==> -20" +
            "\nand so on.")]
        [Input("number", "Number to be rounded.")]
        [Input("tolerance", "Tolerance to use for rounding.")]
        public static int RoundToCeiling(this int number, double tolerance)
        {
            if (tolerance < 0)
            {
                BH.Engine.Base.Compute.RecordError("Tolerance cannot be less than 0.");
                return 0;
            }

            // If the tolerance is the smallest possible double, or if the inputs are invalid, just return.
            if (number == 0 || tolerance == double.MinValue || tolerance == 0 || Double.IsNaN(tolerance) || number == int.MinValue || number == int.MaxValue || Double.IsInfinity(tolerance))
                return number;

            if ((int)tolerance != tolerance)
            {
                BH.Engine.Base.Compute.RecordError("Tolerance needs to be an integer value.");
                return 0;
            }

            return (int)(Math.Ceiling(number / tolerance) * tolerance);
        }

        /***************************************************/
    }
}

