/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using BH.oM.Physical.Materials;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Geometry;
using BH.Engine.Base;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Assigns a MaterialFragment to a physical Material. First removes any previous instances of the structural IMaterialFragment from the list, then adds the new.")]
        [Input("material", "The physical material to add a structural fragment to.")]
        [Input("structuralFragment", "The structural fragment to be appended to the material. Will replace any previous instance.")]
        [Output("material", "Physical Material with structural MaterialFragment set.")]
        public static Material SetStructuralFragment(this Material material, IMaterialFragment structuralFragment)
        {
            if (material == null)
            {
                Base.Compute.RecordError("Physical Material provided is null and therefore SetStructuralFragment cannot be evaluated.");
                return null;
            }

            //Clone the object
            Material clone = material.ShallowClone();

            //null check for the list
            clone.Properties = clone.Properties ?? new List<IMaterialProperties>();

            //Remove any reference to old structural material fragment. Only one is allowed
            clone.Properties = clone.Properties.Where(x => !(x is IMaterialFragment)).ToList();

            //Assign the new fragment
            clone.Properties.Add(structuralFragment);

            return clone;
        }

        /***************************************************/

    }
}






