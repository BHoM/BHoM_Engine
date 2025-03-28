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
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Analytical.Elements;
using BH.oM.Physical.FramingProperties;
 
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        [Description("Returns external edges from a collection of 2D elements.")]
        [Input("elements", "Elements to get external edges from.")]
        [Output("externalEdges", "External edges of elements.")]
        public static List<IElement1D> ExternalEdges(this IEnumerable<IElement2D> elements)
        {
            if (elements == null)
                return null;

            List<IElement1D> allEdges = new List<IElement1D>();
            foreach (IElement2D elem in elements)
            {
                allEdges.AddRange(elem.ExternalElementCurves());
            }

            List<IElement1D> result = new List<IElement1D>();

            //Get only external curves from all element specific outline curves
            foreach (ICurve crv in allEdges)
            {
                List<IElement1D> adjElems = crv.AdjacentElements(allEdges);
                if (adjElems.Count == 1)
                    result.Add(crv);
            }

            return result;
        }

        /***************************************************/

    }
}




