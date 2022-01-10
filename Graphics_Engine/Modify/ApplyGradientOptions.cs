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
        [Input("defaultGradient", "Sets which gradient to use as default if no gradient is already set. Defaults to BlueToRed.")]
        [Output("gradientOptions", "A GradientOptions object which is ready for usage.")]
        public static GradientOptions ApplyGradientOptions(this GradientOptions gradientOptions, IEnumerable<double> allValues = null, string defaultGradient = "BlueToRed")
        {
            
            if (gradientOptions == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot apply gradientOptions because gradientOptions is null or invalid.");
                return null;
            }

            GradientOptions result = gradientOptions.ShallowClone();

            // Checks if bounds exist or can be automatically set
            if ((double.IsNaN(result.UpperBound) || double.IsNaN(result.LowerBound)) && (allValues == null || allValues.Count() < 1))
            {
                BH.Engine.Reflection.Compute.RecordError("No bounds have been manually set for Gradient, and no values are provided by which to set them.");
                return result;
            }

            // Optional auto-domain
            if (double.IsNaN(result.LowerBound))
                result.LowerBound = allValues.Min();
            if (double.IsNaN(result.UpperBound))
                result.UpperBound = allValues.Max();
            
            // Sets a default gradient if none is already set
            if (result.Gradient == null)
            {
                result.Gradient = Library.Query.Match("Gradients", defaultGradient) as Gradient;
                if (result.Gradient == null)
                {
                    Engine.Reflection.Compute.RecordError("Could not find gradient " + defaultGradient + " in the Library, make sure you have BHoM Datasets or create a custom gradient");
                    return null;
                }
            }

            // Centering Options
            switch (result.GradientCenteringOptions)
            {
                case GradientCenteringOptions.Symmetric:
                    result.UpperBound = Math.Max(Math.Abs(result.UpperBound), Math.Abs(result.LowerBound));
                    result.LowerBound = -result.UpperBound;
                    break;
                case GradientCenteringOptions.Asymmetric:
                    result.Gradient = result.Gradient.CenterGradientAsymmetric(result.LowerBound, result.UpperBound);
                    break;
                case GradientCenteringOptions.None:
                default:
                    break;
            }

            return result;
        }

        /***************************************************/

    }
}

