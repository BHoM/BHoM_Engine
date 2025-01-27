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

        [Description("Rounds a number using the given tolerance, rounding to the nearest tolerance multiplier." +
            "Supports any fractional, integer, positive or negative numbers." +
            "\nSome examples:" +
            "\n\t Round(12, 20) ==> 20" +
            "\n\t Round(121, 2) ==> 122" +
            "\n\t Round(1.2345, 1.1) ==> 1.1" +
            "\n\t Round(0.014, 0.01) ==> 0.01" +
            "\n\t Round(-0.014, 0.01) ==> -0.01" +
            "\n\t Round(0.015, 0.01) ==> 0.02" +
            "\n\t Round(0.014, 0.02) ==> 0.02" +
            "\nand so on.")]
        [Input("number", "Number to be rounded.")]
        [Input("tolerance", "Tolerance to use for rounding.")]
        public static double Round(this double number, double tolerance)
        {
            if (tolerance < 0)
            {
                BH.Engine.Base.Compute.RecordError("Tolerance cannot be less than 0.");
                return default(double);
            }

            // If the tolerance is the smallest possible double, or if the inputs are invalid, just return.
            if (number == 0 || tolerance == 0 || Double.IsNaN(tolerance) || Double.IsNaN(number) || Double.IsInfinity(number) || Double.IsInfinity(tolerance))
                return number;

            // First check if the tolerance can be converted into fractional digits, i.e. is a number in the form of 10^someExp.
            // This avoids imprecisions with the approximation formula below. If so, just return Math.Round().
            int fractionalDigits = Math.Abs(System.Convert.ToInt32(Math.Log10(tolerance)));
            if (tolerance < 1 && Math.Pow(10, -fractionalDigits) == tolerance)
                return Math.Round(number, fractionalDigits);

            // Otherwise, perform the approximation with the given tolerance.
            //int unitStep = number > 0 ? 1 : -1; // Useful to deal with negative numbers.
            return RoundNumericValue(number, tolerance);
        }

        /***************************************************/

        // NOTE: Although we could be satisfied with just one method "Round" from the UI perspective,
        // we also need a dedicated method for Integers, to avoid a performance hit when using this method from other parts of the code, e.g. in Hash().

        [Description("Rounds an integer number using the given tolerance, rounding to the nearest tolerance multiplier." +
            "\nSome examples:" +
            "\n\t Round(12, 20) ==> 20" +
            "\n\t Round(121, 2) ==> 122" +
            "\n\t Round(-35, 20) ==> -40" +
            "\nand so on.")]
        [Input("number", "Number to be rounded.")]
        [Input("tolerance", "Tolerance to use for rounding.")]
        public static int Round(this int number, double tolerance)
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

            return (int)(RoundNumericValue(number, tolerance));
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double RoundNumericValue(double value, double accuracy)
        {
            bool neg = value < 0;
            if (neg)
                value = -value;

            decimal decV = (decimal)value;
            decimal decA = (decimal)accuracy;
            decimal diff = decV % decA;
            if (diff >= decA / 2m)
                diff = -(decA - diff);

            double res = (double)(decV - diff);
            if (neg)
                res = -res;

            return res;
        }

        /***************************************************/
    }
}

