/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Dimensional;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Analytical.Elements;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Geometry;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Gets the edge elements from an IOpening defining the boundary of the element. Method required for all IElement2Ds. \n" +
             "For an IOpening this will return a list of its Edges.")]
        [Input("opening", "The IOpening to get outline elements from.")]
        [Output("elements", "Outline elements of the IOpening, i.e. the Edges of the Opening.")]
        public static List<IElement1D> OutlineElements1D<TEdge>(this IOpening<TEdge> opening)
            where TEdge : IEdge
        {
            return opening.Edges.Cast<IElement1D>().ToList();
        }

        /***************************************************/

        [Description("Gets the edge elements from an IPanel defining the boundary of the element. Method required for all IElement2Ds. \n" +
                     "For an IPanel this will return a list of its ExternalEdges.")]
        [Input("panel", "The IPanel to get outline elements from.")]
        [Output("elements", "Outline elements of the IPanel, i.e. the ExternalEdges of the Panel.")]
        public static List<IElement1D> OutlineElements1D<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel)
            where TEdge : IEdge
            where TOpening : IOpening<TEdge>
        {
            return panel.ExternalEdges.Cast<IElement1D>().ToList();
        }

        /***************************************************/

        [Description("Gets the boundary from an IRegion defining the boundary of the element as the subparts of the perimiter curve. Method required for all IElement2Ds.")]
        [Input("region", "The IRegion to get outline elements from.")]
        [Output("elements", "Outline elements of the IRegion, i.e. the subparts of the Perimiter curve.")]
        public static List<IElement1D> OutlineElements1D(this IRegion region)
        {
            if(region == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the outline 1D elements of a null region.");
                return new List<IElement1D>();
            }

            return region.Perimeter.ISubParts().Cast<IElement1D>().ToList();
        }

        /***************************************************/
    }
}




