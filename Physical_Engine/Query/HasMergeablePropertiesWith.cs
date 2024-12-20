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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Physical.Elements;
using BH.oM.Diffing;
using BH.Engine.Geometry;
using BH.oM.Base;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Evaluates if the two IFramingElements' non-geometrical data is equal to the point that they could be merged into one object.")]
        [Input("element", "An IFramingElement to compare the properties of with an other IFramingElement.")]
        [Input("other", "The IFramingElement to compare with the other IFramingElement.")]
        [Output("equal", "True if the IFramingElements' non-geometrical property is equal to the point that they could be merged into one object.")]
        public static bool HasMergeablePropertiesWith(this IFramingElement element, IFramingElement other)
        {
            if (element == null || other == null)
                return false; //If either is null, then it can probably can't have its properties merged

            if (element.GetType() != other.GetType())
                return false;

            if (element.Location == null || element.Location.ILength() < oM.Geometry.Tolerance.Distance || other.Location == null || other.Location.ILength() < oM.Geometry.Tolerance.Distance)
                return false;

            if (!element.Location.IIsLinear() || !other.Location.IIsLinear())
            {
                Engine.Base.Compute.RecordWarning("No merge comparison avalible for non-linear IFramingElements.");
                return false;
            }

            int parallel = element.Location.IStartDir().IsParallel(other.Location.IStartDir());
            if (parallel != 1)
                return false;

            if (element.Normal().Angle(other.Normal()) > BH.oM.Geometry.Tolerance.Angle)
                return false;

            if (element.Property == other.Property)
                return true;

            if (element.Property == null || other.Property == null)
                return false;

            if (element.Property.Name != other.Property.Name)
                return false;

            return !Diffing.Query.DifferentProperties(element.Property, other.Property, new ComparisonConfig())?.Any() ?? true;
        }


        /***************************************************/

        [Description("Evaluates if the two ISurface non-geometrical data is equal to the point that they could be merged into one object.")]
        [Input("element", "An ISurface to compare the properties of with an other ISurface.")]
        [Input("other", "The ISurface to compare with the other ISurface.")]
        [Output("equal", "True if the ISurfaces' non-geometrical property is equal to the point that they could be merged into one object.")]
        public static bool HasMergeablePropertiesWith(this ISurface element, ISurface other)
        {
            if (element == null || other == null)
                return false; //If either is null, then it can probably can't have its properties merged

            if (element.GetType() != other.GetType())
                return false;

            if (element.Offset != other.Offset)
                return false;

            if (element.Construction == other.Construction)
                return true;

            if (element.Construction == null || other.Construction == null)
                return false;

            if (element.Construction.Name != other.Construction.Name)
                return false;

            return !Diffing.Query.DifferentProperties(element.Construction, other.Construction, new ComparisonConfig())?.Any() ?? true;
        }

        /***************************************************/

        [Description("Evaluates if the two IOpenings' non-geometrical data is equal to the point that they could be merged into one object.")]
        [Input("element", "An IOpening to compare the properties of with an other IOpening.")]
        [Input("other", "The IOpening to compare with the other IOpening.")]
        [Output("equal", "True if the IOpenings' non-geometrical property is equal to the point that they could be merged into one object.")]
        public static bool HasMergeablePropertiesWith(this IOpening element, IOpening other)
        {
            if (element == null || other == null)
                return false; //If either is null, then it can probably can't have its properties merged

            return element.GetType() == other.GetType();
        }

        /***************************************************/

        [Description("Evaluates if the two PadFoundation non-geometrical data is equal to the point that they could be merged into one object.")]
        [Input("element", "An PadFoundation to compare the properties of with an other PadFoundation.")]
        [Input("other", "The PadFoundation to compare with the other PadFoundation.")]
        [Output("equal", "True if the PadFoundation non-geometrical property is equal to the point that they could be merged into one object.")]
        public static bool HasMergeablePropertiesWith(this PadFoundation element, PadFoundation other)
        {
            if (element.IsNull() || other.IsNull())
                return false; //If either is null, then it can probably can't have its properties merged

            if (element.Construction == other.Construction)
                return true;

            if (element.Construction.IsNull() || other.Construction.IsNull())
                return false;

            if (element.Construction.Name != other.Construction.Name)
                return false;

            return !Diffing.Query.DifferentProperties(element.Construction, other.Construction, new ComparisonConfig())?.Any() ?? true;
        }

        /***************************************************/

    }
}





