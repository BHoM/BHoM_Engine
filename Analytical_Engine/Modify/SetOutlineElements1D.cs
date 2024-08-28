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

using System;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Analytical.Elements;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Geometry;
using BH.Engine.Base;


namespace BH.Engine.Analytical
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
                where TEdge : IEdge
        {
            IOpening<TEdge> o = opening.ShallowClone();

            o.Edges = ConvertToEdges<TEdge>(edges);
            return o;
        }

        /***************************************************/

        [Description("Sets the outline Element1Ds of a IPanel, i.e. the ExternalEdges of a IPanel. Method required for all IElement2Ds.")]
        [Input("panel", "The IPanel to update the ExternalEdges of.")]
        [Input("edges", "A list of IElement1Ds which all should be of a type of Edge accepted by the IPanel or Geometrical ICurve. \n" +
                        "ICurve will default the outlines properties.")]
        [Output("panel", "The IPanel with updated ExternalEdges.")]
        public static IPanel<TEdge, TOpening> SetOutlineElements1D<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel, IEnumerable<IElement1D> edges)
            where TEdge : IEdge
            where TOpening : IOpening<TEdge>
        {
            IPanel<TEdge, TOpening> pp = panel.ShallowClone();

            pp.ExternalEdges = ConvertToEdges<TEdge>(edges);
            return pp;
        }

        /***************************************************/

        [PreviousVersion("7.3", "BH.Engine.Structure.Modify.SetOutlineElements1D(BH.oM.Structure.Elements.PadFoundation, System.Collections.Generic.IEnumerable<BH.oM.Dimensional.IElement1D>"))]
        [Description("Sets the Outline Element1Ds of an IRegion, i.e. the perimiter. Method required for all IElement2Ds.")]
        [Input("region", "The IRegion to update the Perimeter of.")]
        [Input("outlineElements", "A list of IElement1Ds which all should be Geometrical ICurves.")]
        [Output("region", "The region with updated perimiter.")]
        public static IRegion SetOutlineElements1D(this IRegion region, IEnumerable<IElement1D> outlineElements)
        {
            if(region == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the outline 1D elements of a null region.");
                return null;
            }

            IRegion r = region.ShallowClone();

            IEnumerable<ICurve> joinedCurves = outlineElements.Cast<ICurve>();
            if (outlineElements.Count() != 1)
                joinedCurves = Engine.Geometry.Compute.IJoin(outlineElements.Cast<ICurve>().ToList());

            if (joinedCurves.Count() == 1)
            {
                if (!joinedCurves.First().IIsClosed())
                    Engine.Base.Compute.RecordWarning("The outline elements assigned to the region do not form a closed loop.");

                r.Perimeter = joinedCurves.First();
            }
            else
            {
                Engine.Base.Compute.RecordWarning("The outline elements assigned to the region are disjointed.");
                r.Perimeter = new PolyCurve { Curves = outlineElements.Cast<ICurve>().ToList() };
            }

            return r;
        }

        /***************************************************/
        /****               Private Methods             ****/
        /***************************************************/

        [Description("Takes a list of IElement1D and returns a TEdge for each element. If the IElement1D is a curve a new TEdge is created and assigned the curve. If not, the IElement1D is cast to the TEdge.")]
        private static List<TEdge> ConvertToEdges<TEdge>(IEnumerable<IElement1D> element1ds)
            where TEdge : IEdge
        {
            List<TEdge> edges = new List<TEdge>();
            foreach (IElement1D element1D in element1ds)
            {
                TEdge edge;
                if (element1D is ICurve)
                {
                    //Using reflection as addig `new()` constraint to the method makes it not runnable in the UI
                    edge = Activator.CreateInstance<TEdge>();
                    edge.Curve = element1D as ICurve;
                }
                else
                    edge = (TEdge)element1D;

                edges.Add(edge);
            }
            return edges;
        }

        /***************************************************/
    }
}




