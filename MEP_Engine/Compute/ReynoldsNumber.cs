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

        [Description("Calculates the reynolds number for a duct given fluid velocity, duct circular diameter, and fluid kinematic viscotity. Typically used in pressure drop calculations. If Reynolds number is ommited, default values apply. From ASHRAE 2021 Fundamentals (SI) Chapter 21 Duct Design, Equation 20 and 21.")]
        [Input("fluidVelocity", "fluid flow velocity [m/s]")]
        [Input("circularDiameter", "Circular diameter of a fluid flow area, typically referred to as equivalent circular diameter given any non-ciruclar flow area. [m]")]
        [Input("fluidKinematicViscosity", "fluid kinematice viscosity [m2/s], ")]
        [Output("reynoldsNumber", "The reynolds number for fluid flow through the flow area.[unitless]")]
        public static double ReynoldsNumber(double fluidVelocity, double circularDiameter, double fluidKinematicViscosity = double.NaN)
        {
            double output;
            double reynoldsNumber;
            //convert to mm
            circularDiameter *= 1000;

            if (fluidVelocity == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction factor from a null fluid velocity.");
                return -1;
            }

            if (circularDiameter == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction factor from a null circular diameter.");
                return -1;
            }

            ///For standard air and temperature between 4C and 38C, it is acceptable to use the equation below.
            if (fluidKinematicViscosity == double.NaN)
            {
                reynoldsNumber = 66.4* circularDiameter * fluidVelocity;
                output = reynoldsNumber;

                return output;
            }

            reynoldsNumber = (circularDiameter * fluidVelocity) / (1000 * fluidKinematicViscosity);
            output = reynoldsNumber;

            return output;
        }

        /***************************************************/

    }
}

