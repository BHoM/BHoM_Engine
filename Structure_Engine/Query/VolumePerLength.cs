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

using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using BH.oM.Spatial.ShapeProfiles;
using BH.Engine.Spatial;
using BH.oM.Spatial.ShapeProfiles.CellularOpenings;
using System;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates the volume per length for the section the volume per metre of the concrete section + the volume per metre of the steel section.")]
        [Input("section", "The CompositeSection to calculate the volume per length for.")]
        [Output("volumePerLength", "The volume per length for the section.", typeof(MassPerUnitLength))]
        public static double VolumePerLength(this CellularSection section)
        {
            if(section.IsNull())
                return 0.0;

            double solidArea = section.SolidProfile.Area();
            double areaReduction = section.Opening.IAreaReduction();

            return solidArea - areaReduction * section.BaseProfile.WebThickness;
        }

        [Description("Calculates the volume per length for the section the volume per metre of the concrete section + the volume per metre of the steel section.")]
        [Input("section", "The CompositeSection to calculate the volume per length for.")]
        [Output("volumePerLength", "The volume per length for the section.", typeof(MassPerUnitLength))]
        public static double VolumePerLength(this CompositeSection section)
        {
            //TODO: Handle embedment etc..
            return section.IsNull() ? 0 : section.ConcreteSection.VolumePerLength() + section.SteelSection.VolumePerLength();
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Calculates the volume per length for the section, generally as its area mulitplied by the density. General dispatch method that calls the correct method based on type.")]
        [Input("section", "The SectionProperty to calculate the volume per length for.")]
        [Output("volumePerLength", "The volume per length for the section.", typeof(MassPerUnitLength))]
        public static double IVolumePerLength(this ISectionProperty section)
        {
            return section.IsNull() ? 0 : VolumePerLength(section as dynamic);
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        [Description("Calculates the volume per length for the section as its area mulitplied by the density.")]
        [Input("section", "The SectionProperty to calculate the volume per length for.")]
        [Output("volumePerLength", "The volume per length for the section.", typeof(MassPerUnitLength))]
        private static double VolumePerLength(this ISectionProperty section)
        {
            //General case, without special implementation, relying on Area.
            return section.IsNull() ? 0 : section.Area;
        }

        /***************************************************/

        [Description("Calculates the volume per length for the section as its area mulitplied by the density.")]
        [Input("section", "The SectionProperty to calculate the volume per area for.")]
        [Output("volumePerLength", "The volume per length for the section.", typeof(MassPerUnitLength))]
        private static double VolumePerLength(this IGeometricalSection section)
        {
            //General case, without special implementation, relying on Area.

            //If contains tapered profile, that is used
            if (section.SectionProfile is TaperedProfile)
                return (section.SectionProfile as TaperedProfile).Area();

            return section.IsNull() ? 0 : section.Area;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        /***************************************************/

        private static double IAreaReduction(this ICellularOpening opening)
        {
            if (Math.Abs(opening.WidthWebPost - opening.LengthEndPost) > Tolerance.Distance)
                BH.Engine.Base.Compute.RecordNote("Volume for cellular beam is computed as if the end post length is the same as the same as intermidiate web post.");

            return AreaReduction(opening as dynamic);
        }

        /***************************************************/

        private static double AreaReduction(this HexagonalOpening opening)
        {
            double areaOpening = opening.Height * (opening.Width + opening.WidthWebPost) / 2;
            return areaOpening / opening.Spacing;
        }

        /***************************************************/

        private static double AreaReduction(this CircularOpening opening)
        {
            double r = opening.Diameter / 2;
            double areaOpening = r * r * Math.PI;
            return areaOpening / opening.Spacing;
        }

        /***************************************************/

        private static double AreaReduction(this SinusoidalOpening opening)
        {
            double areaOpening = opening.Height * (opening.SinusoidalLength + opening.WidthWebPost);
            return areaOpening / opening.Spacing;
        }

        /***************************************************/


    }
}




