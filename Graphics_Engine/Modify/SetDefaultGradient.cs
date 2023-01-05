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

        [Description("If the Gradient is null, sets the Gradient of the GradientOption to the a default Gradient by fetching the Library. If the gradient is already defined, no action is taken.")]
        [Input("gradientOptions", "The GradientOptions to set the default Gradient value to if null.")]
        [Input("defaultGradient", "The name of the default gradient to be used.")]
        [Output("gradientOptions", "GradientOptions with applied default gradient.")]
        public static void SetDefaultGradient(this GradientOptions gradientOptions, string defaultGradient = "BlueToRed")
        {
            if (gradientOptions == null)
                return;

            // Sets a default gradient if none is already set
            if (gradientOptions.Gradient == null)
            {
                gradientOptions.Gradient = Library.Query.Match("Graphics\\Gradients", defaultGradient) as Gradient;
                if (gradientOptions.Gradient == null)
                {
                    Compute.RecordError("Could not find gradient " + defaultGradient + " in the Library, make sure you have BHoM Datasets or create a custom gradient");
                }
            }
        }

        /***************************************************/
    }
}

