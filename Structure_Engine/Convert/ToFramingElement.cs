/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Structure.Elements;

using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/
        [Deprecated("2.3", "Replaced be Create(Bar,StructuralUsage) in Structure_Engine")]
        public static FramingElement ToFramingElement(this Bar bar, StructuralUsage1D usage = StructuralUsage1D.Beam)
        {
            return new FramingElement
            {
                LocationCurve = bar.Centreline(),
                Name = bar.Name,
                Property = Create.ConstantFramingElementProperty(bar.SectionProperty, bar.OrientationAngle, bar.SectionProperty.Name),
                StructuralUsage = usage
            };
        }

        /***************************************************/

    }
}
