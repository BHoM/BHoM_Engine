/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Environment.MaterialFragments;
using BH.oM.Physical.Materials;
using BH.oM.Physical.Constructions;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the RValue of a construction layer")]
        [Input("layer", "A Layer object")]
        [Output("rValue", "The rValue of the layer calculated as the layers thickness divided by the materials conductivity")]
        public static double RValue(this Layer layer)
        {
            if(layer == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the RValue of a null layer.");
                return -1;
            }

            //rValue is calculated as being the thickness of the layer dividied by the materials conductivity

            IEnvironmentMaterial envMaterial = layer.Material.Properties.Where(x => x is IEnvironmentMaterial).FirstOrDefault() as IEnvironmentMaterial;
            
            if (envMaterial == null)
                return 0.0;

            return (layer.Thickness / envMaterial.Conductivity);
        }
    }
}



