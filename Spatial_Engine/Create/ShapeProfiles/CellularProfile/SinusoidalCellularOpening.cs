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

        [Description("Creates a SinusoidalCellularOpening to be applied to a cellular/castellated beam.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [InputFromProperty("spacing")]
        [Output("opening", "The created SinusoidalCellularOpening.")]
        public static SinusoidalCellularOpening SinusoidalCellularOpening(double height, double width, double spacing)
        {
            if (width >= spacing)
            {
                Engine.Base.Compute.RecordError($"The {nameof(spacing)} needs to be larger than {nameof(width)}. Unable to create {nameof(SinusoidalCellularOpening)}.");
                return null;
            }
            if (spacing > 2 * width)
            {
                Engine.Base.Compute.RecordError($"The {nameof(spacing)} needs to be smaller than 2 * {nameof(width)}. Unable to create {nameof(SinusoidalCellularOpening)}.");
                return null;
            }

            double widthWebPost = spacing - width;
            double sinusoidalLength = (width - widthWebPost) / 2;

            return new SinusoidalCellularOpening(height, width, sinusoidalLength, widthWebPost, spacing);
        }

        /***************************************************/
    }
}


