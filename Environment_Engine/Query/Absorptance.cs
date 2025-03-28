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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Physical.Constructions;
using BH.oM.Physical.Materials;
using BH.oM.Environment.MaterialFragments;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the absorptance of a construction")]
        [Input("construction", "A Construction object")]
        [Output("absorptance", "The total absorptance of all materials on the construction")]
        public static double Absorptance(this Construction construction)
        {
            if(construction == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the absorptance of a null construction.");
                return 0;
            }

            double absorptance = 0.0;
            foreach (Layer l in construction.Layers)
                absorptance += l.Absorptance();

            return absorptance;
        }

        [Description("Returns the absorptance of a layer")]
        [Input("layer", "A Layer object")]
        [Output("absorptance", "The absorptance of the material on the layer")]
        public static double Absorptance(this Layer layer)
        {
            if (layer == null || layer.Material == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the absorptance of a null layer or where the layer has a null material.");
                return 0;
            }

            return layer.Material.Absorptance();
        }

        [Description("Returns the absorptance of a material calculated as 1 minus the maximum emissivity (external and internal)")]
        [Input("material", "A Material object")]
        [Output("absorptance", "The absorptance of the material")]
        public static double Absorptance(this Material material)
        {
            if (material == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the absorptance of a null material.");
                return 0;
            }

            SolidMaterial materialProperties = material.Properties.Where(x => x is SolidMaterial).FirstOrDefault() as SolidMaterial;
            if (materialProperties == null) return 0.0;

            double maxEmissivity = Math.Max(materialProperties.EmissivityExternal, materialProperties.EmissivityInternal);
            if (maxEmissivity > 1 || maxEmissivity < 0)
            {
                BH.Engine.Base.Compute.RecordError("Maximum emissivity was greater than 1 or less than 0 and so absorptance cannot be accurately calculated");
                return 0.0;
            }

            return 1 - maxEmissivity;
        }
    }
}





