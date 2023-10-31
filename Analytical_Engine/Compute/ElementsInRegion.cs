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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Analytical.Elements;
using BH.oM.Dimensional;

namespace BH.Engine.Analytical
{
    public static partial class Compute
    {
        [Description("Gets the elements that lie within the provided space.")]
        [Input("space", "An Environment Space object defining a perimeter to build a 3D volume from and check if the volume contains the provided point.")]
        [Input("spaceHeight", "The height of the space.", typeof(BH.oM.Quantities.Attributes.Length))]
        [Input("elements", "The elements being checked to see if they are contained within the bounds of the 3D volume.")]
        [Input("acceptOnEdges", "Decide whether to allow elements which sit on the edge of the space, default false.")]
        [Input("acceptPartial", "Decide whether to include elements only partially within the space, default false.")]
        [Output("elements", "The elements from the provided elements that are within the space.")]
        public static List<IElement> ElementsInRegion(this IRegion region, double regionHeight, List<IElement> elements,  bool acceptOnEdges = false, bool acceptPartial = false)
        {
            if (region == null)
                return new List<IElement>();

            List<bool> isContaining = region.IsContaining(regionHeight, elements, acceptOnEdges, acceptPartial);

            return elements
                .Zip(isContaining, (elem, inSpace) => new {
                    elem,
                    inSpace,
                })
                .Where(item => item.inSpace)
                .Select(item => item.elem)
                .ToList();
        }
    }
}
