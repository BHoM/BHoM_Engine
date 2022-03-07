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

using BH.Engine.Base;
using BH.oM.Graphics;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Graphics
{
    public static partial class Modify
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Sets the colour in the middle of a gradient to 0 relative to a pair of boundary values.")]
        [Input("gradient", "The gradient to modify.")]
        [Input("lowerBound", "The lower bound that the gradient will be used with.")]
        [Input("upperBound", "The upper bound that the gradient will be used with.")]
        [Output("gradient", "A gradient with its middle colour set to 0 relative to the pair of boundary values.")]
        [PreviousInputNames("lowerBound", "from")]
        [PreviousInputNames("upperBound", "to")]
        public static IGradient CenterGradientAsymmetric(this IGradient gradient, double lowerBound, double upperBound)
        {
            if (gradient?.Markers == null || gradient.Markers.Count < 2)
            {
                BH.Engine.Base.Compute.RecordError("Cannot edit gradient because gradient is null or invalid.");
                return null;
            }

            IGradient result = gradient.ShallowClone();

            // Add a marker to avoid issues when deleting and transforming
            if (!result.Markers.ContainsKey((decimal)0.5))
                result.Markers.Add((decimal)0.5, result.IColor(0.5));

            if (upperBound <= 0)
            {
                // Scale marker positions to span 0 to 2 and delete those above 1
                result.UpdateMarkers(result.Markers.Where(x => x.Key <= (decimal)0.5).Select(x => x.Value),
                                                    result.Markers.Keys.Select(x => x * 2).Where(x => x <= 1));
            }
            else if (lowerBound >= 0)
            {
                // Scale marker positions to span -1 to 1 and delete those below 0
                result.UpdateMarkers(result.Markers.Where(x => x.Key >= (decimal)0.5).Select(x => x.Value),
                                                    result.Markers.Keys.Select(x => (x - 1) * 2 + 1).Where(x => x >= 0));
            }
            else
            {
                // found = the relative position of zero on the gradient
                decimal found = (decimal)(-lowerBound / (upperBound - lowerBound));
                // scale marker positions below 'found' from: 0 to 0.5 => 0 to found
                // scale marker positions above 'found' from: 0.5 to 1 => found to 1
                result.UpdateMarkers(result.Markers.Values,
                                     result.Markers.Keys.Select(x => x > 0.5m ?
                                                                (x - 1) * 2 * (1 - found) + 1 :
                                                                 x * 2 * found));
            }

            return result;
        }

        /***************************************************/
    }
}

