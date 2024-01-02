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
        [Input("checkForTakeoffFragment", "If true and the provided element is a BHoMObject, the incoming item is checked if it has a VolumetricMaterialTakeoff fragment attached, and if so, returns that Material composition corresponding to this fragment. If false, the MaterialComposition returned will be calculated, independant of fragment attached.")]
        [Output("materialComposition", "The kind of matter the element is composed of and in which ratios.")]
        public static MaterialComposition IMaterialComposition(this IElementM elementM, bool checkForTakeoffFragment = true)
        {
            if (elementM == null)
            {
                Base.Compute.RecordError("Cannot query the MaterialCompositions from a null element.");
                return null;
            }

            //If asked to look for fragment, and if fragment exists, return it
            if (checkForTakeoffFragment)
            {
                VolumetricMaterialTakeoff matTakeoff;
                if (TryGetVolumetricMaterialTakeoffFragment(elementM, out matTakeoff))
                    return Create.MaterialComposition(matTakeoff);
            }

            //IElementMs should implement one of the following:
            // -SolidVolume and MaterialComposition or
            // -VolumetricMaterialTakeoff
            //- GeneralMaterialTakeoff
            //This method first checks if the MaterialComposition method can be found and run, and if so uses it.
            //If not, it falls back to running the VolumetricMaterialTakeoff method and gets the MaterialComposition from it.

            MaterialComposition matComp;
            if (TryGetMaterialComposition(elementM, out matComp))
                return matComp;
            else
            {
                VolumetricMaterialTakeoff volTakeoff;
                if (TryGetVolumetricMaterialTakeoff(elementM, out volTakeoff))
                    return Create.MaterialComposition(volTakeoff);
                else if(TryGetGeneralMaterialTakeoff(elementM, out GeneralMaterialTakeoff generalTakeoff))
                    return Create.MaterialComposition(generalTakeoff);
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





