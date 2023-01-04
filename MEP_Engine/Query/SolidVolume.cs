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

using System.ComponentModel;
using BH.oM.Base.Attributes;
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

        public static double SolidVolume(this Duct duct)
        {
            double length = duct.Length();
            double elementSolidArea = duct.SectionProperty.ElementSolidArea;
            double insulationSolidArea = duct.SectionProperty.InsulationSolidArea;
            double liningSolidArea = duct.SectionProperty.LiningSolidArea;

            if(length <= 0)
            {
                Engine.Base.Compute.RecordError("Cannot query SolidVolume from zero length members.");
                return double.NaN;
            }

            if(duct.SectionProperty.SectionProfile.ElementProfile == null)
            {
                Engine.Base.Compute.RecordWarning("No ElementProfile detected for object " + duct.BHoM_Guid);
            }

            if (duct.SectionProperty.SectionProfile.InsulationProfile == null)
            {
                Engine.Base.Compute.RecordWarning("No InsulationProfile detected for object " + duct.BHoM_Guid);
            }

            if (duct.SectionProperty.SectionProfile.LiningProfile == null)
            {
                Engine.Base.Compute.RecordWarning("No LiningProfile detected for object " + duct.BHoM_Guid);
            }

            if (elementSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("ElementSolidArea is 0. Returning 0 for ElementSolidVolume.");
            }

            if (insulationSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("InsulationSolidArea is 0. Returning 0 for LiningSolidVolume.");
            }

            if (liningSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("LiningSolidArea is 0. Returning 0 for InsulationSolidVolume.");
            }

            return ((length * elementSolidArea) + (length * insulationSolidArea) + (length * liningSolidArea));
        }

        /***************************************************/

        [Description("Queries the solid volume of a Pipe by multiplying the section profile's solid area by the element's length. Note this element contains a composite section and this query method returns a single summed value. If you want precise values per section profile, please use CompositeSolidVolumes.")]
        [Input("pipe", "The Pipe to query solid volume.")]
        [Output("solidVolume", "Combined SolidVolume of the Element's SectionProfiles.")]

        public static double SolidVolume(this Pipe pipe)
        {
            double length = pipe.Length();
            double elementSolidArea = pipe.SectionProperty.ElementSolidArea;
            double insulationSolidArea = pipe.SectionProperty.InsulationSolidArea;
            double liningSolidArea = pipe.SectionProperty.LiningSolidArea;

            if (length <= 0)
            {
                Engine.Base.Compute.RecordError("Cannot query SolidVolume from zero length members.");
                return double.NaN;
            }

            if (pipe.SectionProperty.SectionProfile.ElementProfile == null)
            {
                Engine.Base.Compute.RecordWarning("No ElementProfile detected for object " + pipe.BHoM_Guid);
            }

            if (pipe.SectionProperty.SectionProfile.InsulationProfile == null)
            {
                Engine.Base.Compute.RecordWarning("No InsulationProfile detected for object " + pipe.BHoM_Guid);
            }

            if (pipe.SectionProperty.SectionProfile.LiningProfile == null)
            {
                Engine.Base.Compute.RecordWarning("No LiningProfile detected for object " + pipe.BHoM_Guid);
            }

            if (elementSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("ElementSolidArea is 0. Returning 0 for ElementSolidVolume.");
            }

            if (insulationSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("InsulationSolidArea is 0. Returning 0 for LiningSolidVolume.");
            }

            if (liningSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("LiningSolidArea is 0. Returning 0 for InsulationSolidVolume.");
            }

            return ((length * elementSolidArea) + (length * insulationSolidArea) + (length * liningSolidArea));
        }

        /***************************************************/

        [Description("Queries the solid volume of a WireSegment by multiplying the section profile's solid area by the element's length. Note this element contains a composite section and this query method returns a single summed value. If you want precise values per section profile, please use CompositeSolidVolumes.")]
        [Input("wireSegment", "The WireSegment to query solid volume.")]
        [Output("solidVolume", "Combined SolidVolume of the Element's SectionProfiles.")]

        public static double SolidVolume(this WireSegment wireSegment)
        {
            double length = wireSegment.Length();
            double elementSolidArea = wireSegment.SectionProperty.ElementSolidArea;
            double insulationSolidArea = wireSegment.SectionProperty.InsulationSolidArea;
            double liningSolidArea = wireSegment.SectionProperty.LiningSolidArea;

            if (length <= 0)
            {
                Engine.Base.Compute.RecordError("Cannot query SolidVolume from zero length members.");
                return double.NaN;
            }

            if (wireSegment.SectionProperty.SectionProfile.ElementProfile == null)
            {
                Engine.Base.Compute.RecordWarning("No ElementProfile detected for object " + wireSegment.BHoM_Guid);
            }

            if (wireSegment.SectionProperty.SectionProfile.InsulationProfile == null)
            {
                Engine.Base.Compute.RecordWarning("No InsulationProfile detected for object " + wireSegment.BHoM_Guid);
            }

            if (wireSegment.SectionProperty.SectionProfile.LiningProfile == null)
            {
                Engine.Base.Compute.RecordWarning("No LiningProfile detected for object " + wireSegment.BHoM_Guid);
            }

            if (elementSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("ElementSolidArea is 0. Returning 0 for ElementSolidVolume.");
            }

            if (insulationSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("InsulationSolidArea is 0. Returning 0 for LiningSolidVolume.");
            }

            if (liningSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("LiningSolidArea is 0. Returning 0 for InsulationSolidVolume.");
            }

            return ((length * elementSolidArea) + (length * insulationSolidArea) + (length * liningSolidArea));
        }

        /***************************************************/

        [Description("Queries the solid volume of a WireSegment by multiplying the section profile's solid area by the element's length. Note this element contains a composite section and this query method returns a single summed value. If you want precise values per section profile, please use CompositeSolidVolumes.")]
        [Input("wireSegment", "The WireSegment to query solid volume.")]
        [Output("solidVolume", "Combined SolidVolume of the Element's SectionProfiles.")]

        public static double SolidVolume(this CableTray cableTray)
        {
            double length = cableTray.Length();
            double elementSolidArea = cableTray.SectionProperty.ElementSolidArea;

            if (length <= 0)
            {
                Engine.Base.Compute.RecordError("Cannot query SolidVolume from zero length members.");
                return double.NaN;
            }

            if (cableTray.SectionProperty.SectionProfile.ElementProfile == null)
            {
                Engine.Base.Compute.RecordWarning("No ElementProfile detected for object " + cableTray.BHoM_Guid);
            }

            if (elementSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("ElementSolidArea is 0. Returning 0 for ElementSolidVolume.");
            }

            return length * elementSolidArea;
        }

        /***************************************************/
    }
}



