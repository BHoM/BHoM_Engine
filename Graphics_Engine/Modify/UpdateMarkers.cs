/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;


namespace BH.Engine.Graphics
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Updates the markers of the gradient based on the provided colours and positions.")]
        [Input("colours", "The colours of the new markers. Should be the same number of colours as positions.")]
        [Input("positions", "The positions of the new markers. SHould be values between 0 and 1 and the same number of positions as colours.")]
        [Output("gradient", "Gradient with updated markers.")]
        public static void UpdateMarkers(this IGradient gradient, IEnumerable<Color> colours, IEnumerable<decimal> positions)
        {
            if (gradient == null || colours.IsNullOrEmpty() || positions.IsNullOrEmpty())
                return;

            if (colours.Count() != positions.Count())
            {
                Engine.Base.Compute.RecordWarning("Different number and colours and positions provided. Gradient created will only contain information matching the shorter of the lists. For all input data to be used please provide the same number of colours and positions");
            }
            if (positions.Max() > 1 || positions.Min() < 0)
            {
                Engine.Base.Compute.RecordWarning("Gradients assumes positions between 0 and 1. Values outside this range will not be considered in colour interpolations");
            }
            gradient.Markers = new SortedDictionary<decimal, Color>(
                    colours.Zip(positions, (c, p) => new { c, p })
                    .ToDictionary(x => x.p, x => x.c));
        }

        /***************************************************/
    }
}

