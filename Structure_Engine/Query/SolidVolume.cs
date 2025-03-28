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


using BH.oM.Structure.MaterialFragments;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using BH.oM.Structure.Elements;
using System.Linq;
using System;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Spatial;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.Engine.Geometry;
using BH.oM.Analytical;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a Bar's solid volume based on its SectionProperty area and the CentreLine length.")]
        [Input("bar", "The Bar to get the volume from.")]
        [Output("volume", "The Bar solid material volume.", typeof(Volume))]
        public static double SolidVolume(this Bar bar)
        {
            if (bar.IsNull())
                return 0;

            if (bar.SectionProperty == null)
            {
                Engine.Base.Compute.RecordError("The Bars Solid Volume could not be calculated as no section property has been assigned. Returning zero volume.");
                return 0;
            }
            return bar.SectionProperty.ISolidVolume(bar.Length());
        }

        /***************************************************/

        [Description("Returns a IAreaElement's solid volume as the area of the element times the average thickness of its SurfaceProperty. The average thickness is evaluated as if it was applied to an infinite plane.")]
        [Input("areaElement", "The IAreaElement to get the volume from.")]
        [Output("volume", "The IAreaElement solid material volume.", typeof(Volume))]
        public static double SolidVolume(this IAreaElement areaElement)
        {
            if (areaElement.IIsNull())
                return 0;

            if (areaElement.Property == null)
            {
                Engine.Base.Compute.RecordError("The IAreaElements Solid Volume could not be calculated as no surface property has been assigned. Returning zero volume.");
                return 0;
            }

            double area = areaElement.IArea();
            double thickness = areaElement.Property.IVolumePerArea();

            return area * thickness;
        }

        /***************************************************/

        [Description("Returns a Pile's solid volume based on its Section area and the CentreLine length.")]
        [Input("pile", "The Pile to get the volume from.")]
        [Output("volume", "The Pile solid material volume.", typeof(Volume))]
        public static double SolidVolume(this Pile pile)
        {
            if (pile.IsNull())
                return 0;

            if (pile.Section.IsNull())
                return 0;

            return pile.Section.ISolidVolume(pile.Length());
        }

        [Description("Returns a PileFoundation's solid volume based on the PileCap and Piles volumes.")]
        [Input("pileFoundation", "The PileFoundation to get the volume from.")]
        [Output("volume", "The PileFoundation solid material volume.", typeof(Volume))]
        public static double SolidVolume(this PileFoundation pileFoundation)
        {
            return pileFoundation.IsNull() ? 0 : pileFoundation.PileCap.SolidVolume() + pileFoundation.Piles.Select(x => x.SolidVolume()).Sum();
        }

        /***************************************************/

        [Description("Returns a Stem's solid volume based on the area of the element and its top and bottom thickness.")]
        [Input("stem", "The Stem to get the volume from.")]
        [Output("volume", "The Stem solid material volume.", typeof(Volume))]
        public static double SolidVolume(this Stem stem)
        {
            if (Math.Abs(stem.ThicknessTop - stem.ThicknessBottom) < Tolerance.Distance || stem.OutlineCurve().IsRectangular())
                return stem.IsNull() ? 0 : Engine.Geometry.Query.IArea(stem.Perimeter) * (stem.ThicknessBottom + stem.ThicknessTop) / 2;
            else
            {
                Base.Compute.RecordError("Solid volume cannot be calculated for non-rectangular non-uniform Stems");
                return 0;
            }
        }

        /***************************************************/

        [Description("Returns a RetainingWall's solid volume based on the Stem and Footing volumes.")]
        [Input("retainingWall", "The RetainingWall to get the volume from.")]
        [Output("volume", "The RetainingWall solid material volume.", typeof(Volume))]
        public static double SolidVolume(this RetainingWall retainingWall)
        {
            return retainingWall.IsValid() ? SolidVolume(retainingWall.Footing) + SolidVolume(retainingWall.Stem) : 0;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double ISolidVolume(this ISectionProperty sectionProperty, double length)
        {
            return SolidVolume(sectionProperty as dynamic, length);
        }

        /***************************************************/

        private static double SolidVolume(ISectionProperty sectionProperty, double length)
        {
            return sectionProperty.Area * length;
        }

        /***************************************************/

        private static double SolidVolume(IGeometricalSection sectionProperty, double length)
        {
            //If contains tapered profile, that is used
            double area;
            if (sectionProperty.SectionProfile is TaperedProfile)
                area = (sectionProperty.SectionProfile as TaperedProfile).Area();
            else
                area = sectionProperty.Area;

            return area * length;
        }

        /***************************************************/

        private static double SolidVolume(CellularSection sectionProperty, double length)
        {
            //If contains tapered profile, that is used
            double solidArea = sectionProperty.SolidProfile.Area();
            double openingCount = Math.Floor((length - sectionProperty.Opening.WidthWebPost) / sectionProperty.Opening.Spacing);

            return solidArea * length - openingCount * sectionProperty.Opening.IOpeningArea() * sectionProperty.SolidProfile.WebThickness;
        }

        /***************************************************/

        private static double SolidVolume(CompositeSection sectionProperty, double length)
        {
            //TODO: Handle embedment etc..
            return sectionProperty.ConcreteSection.ISolidVolume(length) + sectionProperty.SteelSection.ISolidVolume(length);
        }

        /***************************************************/
    }
}
