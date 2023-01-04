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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.Engine.Base;
using BH.oM.Graphics;

namespace BH.Engine.Graphics
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Coverts a gradient to a stepped gradient by extracting colours in intervals.")]
        [Input("gradient", "The gradient to convert. If it is already a stepped gradient, no action will be taken.")]
        [Input("steps", "(Optional, defaults to -1) Number of steps to be used. If -1, the markers of the provided gradient will be used.")]
        [Output("stepGradient", "The new stepped gradient.")]
        public static SteppedGradient ToSteppedGradient(this IGradient gradient, int steps = -1)
        {
            if (gradient == null)
                return null;

            if (gradient is SteppedGradient)
                return gradient as SteppedGradient;

            if (steps == -1)
                return new SteppedGradient { Markers = new SortedDictionary<decimal, System.Drawing.Color>(gradient.Markers) };
            else if (steps > 0)
            {
                decimal stepSize = 1.0m / (decimal)(steps + 1);
                double colourStepSize = 1.0 / (double)steps;
                SteppedGradient steppedGradient = new SteppedGradient();
                for (int i = 0; i <= steps; i++)
                {
                    decimal markerVal = stepSize * i;
                    steppedGradient.Markers[markerVal] = gradient.IColor(colourStepSize * i);
                }
                return steppedGradient;
            }
            else
            {
                Compute.RecordError("nbSteps needs to be either -1 for using the same markers as the provided gradient or a positive integer.");
                return null;
            }
        }

        /***************************************************/
    }
}

