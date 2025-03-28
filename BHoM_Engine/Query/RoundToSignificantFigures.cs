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

using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Base;
using System.Collections;
using System.Data;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Performs the rounding to significant figures.")]
        [Input("number", "Number to be rounded.")]
        [Input("figures", "Significant figures to be considered.")]
        public static double RoundToSignificantFigures(this double number, int figures)
        {
            if (number == 0.0 || Double.IsNaN(number) || Double.IsInfinity(number) || figures == int.MaxValue)
                return number;

            // Compute shift of the decimal point.
            int shift = figures - 1 - (int)Math.Floor(Math.Log10(Math.Abs(number)));

            // Return if rounding to the same or higher precision.
            int decimalPlaces = 0;
            for (long pow = 1; Math.Floor(number * pow) != (number * pow); pow *= 10) decimalPlaces++;
            if (shift >= decimalPlaces)
                return number;

            // Round to sf-1 fractional digits of normalized mantissa x.dddd
            double scale = Math.Pow(10, Math.Abs(shift));
            return shift > 0 ?
                   Math.Round(number * scale, MidpointRounding.AwayFromZero) / scale :
                   Math.Round(number / scale, MidpointRounding.AwayFromZero) * scale;
        }

        /***************************************************/

        [Description("Performs the rounding to significant figures.")]
        [Input("number", "Number to be rounded.")]
        [Input("figures", "Significant figures to be considered.")]
        public static int RoundToSignificantFigures(this int number, int figures)
        {
            if (number == 0 || figures == int.MaxValue)
                return number;

            // If a faster implementation for integer can be done, enter it here.
            // For now, casting to double and back to int.
            return (int)RoundToSignificantFigures((double)number, figures);
        }
    }
}




