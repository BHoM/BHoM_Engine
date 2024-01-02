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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Physical.Materials;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Physical
{
    public static partial class Modify
    {
        [Description("Assign a new set of material properties to a material")]
        [Input("material", "A material to add properties to")]
        [Input("newProperties", "The properties to add to the material")]
        [Output("The updated Material")]
        public static Material AddMaterialProperties(this Material material, IMaterialProperties newProperties)
        {
            if (material == null)
                return material;

            if (newProperties != null && material.Properties.Where(x => x.GetType() == newProperties.GetType()).FirstOrDefault() != null)
                BH.Engine.Base.Compute.RecordError("Properties of that type already exist on this material - please remove them before adding new properties of that type");
            else
                material.Properties.Add(newProperties);

            return material;
        }
    }
}





