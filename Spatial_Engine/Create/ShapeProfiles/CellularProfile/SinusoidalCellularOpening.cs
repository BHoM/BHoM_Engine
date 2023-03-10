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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.oM.Spatial.ShapeProfiles.CellularOpenings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a SinusoidalCellularOpening to be applied to a cellular/castelated beam.")]
        [InputFromProperty("height")]
        [Input("openingWidth", "Total width of the opening. Needs to be smaller than the spacing.", typeof(Length))]
        [InputFromProperty("spacing")]
        [Output("opening", "The created SinusoidalCellularOpening.")]
        public static SinusoidalCellularOpening SinusoidalCellularOpening(double height, double openingWidth, double spacing)
        {
            if (openingWidth > spacing)
            {
                Engine.Base.Compute.RecordError($"The {nameof(spacing)} needs to be larger than {nameof(openingWidth)}. Unable to create {nameof(SinusoidalCellularOpening)}.");
                return null;
            }

            double widthWebPost = spacing - openingWidth;
            double sinusoidalLength = (openingWidth - widthWebPost) / 2;

            return new SinusoidalCellularOpening(height, sinusoidalLength, widthWebPost, spacing);
        }

        /***************************************************/
    }
}
