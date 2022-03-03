/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
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

        [Description("Gets a color from a gradient at the spacified position between 0 and 1.")]
        [Input("gradient", "The gradient to Query a Color from.")]
        [Input("val", "The number between 0 and 1 to use for interpolation of the markers colors.")]
        [Output("color", "The Color at the specified position.")]
        public static Color Color(this Gradient gradient, double val)
        {
            if(gradient == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the colour of a null gradient.");
                return System.Drawing.Color.Transparent;
            }

            // keep val in the domain
            val = val > 1 ? 1 : val < 0 ? 0 : val;

            // Find adjacent markers
            KeyValuePair<decimal, Color> upper = gradient.Markers.FirstOrDefault(x => x.Key >= (decimal)val);
            KeyValuePair<decimal, Color> lower = gradient.Markers.LastOrDefault(x => x.Key <= (decimal)val);

            // Check adjacency on both sides
            if (lower.Equals(default(KeyValuePair<decimal, Color>)))
                return gradient.Markers.First().Value;
            if (upper.Equals(default(KeyValuePair<decimal, Color>)))
                return gradient.Markers.Last().Value;

            // Interpolate
            return InterpolateColor(lower, upper, (decimal)val);
        }

        /***************************************************/

        [Description("Gets a color from a gradient at the spacified position between 0 and 1.")]
        [Input("gradient", "The gradient to Query a Color from.")]
        [Input("val", "The number between 0 and 1 to use for extracting the colour from the gradient.")]
        [Output("color", "The Color at the specified position.")]
        public static Color Color(this SteppedGradient gradient, double val)
        {
            if (gradient == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the colour of a null gradient.");
                return System.Drawing.Color.Transparent;
            }

            // keep val in the domain
            val = val > 1 ? 1 : val < 0 ? 0 : val;

            // Find first marker with value above the value
            return gradient.Markers.LastOrDefault(x => x.Key <= (decimal)val).Value;
        }

        /***************************************************/

        [Description("Gets a color from a gradient at the spacified position scaled between from and to.")]
        [Input("gradient", "The gradient to Query a Color from.")]
        [Input("val", "The number between 'from' and 'to' to use for extraction of colour from the gradient.")]
        [Input("from", "The lower bound of 'val's rescaling to 0 to 1.")]
        [Input("to", "The upper bound of 'val's rescaling to 0 to 1.")]
        [Output("color", "The Color at the specified position.")]
        public static Color Color(this IGradient gradient, double val, double from, double to)
        {
            if (gradient == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the colour of a null gradient.");
                return System.Drawing.Color.Transparent;
            }

            return gradient.IColor((val - from) / (to - from));
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Gets a color from a gradient at the spacified position between 0 and 1.")]
        [Input("gradient", "The gradient to Query a Color from.")]
        [Input("val", "The number between 0 and 1 to use for extracting the colour from the gradient.")]
        [Output("color", "The Color at the specified position.")]
        public static Color IColor(this IGradient gradient, double val)
        {
            if (gradient == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the colour of a null gradient.");
                return System.Drawing.Color.Transparent;
            }

            return Color(gradient as dynamic, val);
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/

        private static Color InterpolateColor(KeyValuePair<decimal, Color> lower, KeyValuePair<decimal, Color> upper, decimal val)
        {
            // Safety check
            val = val > lower.Key ? val < upper.Key ? val : upper.Key : lower.Key;

            decimal diff = upper.Key - lower.Key;
            if (diff == 0)
                return lower.Value;
            decimal fraction = (val - lower.Key) / diff;

            int alpha = (int)Math.Round(Interpolate(lower.Value.A, upper.Value.A, fraction));
            int red = (int)Math.Round(Interpolate(lower.Value.R, upper.Value.R, fraction));
            int green = (int)Math.Round(Interpolate(lower.Value.G, upper.Value.G, fraction));
            int blue = (int)Math.Round(Interpolate(lower.Value.B, upper.Value.B, fraction));

            return System.Drawing.Color.FromArgb(alpha, red, green, blue);
        }

        /***************************************************/

        private static decimal Interpolate(decimal a, decimal b, decimal val)
        {
            return (b - a) * val + a;
        }

        /***************************************************/
    }
}



