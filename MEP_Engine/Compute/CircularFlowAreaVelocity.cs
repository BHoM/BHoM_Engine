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

using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the circular flow area velocity given a volumetric flow rate and circular diameter. Typically used in pressure drop calculations")]
        [Input("volumetricFlowRate", "Volumetric flow rate of fluid through fluid flow area.")]
        [Input("circularDiameter", "Circular diameter of a fluid flow area, typically referred to as equivalent circular diameter given any non-ciruclar flow area.")]
        [Output("circularFlowAreaVelocity", "The velocity of the fluid through the flow area.")]
        public static double CircularFlowAreaVelocity(double volumetricFlowRate, double circularDiameter)
        {
            double output;
            if(volumetricFlowRate == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the circular flow area velocity from a null volumetric flow rate.");
                return -1;
            }

            if(circularDiameter == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the circular flow area velocity from a null circular diameter.");
                return -1;
            }

            double circularArea = Math.PI * Math.Pow((circularDiameter / 2),2);
            double velocity = volumetricFlowRate / circularArea;
            output = velocity;
           
           
            return output;
        }

        /***************************************************/

    }
}

