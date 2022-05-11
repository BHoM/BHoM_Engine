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

        [Description("Calculates the  friction losses for a duct given duct length, circular diameter, volumetric flow rate, surface roughness, fluid density, and fluid kinematic viscosity. Typically used in pressure drop calculations")]
        [Input("ductLength", "length of duct [m]")]
        [Input("circularDiameter", "Circular diameter of a fluid flow area, typically referred to as equivalent circular diameter given any non-ciruclar flow area., [m]")]
        [Input("volumetricFlowRate", "Volumetric flow rate of fluid through fluid flow area.")]
        [Input("surfaceRoughness", "Surfae roughness [m]")]
        [Input("fluidDensity", "fluid density [kg/m3]")]
        [Input("fluidKinematicViscosity", "fluid kinematice viscosity [m2/s]")]
        [Output("frictionLosses", "The friction losses due to fluid flow through the flow area.[pa]")]
        public static double DuctStraightRunPressureFrictionLosses(double ductLength, double circularDiameter, double volumetricFlowRate, double surfaceRoughness, double fluidDensity, double fluidKinematicViscosity)
        {
            double output;
            if (ductLength == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null duct length.");
                return -1;
            }

            if (circularDiameter == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null circular diameter.");
                return -1;
            }

            if (volumetricFlowRate == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null volumetric flow rate.");
                return -1;
            }

            if (surfaceRoughness == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null surface roughness.");
                return -1;
            }

            if (fluidDensity == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null fluid density.");
                return -1;
            }

            if (fluidKinematicViscosity == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null fluid kinemetatic viscosity.");
                return -1;
            }
            
            

            double _circularFlowAreaVelocity = CircularFlowAreaVelocity(volumetricFlowRate,circularDiameter);
            double _reynoldsNumber = ReynoldsNumber(_circularFlowAreaVelocity, circularDiameter, fluidKinematicViscosity);
            double _frictionFactor = FrictionFactor(_reynoldsNumber, circularDiameter, surfaceRoughness);
            //convert to mm
            circularDiameter *= 1000;
            double frictionLosses = (1000 * _frictionFactor*ductLength/circularDiameter) * (fluidDensity/2D) * Math.Pow(_circularFlowAreaVelocity,2);
            output = frictionLosses;
           
           
            return output;
        }

        /***************************************************/

    }
}

