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
using System.Collections.Generic;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BHPE = BH.oM.Physical.Elements;
using BHPP = BH.oM.Physical.FramingProperties;
using BH.oM.Physical.Reinforcement;
using BH.oM.Spatial.ShapeProfiles;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Creates a physical IFramingElement from a Bar. The framing element will be assigned a ConstantFramingProperty based on the SectionProperty of the Bar and have a type based on the structural usage.")]
        [Input("bar", "The Bar to use as the base for the framing element.")]
        [Input("structuralUsage", "Used to determine which type of framing element that should be constructed.")]
        [Output("framing", "The created physical FramingElement based on the Bar element provided.")]
        public static BHPE.IFramingElement FramingElement(Bar bar, StructuralUsage1D structuralUsage = StructuralUsage1D.Beam)
        {
            if (bar.IsNull())
                return null;
            ISectionProperty prop = bar.SectionProperty;
            BHPP.ConstantFramingProperty framingProp = null;
            if (prop == null)
                Base.Compute.RecordWarning("The bar does not contain a sectionProperty. Can not extract profile or material");
            else
                framingProp = Create.ConstantFramingProperty(bar.SectionProperty, bar.OrientationAngle);
            Line location = bar.Centreline();
            string name = bar.Name ?? "";
            BHPE.IFramingElement framingElement;
            switch (structuralUsage)
            {
                case StructuralUsage1D.Column:
                    framingElement = Physical.Create.Column(location, framingProp, name);
                    break;
                case StructuralUsage1D.Brace:
                    framingElement = Physical.Create.Bracing(location, framingProp, name);
                    break;
                case StructuralUsage1D.Cable:
                    framingElement = Physical.Create.Cable(location, framingProp, name);
                    break;
                case StructuralUsage1D.Pile:
                    framingElement = Physical.Create.Pile(location, framingProp, name);
                    break;
                default:
                case StructuralUsage1D.Undefined:
                case StructuralUsage1D.Beam:
                    framingElement = Physical.Create.Beam(location, framingProp, name);
                    break;
            }

            if (bar.HasReinforcement())
            {
                List<IReinforcingBar> reinforcement = bar.ReinforcingBars();
                if (reinforcement.Count != 0)
                {
                    framingElement.Fragments.Add(new ReinforcementFragment{ReinforcingBars = reinforcement});
                }
            }

            return framingElement;
        }
    /***************************************************/
    }
}