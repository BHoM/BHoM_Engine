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
using BH.oM.Reflection;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        /***************************************************/

        [Description("Returns elements adjacent to a provided element from a collection of elements")]
        [Input("elem", "Element to find adjacencies at")]
        [Input("refElems", "Elements to use to find element adjacencies")]
        [Output("adjElems", "Adjacents elements to the provided element")]
        public static List<IElement2D> AdjacentElements(this IElement2D elem, List<IElement2D> refElems)
        {
            List<IElement2D> adjElems = new List<IElement2D>();

            PolyCurve outline = elem.OutlineCurve();

            foreach (IElement2D refElem in refElems)
            {
                PolyCurve refOutline = refElem.OutlineCurve();
                BH.oM.Reflection.Output<Point, Point> results = outline.CurveProximity(refOutline);
                double distance = results.Item1.Distance(results.Item2);
                if (distance < Tolerance.Distance)
                    adjElems.Add(refElem);
            }

            return adjElems;
        }
    }
}

