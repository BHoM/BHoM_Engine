/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Matter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the unique Materials along with their relative proportions defining an object's make-up.")]
        [Input("elementM", "The element to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the element is composed of and in which ratios.")]
        public static MaterialComposition IMaterialComposition(this IElementM elementM)
        {
            if (elementM == null)
            {
                Base.Compute.RecordError("Cannot query the MaterialCompositions from a null element.");
                return null;
            }
            //IElementMs should implement one of the following:
            // -SolidVolume and MaterialComposition or
            // -VolumetricMaterialTakeoff
            //This method first checks if the MaterialComposition method can be found and run, and if so uses it.
            //If not, it falls back to running the VolumetricMaterialTakeoff method and gets the MaterialComposition from it.

            MaterialComposition matComp;
            if (TryGetMaterialComposition(elementM, out matComp))
                return matComp;
            else
            {
                VolumetricMaterialTakeoff takeoff;
                if (TryGetVolumetricMaterialTakeoff(elementM, out takeoff))
                    return Create.MaterialComposition(takeoff);
                else
                {
                    Base.Compute.RecordError($"The provided element of type {elementM.GetType()} does not implement MaterialComposition or VolumetricMaterialTakeoff methods. The MaterialComposition could not be extracted.");
                    return null;
                }
            }
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Tries running the MaterialComposition method on the IElementM. Returns true if the method successfully can be found.")]
        private static bool TryGetMaterialComposition(this IElementM elementM, out MaterialComposition materialComposition)
        {
            object result;
            bool success = Base.Compute.TryRunExtensionMethod(elementM, "MaterialComposition", out result);
            materialComposition = result as MaterialComposition;
            return success;
        }

        /***************************************************/
    }
}



