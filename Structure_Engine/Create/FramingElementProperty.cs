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

using BH.oM.Structure.Properties.Framing;
using BH.oM.Structure.Properties.Section;
using BH.oM.Geometry;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Constructs the simplest type of FramingELementProperty, with constant section proeprty along the element as well as a constant orientation angle along the element")]
        [Input("sectionProperty", "The section property used by the element. Constant along the whole element")]
        [Input("orientationAngle", "orientation angle of the element. Constant along the whole element")]
        [Input("name", "Name of the ConstantFramingElementProeprty. If no name is provided, the name of the provided SectionProeprty will be used")]
        public static ConstantFramingElementProperty ConstantFramingElementProperty(ISectionProperty sectionProperty, double orientationAngle, string name = "")
        {
            //If no name is provided, use the name of the section proeprty
            if (string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(sectionProperty.Name))
                name = sectionProperty.Name;

            return new ConstantFramingElementProperty { SectionProperty = sectionProperty, OrientationAngle = orientationAngle, Name = name };
        }

        /***************************************************/
    }
}

