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
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Applies the GradientCenteringOptions of the gradient by either modifying the bounds or the markers of the gradient.")]
        [Input("gradientOptions", "The GradientOptions to have its centering options applied.")]
        [Output("gradientOptions", "GradientOptions with centering options applied.")]
        public static void ApplyGradientCentering(this GradientOptions gradientOptions)
        {
            if (gradientOptions == null)
                return;

            switch (gradientOptions.GradientCenteringOptions)
            {
                case GradientCenteringOptions.Symmetric:
                    gradientOptions.UpperBound = Math.Max(Math.Abs(gradientOptions.UpperBound), Math.Abs(gradientOptions.LowerBound));
                    gradientOptions.LowerBound = -gradientOptions.UpperBound;
                    break;
                case GradientCenteringOptions.Asymmetric:
                    gradientOptions.Gradient = gradientOptions.Gradient.CenterGradientAsymmetric(gradientOptions.LowerBound, gradientOptions.UpperBound);
                    break;
                case GradientCenteringOptions.None:
                default:
                    break;
            }
        }

        /***************************************************/
    }
}



