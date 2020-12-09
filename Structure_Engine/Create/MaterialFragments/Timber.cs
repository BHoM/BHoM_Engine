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

        [Description("Creates a structural Timber material fragment to be used on analytical structural elements, or as a fragment of the physical material.")]
        [Input("name", "The name of the created Timber material. This is required for various structural packages to create the object.")]
        [InputFromProperty("youngsModulus")]
        [InputFromProperty("poissonsRatio")]
        [InputFromProperty("shearModulus")]
        [InputFromProperty("thermalExpansionCoeff")]
        [InputFromProperty("density")]
        [InputFromProperty("dampingRatio")]
        [Output("timber", "The created structural Timber material fragment.")]
        [PreviousVersion("4.0", "BH.Engine.Structure.Create.Timber(System.String, BH.oM.Geometry.Vector, BH.oM.Geometry.Vector, BH.oM.Geometry.Vector, BH.oM.Geometry.Vector, System.Double, System.Double, System.Double)")]
        public static Timber Timber(string name, Vector youngsModulus, Vector poissonsRatio, Vector shearModulus, Vector thermalExpansionCoeff, double density, double dampingRatio, double embodiedCarbon = 0.4)
        {
            return new Timber()
            {
                Name = name,
                Density = density,
                YoungsModulus = youngsModulus,
                PoissonsRatio = poissonsRatio,
                ShearModulus = shearModulus,
                ThermalExpansionCoeff = thermalExpansionCoeff,
                DampingRatio = dampingRatio,
            };

        }

        /***************************************************/
    }
}
