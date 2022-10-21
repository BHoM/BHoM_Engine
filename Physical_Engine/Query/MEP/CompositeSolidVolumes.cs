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

using System.ComponentModel;
using BH.Engine.Spatial;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Elements;

namespace BH.Engine.Physical.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Queries the solid volume of a Duct by multiplying the section profile's solid area by the element's length.")]
        [Input("duct", "The Duct to query solid volume.")]
        [MultiOutput(0, "elementSolidVolume", "SolidVolume of the Element itself within a compiled SectionProfile.")]
        [MultiOutput(1, "insulationSolidVolume", "The solid volume of the Duct's exterior insulation.")]
        [MultiOutput(2, "liningSolidVolume", "The solid volume of the Duct's interior lining.")]
        public static Output<double, double, double> CompositeSolidVolumes(this Duct duct)
        {
            double length = duct.Length();
            double elementSolidVolume = duct.SectionProperty.ElementSolidArea * length;
            double insulationSolidVolume = duct.SectionProperty.InsulationSolidArea * length;
            double liningSolidVolume = duct.SectionProperty.LiningSolidArea * length;

            if (duct.SectionProperty == null)
            {
                Engine.Base.Compute.RecordError("No section property defined.");
                return null;
            }

            //Negative LiningThickness Warning
            if (duct.SectionProperty.LiningSolidArea < 0)
            {
                Engine.Base.Compute.RecordWarning("LiningSolidArea is a negative value, and will result in incorrect SolidVolume results. Try adjusting LiningThickness to produce a positive value for SolidArea.");
            }

            //SolidArea = 0 user feedback.
            if (duct.SectionProperty.ElementSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("ElementSolidArea is 0. Returning 0 for ElementSolidVolume.");
            }

            if (duct.SectionProperty.LiningSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("LiningSolidArea is 0. Returning 0 for LiningSolidVolume.");
            }

            if (duct.SectionProperty.InsulationSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("InsulationSolidArea is 0. Returning 0 for InsulationSolidVolume.");
            }

            Output<double, double, double> output = new Output<double, double, double>
            {
                Item1 = elementSolidVolume,
                Item2 = insulationSolidVolume,
                Item3 = liningSolidVolume,
            };
            return output;
        }

        /***************************************************/

        //This may get adjusted per finalised property names and section compositions.//
        [Description("Queries the solid volume of a Pipe by multiplying the section profile's solid area by the element's length.")]
        [Input("pipe", "The Pipe to query solid volume.")]
        [MultiOutput(0, "elementSolidVolume", "SolidVolume of the Element itself within a compiled SectionProfile.")]
        [MultiOutput(1, "insulationSolidVolume", "The solid volume of the Pipe's exterior insulation.")]
        [MultiOutput(2, "liningSolidVolume", "The solid volume of the Pipe's interior lining.")]
        public static Output<double, double, double> CompositeSolidVolumes(this Pipe pipe)
        {
            double length = pipe.Length();
            double elementSolidVolume = pipe.SectionProperty.ElementSolidArea * length;
            double insulationSolidVolume = pipe.SectionProperty.InsulationSolidArea * length;
            double liningSolidVolume = pipe.SectionProperty.LiningSolidArea * length;

            if (pipe.SectionProperty == null)
            {
                Engine.Base.Compute.RecordError("No section property defined.");
                return null;
            }

            //Negative LiningThickness Warning
            if (pipe.SectionProperty.LiningSolidArea < 0)
            {
                Engine.Base.Compute.RecordWarning("LiningSolidArea is a negative value, and will result in incorrect SolidVolume results. Try adjusting LiningThickness to produce a positive value for SolidArea.");
            }

            //SolidArea = 0 user feedback.
            if (pipe.SectionProperty.ElementSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("ElementSolidArea is 0. Returning 0 for ElementSolidVolume.");
            }

            if (pipe.SectionProperty.LiningSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("LiningSolidArea is 0. Returning 0 for LiningSolidVolume.");
            }

            if (pipe.SectionProperty.InsulationSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("InsulationSolidArea is 0. Returning 0 for InsulationSolidVolume.");
            }

            Output<double, double, double> output = new Output<double, double, double>
            {
                Item1 = elementSolidVolume,
                Item2 = insulationSolidVolume,
                Item3 = liningSolidVolume,
            };
            return output;
        }
        /***************************************************/

        //This method may get adjusted per finalised property names and section compositions.//
        [Description("Queries the solid volume of a Wire by multiplying the section profile's solid area by the element's length.")]
        [Input("wire", "The Wire to query solid volume.")]
        [MultiOutput(0, "elementSolidVolume", "SolidVolume of the Element itself within a compiled SectionProfile.")]
        [MultiOutput(1, "insulationSolidVolume", "The solid volume of the Wire's exterior insulation.")]
        [MultiOutput(2, "liningSolidVolume", "The solid volume of the Wire's interior lining.")]
        public static Output<double, double, double> CompositeSolidVolumes(this WireSegment wire)
        {
            double length = wire.Length();
            double elementSolidVolume = wire.SectionProperty.ElementSolidArea * length;
            double insulationSolidVolume = wire.SectionProperty.InsulationSolidArea * length;
            double liningSolidVolume = wire.SectionProperty.LiningSolidArea * length;

            if (wire.SectionProperty == null)
            {
                Engine.Base.Compute.RecordError("No section property defined.");
                return null;
            }

            //Negative LiningThickness Warning
            if (wire.SectionProperty.LiningSolidArea < 0)
            {
                Engine.Base.Compute.RecordWarning("LiningSolidArea is a negative value, and will result in incorrect SolidVolume results. Try adjusting LiningThickness to produce a positive value for SolidArea.");
            }

            //SolidArea = 0 user feedback.
            if (wire.SectionProperty.ElementSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("ElementSolidArea is 0. Returning 0 for ElementSolidVolume.");
            }

            if (wire.SectionProperty.LiningSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("LiningSolidArea is 0. Returning 0 for LiningSolidVolume.");
            }

            if (wire.SectionProperty.InsulationSolidArea <= 0)
            {
                Engine.Base.Compute.RecordNote("InsulationSolidArea is 0. Returning 0 for InsulationSolidVolume.");
            }

            Output<double, double, double> output = new Output<double, double, double>
            {
                Item1 = elementSolidVolume,
                Item2 = insulationSolidVolume,
                Item3 = liningSolidVolume,
            };
            return output;
        }
        /***************************************************/
    }
}


