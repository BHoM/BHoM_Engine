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

        [Description("Return the total volume of SolidBulk.")]
        [Input("solidBulk", "Solid geometric elements that have a MaterialComposition.")]
        [Output("volume", "The combined volume of SolidBulk.", typeof(Volume))]
        public static double SolidVolume(this SolidBulk solidBulk)
        {
            if (solidBulk == null || solidBulk.Geometry == null)
            {
                Engine.Reflection.Compute.RecordError("No valid SolidBulk objects have been provided. Returning NaN.");
                return double.NaN;
            }

            double solidVolume = solidBulk.Geometry.Select(x => x.IVolume()).Sum();

            if (solidVolume <= 0)
            {
                Engine.Reflection.Compute.RecordError("The queried volume has been nonpositive. Returning zero instead.");
                return 0;
            }

            return solidVolume;
        }

        /***************************************************/

        [Description("Return the total volume of ExplicitBulk.")]
        [Input("explicitBulk", "Elements containing Volume and MaterialComposition properties.")]
        [Output("volume", "The combined volume of ExplicitBulk.", typeof(Volume))]
        public static double SolidVolume(this ExplicitBulk explicitBulk)
        {
            if (explicitBulk == null)
            {
                Engine.Reflection.Compute.RecordError("No valid ExplicitBulk objects have been provided. Returning NaN.");
                return double.NaN;
            }

            double solidVolume = explicitBulk.Volume;

            if (solidVolume <= 0)
            {
                Engine.Reflection.Compute.RecordError("The queried volume has been nonpositive. Returning zero instead.");
                return 0;
            }

            return solidVolume;
        }

        /***************************************************/

        [Description("Returns an door's solid volume based on thickness and area.")]
        [Input("door", "the door to get the volume from")]
        [Output("volume", "The door's solid material volume.", typeof(Volume))]
        public static double SolidVolume(this oM.Physical.Elements.Door door)
        {
            if (door.Construction == null)
            {
                Engine.Reflection.Compute.RecordError("The Door's Solid Volume could not be calculated as no IConstruction has been assigned. Returning zero volume.");
                return 0;
            }

            double area = door.Location.IArea();

            return area * door.Construction.IThickness();
        }

        /***************************************************/

        [Description("Returns an window's solid volume based on thickness and area.")]
        [Input("window", "the window to get the volume from")]
        [Output("volume", "The window's solid material volume.", typeof(Volume))]
        public static double SolidVolume(this oM.Physical.Elements.Window window)
        {
            if (window.Construction == null)
            {
                Engine.Reflection.Compute.RecordError("The window's Solid Volume could not be calculated as no IConstruction has been assigned. Returning zero volume.");
                return 0;
            }

            double area = window.Location.IArea();

            return area * window.Construction.IThickness();
        }

        /***************************************************/
    }
}
