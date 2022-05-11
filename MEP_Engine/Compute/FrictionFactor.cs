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

        [Description("Calculates the  friction factor for a duct given reynolds number, circular diameter, and surface roughness. Typically used in pressure drop calculations")]
        [Input("reynoldsNumber", "Reynolds number, [unitless]")]
        [Input("circularDiameter", "Circular diameter of a fluid flow area, typically referred to as equivalent circular diameter given any non-ciruclar flow area., [m]")]
        [Input("surfaceRoughness", "Surfae roughness [m]")]
        [Output("circularFlowAreaVelocity", "The velocity of the fluid through the flow area.")]
        public static double FrictionFactor(double reynoldsNumber, double circularDiameter, double surfaceRoughness)
        {
            double output;
            if(reynoldsNumber == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction factor from a null reynolds number.");
                return -1;
            }

            if(circularDiameter == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction factor from a null circular diameter.");
                return -1;
            }

            if (surfaceRoughness == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction factor from a null surface roughness.");
                return -1;
            }

            double componentA = Math.Pow(2.457 * Math.Log(1/(Math.Pow(7/reynoldsNumber,0.9)+(0.27*surfaceRoughness/circularDiameter))),16);
            double componentB = Math.Pow((37530/reynoldsNumber),16);


            
            double frictionFactor = 8 * Math.Pow(Math.Pow(8/reynoldsNumber,12)+ (1/Math.Pow(componentA + componentB,1.5)),1/12);
            output = frictionFactor;
           
           
            return output;
        }

        /***************************************************/

    }
}

