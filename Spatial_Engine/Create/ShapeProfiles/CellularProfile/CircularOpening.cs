﻿/*
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

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static CircularOpening CircularOpening(double diameter, double widthWebPost, double lengthEndPost = double.NaN, double spacerHeight = 0)
        {
            if (double.IsNaN(lengthEndPost))
                lengthEndPost = widthWebPost;

            if (widthWebPost > diameter)
            {
                Engine.Base.Compute.RecordError($"The {nameof(diameter)} needs to be larger than {nameof(widthWebPost)}. Unable to create circular opening.");
                return null;
            }

            double spacing = diameter + widthWebPost;

            return new CircularOpening(diameter, spacerHeight, widthWebPost, lengthEndPost, spacing);
        }

        /***************************************************/
    }
}