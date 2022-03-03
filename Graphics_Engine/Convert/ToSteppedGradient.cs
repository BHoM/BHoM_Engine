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

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static SteppedGradient ToSteppedGradient(this IGradient gradient, int nbSteps = -1)
        {
            if (gradient == null)
                return null;

            if (gradient is SteppedGradient)
                return gradient as SteppedGradient;

            if (nbSteps == -1)
                return new SteppedGradient { Markers = new SortedDictionary<decimal, System.Drawing.Color>(gradient.Markers) };
            else
            {
                decimal stepSize = 1.0m / (decimal)(nbSteps+1);
                double colourStepSize = 1.0 / (double)nbSteps;
                SteppedGradient steppedGradient = new SteppedGradient();
                for (int i = 0; i <= nbSteps; i++)
                {
                    decimal markerVal = stepSize * i;
                    steppedGradient.Markers[markerVal] = gradient.IColor(colourStepSize * i);
                }
                return steppedGradient;
            }
        }

        /***************************************************/
    }
}
