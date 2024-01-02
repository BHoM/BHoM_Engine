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
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sets the bounds of the gradient based on provided values. If the bounds are already set, no action will be taken. If not set, min and max values of the provided values will be used to set the Bounds.")]
        [Input("gradientOptions", "The GradientOptions to set the bounds to.")]
        [Input("allValues", "The values to use to update the gradient bounds. LowerBound will be set to Min value if it is NaN and UpperBound set to Max value if NaN.")]
        [Input("gradientBoundsWarning", "If true, a warning will be raised if the bounds have been manually set and any of the provided values are outside of the bounds domain.")]
        [Output("gradientOptions", "GradientOptions with updated unset bounds.")]
        public static void SetGradientBounds(this GradientOptions gradientOptions, IEnumerable<double> allValues = null, bool gradientBoundsWarning = true)
        {
            if (gradientOptions == null)
                return;

            // Checks if bounds exist or can be automatically set
            if ((double.IsNaN(gradientOptions.UpperBound) || double.IsNaN(gradientOptions.LowerBound)) && (allValues == null || allValues.Count() < 1))
            {
                BH.Engine.Base.Compute.RecordError("No bounds have been manually set for Gradient, and no values are provided by which to set them.");
                return;
            }

            // Optional auto-domain
            if (double.IsNaN(gradientOptions.LowerBound))
                gradientOptions.LowerBound = allValues.Min();
            else if (gradientBoundsWarning && allValues.Any(x => x < gradientOptions.LowerBound))
                Compute.RecordWarning("Some values are smaller than the preset LowerBound. Values below the LowerBound will get a colour equal to that of the LowerBound");

            if (double.IsNaN(gradientOptions.UpperBound))
                gradientOptions.UpperBound = allValues.Max();
            else if (gradientBoundsWarning && allValues.Any(x => x > gradientOptions.UpperBound))
                Compute.RecordWarning("Some values are larger than the preset UpperBound. Values above the UpperBound will get a colour equal to that of the UpperBound");
        }

        /***************************************************/
    }
}


