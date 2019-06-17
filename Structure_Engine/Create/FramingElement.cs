/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
//using BH.oM.Structure.FramingProperties;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BHPE = BH.oM.Physical.Elements;
using BHPP = BH.oM.Physical.FramingProperties;
using BH.oM.Geometry.ShapeProfiles;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a physical IFramingElement from a bar. The framing element will be assigned a ConstantFramingProperty based on the sectionproeprty of the bar and have a type based on the structural usage")]
        [Input("bar", "The bar to use as the base for the framing element.")]
        [Input("structuralUsage", "Used to determine which type of framing element that should be constructed")]
        public static BHPE.IFramingElement FramingElement(Bar bar, StructuralUsage1D structuralUsage = StructuralUsage1D.Beam)
        {
            ISectionProperty prop = bar.SectionProperty;
            BHPP.ConstantFramingProperty framingProp = null;
            if (prop == null)
                Reflection.Compute.RecordWarning("The bar does not contain a sectionProperty. Can not extract profile or material");
            else
                framingProp = Create.ConstantFramingProperty(bar.SectionProperty, bar.OrientationAngle);

            Line location = bar.Centreline();
            string name = bar.Name ?? "";
            switch (structuralUsage)
            {
                case StructuralUsage1D.Column:
                    return Physical.Create.Column(location, framingProp, name);
                case StructuralUsage1D.Brace:
                    return Physical.Create.Bracing(location, framingProp, name);
                case StructuralUsage1D.Cable:
                    return Physical.Create.Cable(location, framingProp, name);
                case StructuralUsage1D.Pile:
                    return Physical.Create.Pile(location, framingProp, name);
                default:
                case StructuralUsage1D.Undefined:
                case StructuralUsage1D.Beam:
                    return Physical.Create.Beam(location, framingProp, name);

            }
        }


        /***************************************************/
        /**** Public Methods -Deprecated                ****/
        /***************************************************/

        [Deprecated("2.3", "Deprecated by method accepting a ICurve", null, "FramingElement")]
        public static FramingElement FramingElement(Line locationCurve, IFramingElementProperty property, StructuralUsage1D structuralUsage = StructuralUsage1D.Beam, string name = "")
        {
            return new FramingElement { LocationCurve = locationCurve, Property = property, StructuralUsage = structuralUsage, Name = name };
        }

        /***************************************************/
        [Description("Creates a framing elements from a curve and a property")]
        [Input("locationCurve", "The centreline curve of the framing element")]
        [Input("property", "The property to be used on the framing element. The most simple version for this would be the ConstantFramingElementProperty, used for beams with constant section and rotation along the full element")]
        [Input("structuralUsage", "Describes the usage of the element.")]
        [Input("name", "The name of the element.")]
        [Deprecated("2.3", "Methods replaced with methods targeting BH.oM.Physical.Elements.IFramingElement")]
        public static FramingElement FramingElement(ICurve locationCurve, IFramingElementProperty property, StructuralUsage1D structuralUsage = StructuralUsage1D.Beam, string name = "")
        {
            return new FramingElement { LocationCurve = locationCurve, Property = property, StructuralUsage = structuralUsage, Name = name };
        }

        /***************************************************/
    }
}
