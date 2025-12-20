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

using BH.oM.Base.Attributes;
using System;
using System.ComponentModel;
using System.Numerics;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Parses a string representation of a Complex number into a Complex object.")]
        [Input("text", "String to parse as a Complex number.")]
        [Output("complex", "Parsed Complex number, or null if parsing fails.")]
        public static Complex? ParseComplex(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                RecordError("Cannot parse Complex number from null or empty string.");
                return null;
            }

            text = text.ToLower().Trim().Replace(" ", "");
            text = NormaliseOperators(text);
            if (text == null)
            {
                RecordError("Invalid operator sequence in Complex number.");
                return null;
            }

            // Handle pure real numbers
            if (double.TryParse(text, out double realOnly))
                return new Complex(realOnly, 0);

            // Handle pure imaginary numbers (e.g., 3i, -5i, i, -i, +i, 2.5i)
            var pureImagPattern = @"^(?<imaginary>[+-]?\d*\.?\d*)i$";
            var pureImagMatch = System.Text.RegularExpressions.Regex.Match(text, pureImagPattern);
            if (pureImagMatch.Success)
            {
                var imagStr = pureImagMatch.Groups["imaginary"].Value;
                double imaginary = 0;
                if (string.IsNullOrEmpty(imagStr) || imagStr == "+")
                    imaginary = 1;
                else if (imagStr == "-")
                    imaginary = -1;
                else
                    double.TryParse(imagStr, out imaginary);
                return new Complex(0, imaginary);
            }

            // Handle a+bi or a-bi
            var realImagPattern = @"^(?<real>[+-]?\d*\.?\d+)(?<sign>[+-])(?<imaginary>\d*\.?\d*)i$";
            var realImagMatch = System.Text.RegularExpressions.Regex.Match(text, realImagPattern);
            if (realImagMatch.Success)
            {
                var realStr = realImagMatch.Groups["real"].Value;
                var sign = realImagMatch.Groups["sign"].Value;
                var imagStr = realImagMatch.Groups["imaginary"].Value;
                double real = 0;
                double imaginary = 0;
                double.TryParse(realStr, out real);
                if (string.IsNullOrEmpty(imagStr))
                    imaginary = 1;
                else
                    double.TryParse(imagStr, out imaginary);
                if (sign == "-")
                    imaginary = -imaginary;
                return new Complex(real, imaginary);
            }

            // Handle bi+a or -bi+a (imaginary first, then real)
            var imagRealPattern = @"^(?<imaginary>[+-]?\d*\.?\d*)i(?<sign>[+-])(?<real>\d*\.?\d+)$";
            var imagRealMatch = System.Text.RegularExpressions.Regex.Match(text, imagRealPattern);
            if (imagRealMatch.Success)
            {
                var imagStr = imagRealMatch.Groups["imaginary"].Value;
                var sign = imagRealMatch.Groups["sign"].Value;
                var realStr = imagRealMatch.Groups["real"].Value;
                double real = 0;
                double imaginary = 0;
                if (string.IsNullOrEmpty(imagStr) || imagStr == "+")
                    imaginary = 1;
                else if (imagStr == "-")
                    imaginary = -1;
                else
                    double.TryParse(imagStr, out imaginary);
                double.TryParse(realStr, out real);
                if (sign == "-")
                    real = -real;
                return new Complex(real, imaginary);
            }

            RecordError($"Cannot parse Complex number from '{text}'.");
            return null;
        }

        // Normalises operator sequences (e.g., 1++2i -> 1+2i, 1--2i -> 1+2i, 1+-2i -> 1-2i, etc.)
        private static string NormaliseOperators(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Replace operator sequences
            text = text.Replace("++", "+");
            text = text.Replace("+-", "-");
            text = text.Replace("-+", "-");
            text = text.Replace("--", "+");

            // Check for invalid operator sequences (more than 2 consecutive operators)
            for (int i = 0; i < text.Length - 2; i++)
            {
                if ((text[i] == '+' || text[i] == '-') &&
                    (text[i + 1] == '+' || text[i + 1] == '-') &&
                    (text[i + 2] == '+' || text[i + 2] == '-'))
                {
                    return null; // Invalid sequence
                }
            }
            return text;
        }

        /***************************************************/
    }
} 
