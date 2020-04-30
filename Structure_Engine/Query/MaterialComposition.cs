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


using BH.oM.Structure.MaterialFragments;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System;
using BH.oM.Physical.Materials;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a Bar's homogeneous MaterialComposition.")]
        [Input("bar", "The Bar to material from")]
        [Output("materialComposition", "The kind of matter the Bar is composed of.")]
        public static MaterialComposition MaterialComposition(this Bar bar)
        {
            if (bar.SectionProperty == null || bar.SectionProperty.Material == null)
            {
                Engine.Reflection.Compute.RecordError("The Bars MaterialComposition could not be calculated as no Material has been assigned.");
                return null;
            }
            Material mat = Physical.Create.Material(bar.SectionProperty.Material);
            return (MaterialComposition)mat;
        }

        /***************************************************/

        [Description("Returns a areaElement's homogeneous MaterialComposition.")]
        [Input("areaElement", "The areaElement to material from")]
        [Output("materialComposition", "The kind of matter the areaElement is composed of.")]
        public static MaterialComposition MaterialComposition(this IAreaElement areaElement)
        {   
                if (areaElement.Property == null || areaElement.Property.Material == null)
            {
                Engine.Reflection.Compute.RecordError("The areaElements MaterialComposition could not be calculated as no Material has been assigned.");
                return null;
            }
            Material mat = Physical.Create.Material(areaElement.Property.Material);
            return (MaterialComposition)mat;
        }

        /***************************************************/
        
    }
}
