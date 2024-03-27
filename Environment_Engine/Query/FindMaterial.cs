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

using BH.oM.Environment;
using BHM = BH.oM.Environment.MaterialFragments;
using BH.oM.Physical.Constructions;
using BH.oM.Physical.Materials;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns an instance of a given material type from a layer")]
        [Input("layer", "A Layer object")]
        [Input("materialType", "The type of material to get from the layer")]
        [Output("materialProperties", "The instance of that material type if it exists on the object, null otherwise")]
        public static T FindMaterial<T>(this Layer layer, Type materialType)
        {
            return (T)System.Convert.ChangeType(layer.Material.Properties.Where(x => x.GetType() == materialType).FirstOrDefault(), materialType);
        }
    }
}




