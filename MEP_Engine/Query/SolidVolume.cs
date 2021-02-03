/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.MEP.System;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Queries the solid volume of a Duct by multiplying the section profile's solid area by the element's length. Note this element contains a composite section and this query method returns a single summed value. If you want precise values per section profile, please use CompositeSolidVolumes.")]
        [Input("duct", "The Duct to query solid volume.")]
        [Output("solidVolume", "Combined SolidVolume of the Element's SectionProfiles.")]

        public static double SolidVolume(this IFlow obj)
        {
            double length = obj.Length();
            double elementSolidArea = obj.SectionProperty.ElementSolidArea;
            double liningSolidArea = obj.SectionProperty.LiningSolidArea;
            double insulationSolidArea = obj.SectionProperty.InsulationSolidArea;

            if (length <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("The object has no length. Returning NaN.");
                return double.NaN;
            }

            if (elementSolidArea <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("No Element profile was detected in your object.");
                elementSolidArea = 0;
            }

            if (liningSolidArea <= 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No Lining was detected in your object.");
                liningSolidArea = 0;
            }

            if (insulationSolidArea <= 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No Insulation was detected in your object.");
                liningSolidArea = 0;
            }

            return ((length * elementSolidArea) + (length * insulationSolidArea) + (length * liningSolidArea));
        }

        /***************************************************/
    }
}

