/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Materials;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a concrete material. First constructs a concrete material fragment, then applies it to a new Material class")]
        [Input("youngsModulus", "Youngs modulus. Given in [Pa] or [N/m]. Will be stored on the material fragment")]
        [Input("poissonsRatio", "Poissons Ratio. Will be stored on the material fragment")]
        [Input("thermalExpansionCoeff", "Modulus of thermal expansion. Given in [1/°C] or [1/K]. Will be stored on the material fragment")]
        [Input("density", "Given as [kg/m3]. Will be stored on the base material")]
        [Input("dampingRatio", "Dynamic damping ratio of the material. Will be stored on the material fragment")]
        [Input("cubeStrength", "Cube strength of the concrete material. Given in [Pa] or [N/m]. Will be stored on the material fragment")]
        [Input("cylinderStrength", "Cylinder strength of the concrete material. Given in [Pa] or [N/m].Will be stored on the material fragment")]
        [Input("embodiedCarbon", "Embodied carbon for the material. Will be stored on the material fragment")]
        [Output("Material", "The created material with a concrete fragment")]
        public static Concrete Concrete(string name, double youngsModulus = 33000000000, double poissonsRatio = 0.2, double thermalExpansionCoeff = 0.00001, double density = 2550, double dampingRatio = 0, double cubeStrength = 0, double cylinderStrength = 0, double embodiedCarbon = 0.12)
        {
            return new Concrete()
            {
                Name = name,
                Density = density,
                YoungsModulus = youngsModulus,
                PoissonsRatio = poissonsRatio,
                ThermalExpansionCoeff = thermalExpansionCoeff,
                CubeStrength = cubeStrength,
                DampingRatio = dampingRatio,
                CylinderStrength = cylinderStrength,
                EmbodiedCarbon = embodiedCarbon,
            };
        }

        /***************************************************/

    }
}

