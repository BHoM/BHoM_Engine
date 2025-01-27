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

using BH.Engine.Base;
using BH.oM.Graphics;
using BH.oM.Graphics.Colours;
using BH.oM.Graphics.Enums;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Graphics
{
    public static partial class Modify
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Sets up the properties of a GradientOptions object for usage.")]
        [Input("gradientOptions", "GradientOptions object to modify.")]
        [Input("allValues", "The values to set gradient auto range from. Optional if range is already set.")]
        [Input("gradientBoundsWarning", "If true, a warning will be raised if the bounds have been manually set and any of the provided values are outside of the bounds domain.")]
        [Input("defaultGradient", "Sets which gradient to use as default if no gradient is already set. Defaults to BlueToRed.")]
        [Output("gradientOptions", "A GradientOptions object which is ready for usage.")]
        public static GradientOptions ApplyGradientOptions(this GradientOptions gradientOptions, IEnumerable<double> allValues = null, bool gradientBoundsWarning = true, string defaultGradient = "BlueToRed")
        {
            
            if (gradientOptions == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot apply gradientOptions because gradientOptions is null or invalid.");
                return null;
            }

            GradientOptions result = gradientOptions.ShallowClone();

            //Set up the bounds of the Gradient
            result.SetGradientBounds(allValues, gradientBoundsWarning);

            // Sets a default gradient if none is already set
            result.SetDefaultGradient(defaultGradient);

            // Centering Options
            result.ApplyGradientCentering();

            return result;
        }

        /***************************************************/

    }
}




