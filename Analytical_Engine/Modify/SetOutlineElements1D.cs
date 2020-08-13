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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Analytical.Elements;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.Engine.Base;


namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Sets the Outline Element1Ds of an opening, i.e. the Edges of an Opening. Method required for all IElement2Ds.")]
        [Input("opening", "The Opening to update the Edges of.")]
        [Input("edges", "A list of IElement1Ds which all should be of a type of Edge accepted by the Opening or Geometrical ICurve. \n" +
                "ICurve will default the outlines properties.")]
        [Output("opening", "The opening with updated Edges.")]
        public static IOpening<TEdge> SetOutlineElements1D<TEdge>(this IOpening<TEdge> opening, IEnumerable<IElement1D> edges)
                where TEdge : class, IEdge, new()
        {
            IOpening<TEdge> o = opening.GetShallowClone(true) as IOpening<TEdge>;
            o.Edges = edges.Select(x => x is ICurve ? new TEdge() { Curve = (x as ICurve) } : x as TEdge).ToList();
            return o;
        }

        /***************************************************/

        [Description("Sets the outline Element1Ds of a IPanel, i.e. the ExternalEdges of a IPanel. Method required for all IElement2Ds.")]
        [Input("panel", "The IPanel to update the ExternalEdges of.")]
        [Input("edges", "A list of IElement1Ds which all should be of a type of Edge accepted by the IPanel or Geometrical ICurve. \n" +
                        "ICurve will default the outlines properties.")]
        [Output("panel", "The IPanel with updated ExternalEdges.")]
        public static IPanel<TEdge, TOpening> SetOutlineElements1D<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel, IEnumerable<IElement1D> edges)
            where TEdge : class, IEdge, new()
            where TOpening : IOpening<TEdge>
        {
            IPanel<TEdge, TOpening> pp = panel.GetShallowClone(true) as IPanel<TEdge, TOpening>;
            pp.ExternalEdges = edges.Select(x => x is ICurve ? new TEdge() { Curve = (x as ICurve) } : x as TEdge).ToList();
            return pp;
        }

        /***************************************************/
    }
}

