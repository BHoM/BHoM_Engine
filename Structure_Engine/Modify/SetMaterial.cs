/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using BH.Engine.Base;


namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sets the material of a Bar by updating the material of its SectionProperty.")]
        [Input("bar", "The Bar to set the material to, i.e. the Bar to have the material of its SectionProperty updated.")]
        [Input("material", "The material to set to the Bar.")]
        [Output("bar", "The Bar with updated material.")]
        public static Bar SetMaterial(this Bar bar, IMaterialFragment material = null)
        {
            if (bar.IsNull())
                return null;

            Bar clone = bar.ShallowClone();
            if (bar.SectionProperty == null)
            {
                Engine.Base.Compute.RecordError("The section property parameter is null - material has not been assigned");
                return clone;
            }
            ISectionProperty sectionClone = bar.SectionProperty.ShallowClone();
            sectionClone.Material = material;
            clone.SectionProperty = sectionClone;
            return clone;
        }

        /***************************************************/

    }

}





