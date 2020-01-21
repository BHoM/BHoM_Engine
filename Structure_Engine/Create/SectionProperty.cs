/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Structure.MaterialFragments;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Section property from a Profile and a Material. The type of section that will be created depends on the material provided. Null material or unsupported materials will return a GenericSection.")]
        [Input("profile", "The profile of the section property")]
        [Input("material", "The material of the section property. Used to determain which type of section that will be created. If null or a not yet explicitly supported material type, a generic section will be created.")]
        [Input("name", "The name of the section property")]
        [Output("section", "The created section property of a type matching the material provided.")]
        public static IGeometricalSection SectionPropertyFromProfile(IProfile profile, IMaterialFragment material = null, string name = "")
        {
            MaterialType materialType = material == null ? MaterialType.Undefined : material.IMaterialType();

            switch (materialType)
            {
                case MaterialType.Steel:
                    return SteelSectionFromProfile(profile, material as Steel, name);
                case MaterialType.Concrete:
                    return ConcreteSectionFromProfile(profile, material as Concrete, name);
                case MaterialType.Aluminium:
                    return AluminiumSectionFromProfile(profile, material as Aluminium, name);
                case MaterialType.Timber:
                    return TimberSectionFromProfile(profile, material as Timber, name);
                case MaterialType.Rebar:
                case MaterialType.Tendon:
                case MaterialType.Glass:
                case MaterialType.Cable:
                case MaterialType.Undefined:
                default:
                    Reflection.Compute.RecordWarning("The BHoM does not currently explicitly support sections of material type " + materialType + ". A generic section has been created with the material applied to it");
                    return GenericSectionFromProfile(profile, material, name);
            }

        }

        /***************************************************/
    }
}
