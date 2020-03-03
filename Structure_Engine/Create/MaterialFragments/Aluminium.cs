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

        [Description("Creates a aluminium material. First constructs a Aluminium material fragment, then applies it to a new Material class")]
        [Input("e", "Youngs modulus. Given in [Pa] or [N/m]. Will be stored on the material fragment")]
        [Input("v", "Poissons Ratio. Will be stored on the material fragment")]
        [Input("tC", "Modulus of thermal expansion. Given in [1/°C] or [1/K]. Will be stored on the material fragment")]
        [Input("density", "Given as [kg/m3]. Will be stored on the base material")]
        [Input("dampingRatio", "Dynamic damping ratio of the material. Will be stored on the material fragment")]
        [Input("embodiedCarbon", "Embodied carbon for the material. Will be stored on the material fragment")]
        [Output("Material", "The created material with a aluminium fragment")]
        public static Aluminium Aluminium(string name, double e = 70000000000, double v = 0.34, double tC = 0.000023, double density = 2710, double dampingRatio = 0, double embodiedCarbon = 7.9)
        {
            return new Aluminium()
            {
                Name = name,
                Density = density,
                YoungsModulus = e,
                PoissonsRatio = v,
                ThermalExpansionCoeff = tC,
                DampingRatio = dampingRatio,
                EmbodiedCarbon = embodiedCarbon,
            };
        }

        /***************************************************/
    }
}

