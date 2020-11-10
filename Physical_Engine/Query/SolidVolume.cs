/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Elements;

using BH.Engine.Base;
using BH.oM.Physical.FramingProperties;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Base;
using BH.oM.Quantities.Attributes;
using BH.oM.Physical.Fragments;
using BH.Engine.Reflection;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets an IFramingElement's solid volume from the average area and length")]
        [Input("framingElement", "the IFramingElement to get the volume from")]
        [Output("volume", "The IFramingElement's solid material volume.", typeof(Volume))]
        public static double SolidVolume(this IFramingElement framingElement)
        {
            if (framingElement.Property == null)
            {
                Engine.Reflection.Compute.RecordError("The IFramingElement Solid Volume could not be calculated as no property has been assigned. Returning zero volume.");
                return 0;
            }
            return framingElement.Location.Length() * IAverageProfileArea(framingElement.Property);
        }

        /***************************************************/

        [Description("Returns an ISurface's solid volume based on thickness and area." + 
                     "ISurfaces with offsets other than Centre are not fully supported.")]
        [Input("surface", "the ISurface to get the volume from")]
        [Output("volume", "The ISurface's solid material volume.", typeof(Volume))]
        public static double SolidVolume(this oM.Physical.Elements.ISurface surface)
        {
            if (surface.Construction == null)
            {
                Engine.Reflection.Compute.RecordError("The ISurface Solid Volume could not be calculated as no IConstruction has been assigned. Returning zero volume.");
                return 0;
            }

            if (surface.Offset != Offset.Centre && !surface.Location.IIsPlanar())
                Reflection.Compute.RecordWarning("The SolidVolume for non-Planar ISurfaces with offsets other than Centre is approxamite at best");

            double area = surface.Location.IArea();
            area -= surface.Openings.Sum(x => x.Location.IArea());
            
            return area * surface.Construction.IThickness();
        }

        /***************************************************/

        [Description("Return the total volume of BulkSolids")]
        [Input("bulkSolids", "Solid geometric elements that have a material composition")]
        [Output("volume", "The combined volume of BulkSolids", typeof(Volume))]
        public static double SolidVolume(this BulkSolids bulkSolids)
        {
            double solidVolume = bulkSolids.Geometry.Select(x => BH.Engine.Geometry.Query.IVolume(x)).Sum();

            if (solidVolume <= 0)
            {
                solidVolume = bulkSolids.FindFragment<VolumeFragment>(typeof(VolumeFragment)).Volume;

                Reflection.Compute.RecordNote("The SolidVolume of the BulkSolids is not based on Volume calculations, but has been set by VolumeFragment");
                return solidVolume;
            }

            return solidVolume;
        }

        /***************************************************/
    }
}
