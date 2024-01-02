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

using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Physical.Materials;
using BH.oM.Physical.Constructions;
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

        [Description("Determines whether a material is transparent or not")]
        [Input("material", "A Physical Material")]
        [Output("isTransparent", "True if the material is transparent, false otherwise")]
        public static bool IsTransparent(this Material material)
        {
            if(material == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the transparency of a null material.");
                return false;
            }

            if (material.Properties.Where(x => x is IEnvironmentMaterial).FirstOrDefault() == null)
                return false; //No Environment Material Fragment on this material

            if (material.Properties.Where(x => x is GasMaterial).FirstOrDefault() != null)
                return true; //All gas materials are transparent

            if (material.Properties.Where(x => x is SolidMaterial).FirstOrDefault() == null)
                return false; //Protection in case there is no solid material on this material

            return ((material.Properties.Where(x => x is SolidMaterial).First() as SolidMaterial).LightTransmittance > 0); //Solid material is transparent (partially or wholly) if it can transmit some light
        }

        [Description("Determines whether a layer is transparent or not")]
        [Input("layer", "A Physical Layer containing a single material")]
        [Output("isTransparent", "True if the material on the layer is transparent, false otherwise")]
        public static bool IsTransparent(this Layer layer)
        {
            if (layer == null || layer.Material == null)
                return false;

            return layer.Material.IsTransparent();
        }
    }
}







