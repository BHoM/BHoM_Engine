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

using BH.oM.Geometry;
using BH.oM.Graphics;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

using BH.Engine.Base;

namespace BH.Engine.Graphics
{
    public static partial class Create
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Creates a colour gradient with colour values corresponding to values between 0 and 1.")]
        [Input("colours", "A list of colours for the gradient.")]
        [Input("positions", "A corresponding list of positions for the coloured markers between 0 and 1.")]
        [Output("gradient", "A colour Gradient.")]
        public static SteppedGradient SteppedGradient(IEnumerable<Color> colours, IEnumerable<decimal> positions)
        {
            if (colours.IsNullOrEmpty() || positions.IsNullOrEmpty())
                return null;

            if (colours.Count() != positions.Count())
            {
                Engine.Base.Compute.RecordWarning("Different number and colours and positions provided. Gradient created will only contain information matching the shorter of the lists. For all input data to be used please provide the same number of colours and positions");
            }
            if (positions.Max() > 1 || positions.Min() < 0)
            {
                Engine.Base.Compute.RecordWarning("Gradients assumes positions between 0 and 1. Values outside this range will not be considered in colour interpolations");
            }
            return new SteppedGradient()
            {
                Markers = new SortedDictionary<decimal, Color>(
                    colours.Zip(positions, (c, p) => new { c, p })
                    .ToDictionary(x => x.p, x => x.c)
                    )
            };
        }

        /***************************************************/

    }
}





