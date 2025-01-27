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

using BH.oM.Geometry;
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Analytical.Elements;
using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
 
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Returns elements adjacent to a provided Element2D from a collection of elements.")]
        [Input("element", "Element to find adjacencies at.")]
        [Input("referenceElements", "Elements to use to find element adjacencies.")]
        [Output("adjacentElements", "Adjacents elements to the provided element.")]
        public static List<IElement2D> AdjacentElements(this IElement2D element, IEnumerable<IElement2D> referenceElements)
        {          
            List<IElement2D> adjacentElements = new List<IElement2D>();

            if (element == null || referenceElements == null)
            {
                Base.Compute.RecordWarning("Can not get adjacencies of a null element.");
                return null;
            }

            PolyCurve outline = element.OutlineCurve();

            foreach (IElement2D refElem in referenceElements)
            {
                PolyCurve refOutline = refElem.OutlineCurve();
                if (refOutline.IIsAdjacent(outline))
                    adjacentElements.Add(refElem);
            }

            return adjacentElements;
        }


        /***************************************************/

        [Description("Returns elements adjacent to a provided Element1D from a collection of elements.")]
        [Input("element", "Element to find adjacencies at.")]
        [Input("referenceElements", "Elements to use to find element adjacencies.")]
        [Output("adjacentElements", "Adjacents elements to the provided element.")]
        public static List<IElement2D> AdjacentElements(this IElement1D element, IEnumerable<IElement2D> referenceElements)
        {
            List<IElement2D> adjacentElements = new List<IElement2D>();

            if (element == null || referenceElements == null)
            {
                Base.Compute.RecordWarning("Can not get adjacencies of a null element.");
                return null;
            }

            PolyCurve outline = element.ElementCurves().IJoin()[0];

            foreach (IElement2D refElem in referenceElements)
            {
                PolyCurve refOutline = refElem.OutlineCurve();
                if (refOutline.IIsAdjacent(outline))
                    adjacentElements.Add(refElem);
            }

            return adjacentElements;
        }


        /***************************************************/

        [Description("Returns elements adjacent to a provided Element1D from a collection of elements.")]
        [Input("element", "Element to find adjacencies at.")]
        [Input("referenceElements", "Elements to use to find element adjacencies.")]
        [Output("adjacentElements", "Adjacent elements to the provided element.")]
        public static List<IElement1D> AdjacentElements(this IElement1D element, IEnumerable<IElement1D> referenceElements)

        {
            if (element == null || referenceElements == null)
            {
                Base.Compute.RecordWarning("Can not get adjacencies of a null element.");
                return null;
            }

            return referenceElements.Where(x => x.IIsAdjacent(element)).ToList();
        }
    }
}






