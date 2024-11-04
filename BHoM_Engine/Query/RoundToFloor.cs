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

        [Description("Rounds a number using the given tolerance, rounding to floor to the nearest tolerance multiplier." +
            "Supports any fractional, integer, positive or negative numbers." +
            "\nSome examples:" +
            "\n\t RoundWithTolerance(12, 20) ==> 0" +
            "\n\t RoundWithTolerance(121, 2) ==> 120" +
            "\n\t RoundWithTolerance(1.2345, 1.1) ==> 1.1" +
            "\n\t RoundWithTolerance(0.014, 0.01) ==> 0.01" +
            "\n\t RoundWithTolerance(0.014, 0.02) ==> 0" +
            "\nand so on.")]
        [Input("number", "Number to be rounded.")]
        [Input("tolerance", "Tolerance to use for rounding.")]
        public static double RoundToFloor(this double number, double tolerance)
        {
            if (tolerance < 0)
            {
                BH.Engine.Base.Compute.RecordError("Tolerance cannot be less than 0.");
                return default(double);
            }

            // If the tolerance is the smallest possible double, or if the inputs are invalid, just return.
            if (tolerance == double.MinValue || tolerance == 0 || Double.IsNaN(tolerance) || Double.IsNaN(number) || Double.IsInfinity(number) || Double.IsInfinity(tolerance))
                return number;

            // Otherwise, perform the approximation with the given tolerance.
            int unitStep = number > 0 ? 1 : -1; // Useful to deal with negative numbers.
            return tolerance * Math.Floor(number * unitStep / tolerance) * unitStep;
        }

        /***************************************************/

        // NOTE: Although we could be satisfied with just one method "RoundToFloor" from the UI perspective,
        // we also need a dedicated method for Integers, to avoid a performance hit when using this method from other parts of the code, e.g. in Hash().

        [Description("Rounds an integer number using the given tolerance, rounding to floor to the nearest tolerance multiplier." +
            "\nSome examples:" +
            "\n\t RoundWithTolerance(12, 20) ==> 0" +
            "\n\t RoundWithTolerance(121, 2) ==> 120" +
            "\n\t RoundWithTolerance(-40, 20) ==> -40" +
            "\nand so on.")]
        [Input("number", "Number to be rounded.")]
        [Input("tolerance", "Tolerance to use for rounding.")]
        public static int RoundToFloor(this int number, double tolerance)
        {
            if (tolerance < 1)
                return number; 

            if (number > 0 && tolerance > number)
                return 0;

            if (tolerance < 0)
            {
                BH.Engine.Base.Compute.RecordError("Tolerance cannot be less than 0.");
                return 0;
            }

            int unitStep = number > 0 ? 1 : -1;
            int flooredIntegerTolerance = (int)tolerance;
            return FlooredIntegerDivision(number * unitStep, flooredIntegerTolerance) * flooredIntegerTolerance * unitStep;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static int FlooredIntegerDivision(int a, int b)
        {
            if (a < 0)
            {
                if (b > 0)
                    return (a - b + 1) / b;
            }
            else if (a > 0)
            {
                if (b < 0)
                    return (a - b - 1) / b;
            }
            return a / b;
        }

        /***************************************************/
    }
}
