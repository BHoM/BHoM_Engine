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
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Linq;
using BH.oM.Physical.Materials;

namespace BH.Engine.Matter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns an element's solid volume")]
        [Input("elementM", "The element to get the volume from")]
        [Input("checkForTakeoffFragment", "If true and the provided element is a BHoMObject, the incoming item is checked if it has a VolumetricMaterialTakeoff fragment attached, and if so, returns that total volume corresponding to this fragment. If false, the SolidVolume returned will be calculated, independant of fragment attached.")]
        [Output("volume", "The element's solid material volume.", typeof(Volume))]
        public static double ISolidVolume(this IElementM elementM, bool checkForTakeoffFragment = false)
        {
            if(elementM == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the solid volume of a null element.");
                return double.NaN;
            }

            //If asked to look for fragment, and if fragment exists, return it
            VolumetricMaterialTakeoff matTakeoff;
            if (TryGetVolumetricMaterialTakeoffFragment(elementM, checkForTakeoffFragment, out matTakeoff))
                return matTakeoff.SolidVolume();

            //IElementMs should implement one of the following:
            // -SolidVolume and MaterialComposition or
            // -VolumetricMaterialTakeoff
            //This method first checks if the SolidVolume method can be found and run, and if so uses it.
            //If not, it falls back to running the VolumetricMaterialTakeoff method and gets the SolidVolume from it.
            double volume;
            if (TryGetSolidVolume(elementM, out volume))
                return volume;
            else
            {
                VolumetricMaterialTakeoff takeoff;
                if (TryGetVolumetricMaterialTakeoff(elementM, out takeoff))
                    return takeoff.SolidVolume();
                else
                {
                    Base.Compute.RecordError($"The provided element of type {elementM.GetType()} does not implement SolidVolume or VolumetricMaterialTakeoff methods. The volume could not be extracted.");
                    return double.NaN;
                }
            }
        }

        /******************************************/

        [Description("Returns the total solid volume of the provided VolumetricMaterialTakeoff.")]
        [Input("VolumetricMaterialTakeoff", "The VolumetricMaterialTakeoff to get the total SolidVolume from.")]
        [Output("volume", "The total volumetric amount of matter in the VolumetricMaterialTakeoff", typeof(Volume))]
        public static double SolidVolume(this VolumetricMaterialTakeoff volumetricMaterialTakeoff)
        {
            if (volumetricMaterialTakeoff == null)
            {
                Base.Compute.RecordError("Connat query the solid volume from a null VolumetricMaterialTakeoff.");
                return double.NaN;
            }

            return volumetricMaterialTakeoff.Volumes.Sum();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Tries running the SolidVolume method on the IElementM. Returns true if the method successfully can be found.")]
        private static bool TryGetSolidVolume(this IElementM elementM, out double volume)
        {
            object result;
            bool success = Base.Compute.TryRunExtensionMethod(elementM, "SolidVolume", out result);
            volume = success ? (double)result : double.NaN;
            return success;
        }

        /***************************************************/
    }
}


