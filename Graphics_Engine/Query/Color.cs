/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace BH.Engine.Graphics
{
    public static partial class Query
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Gets a color from a gradient at the spacified position")]
        public static Color Color(this Gradient gradient, double val)
        {
            // Find adjacent markers
            KeyValuePair<decimal, Color> upper = gradient.Markers.FirstOrDefault(x => x.Key > (decimal)val);
            KeyValuePair<decimal, Color> lower = gradient.Markers.LastOrDefault(x => x.Key < (decimal)val);

            // Check adjacency on both sides
            if (lower.Equals(default(KeyValuePair<decimal, Color>)))
                return gradient.Markers.First().Value;
            if (upper.Equals(default(KeyValuePair<decimal, Color>)))
                return gradient.Markers.Last().Value;

            // Interpolate
            return InterpolateColor(lower, upper, (decimal)val);
        }

        /***************************************************/

        [Description("Gets a color from a gradient at the spacified position scaled between from and to")]
        public static Color Color(this Gradient gradient, double val, double from, double to)
        {
            return gradient.Color((val - from) / (to - from));
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/

        private static Color InterpolateColor(KeyValuePair<decimal, Color> lower, KeyValuePair<decimal, Color> upper, decimal val)
        {
            // safty check
            val = val > lower.Key ? val < upper.Key ? val : upper.Key : lower.Key;

            decimal diff = upper.Key - lower.Key;
            decimal fraction = (val - lower.Key) / diff;

            int A = (int)Math.Floor(Interpolate(lower.Value.A, upper.Value.A, fraction));
            int R = (int)Math.Floor(Interpolate(lower.Value.R, upper.Value.R, fraction));
            int G = (int)Math.Floor(Interpolate(lower.Value.G, upper.Value.G, fraction));
            int B = (int)Math.Floor(Interpolate(lower.Value.B, upper.Value.B, fraction));

            return System.Drawing.Color.FromArgb(A, R, G, B);
        }

        /***************************************************/

        private static decimal Interpolate(decimal a, decimal b, decimal val)
        {
            return (b - a) * val + a;
        }

        /***************************************************/
    }
}
