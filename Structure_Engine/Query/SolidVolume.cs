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


using BH.oM.Structure.MaterialFragments;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System;
using BH.oM.Physical.Materials;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Spatial;

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
            return bar.SectionProperty.Area * bar.Length();
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

    }
}



