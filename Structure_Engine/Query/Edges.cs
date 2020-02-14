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

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extracts the edge curves from the geometric surface of the element.")]
        [Input("surface","The structural surface to extract edges from.")]
        [Output("edges", "The list of curves representing the edges of the underlying surface.")]
        public static List<ICurve> Edges(this Surface surface)
        {
            if (surface.Extents != null)
                return surface.Extents.IExternalEdges();
            else
                return new List<ICurve>();
        }

        /***************************************************/


        [Description("Extracts the edge curves from all of the Openings of a Panel.")]
        [Input("panel", "The Panel to extract opening edges from.")]
        [Output("edges", "The list of curves representing the edges of the openings of the panel.")]
        public static List<ICurve> InternalEdgeCurves(this Panel panel)
        {
            List<ICurve> edges = new List<ICurve>();
            foreach (Opening o in panel.Openings)
            {
                edges.AddRange(o.Edges.Select(e => e.Curve).ToList());
            }
            return edges;
        }

        /***************************************************/

        [Description("Extracts the edge curves from the external edges of a Panel.")]
        [Input("panel", "The Panel to extract external edges from.")]
        [Output("edges", "The list of curves representing the external edges of the panel.")]
        public static List<ICurve> ExternalEdgeCurves(this Panel panel)
        {
            return panel.ExternalEdges.Select(x => x.Curve).ToList();
        }

        /***************************************************/

        [Description("Extracts the edge curves from the external edges as well as all the Openings of a Panel.")]
        [Input("panel", "The Panel to extract all edges from.")]
        [Output("edges", "The list of curves representing the external edges and the edges of the openings of the panel.")]
        public static List<ICurve> AllEdgeCurves(this Panel panel)
        {
            List<ICurve> result = panel.ExternalEdgeCurves();
            result.AddRange(panel.InternalEdgeCurves());
            return result;
        }

        /***************************************************/

        [Description("Extracts the edge curves from all of an Openings.")]
        [Input("opening", "The Opening to extract opening edges from.")]
        [Output("edges", "The list of curves representing the edges of the Opening.")]
        public static List<ICurve> EdgeCurves(this Opening opening)
        {
            return opening.Edges.Select(e => e.Curve).ToList();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Extracts all the edge curves from an AreaElement.")]
        [Input("element", "The element to extract opening edges from.")]
        [Output("edges", "The list of curves representing all internal and external edges of an element.")]
        public static IEnumerable<ICurve> IEdges(this IAreaElement element)
        {
            if (element is Panel)
                return (AllEdgeCurves(element as Panel));
            else
                return Edges(element as dynamic);
        }

        /***************************************************/

        private static IEnumerable<ICurve> Edges(this IAreaElement element)
        {
            Reflection.Compute.RecordWarning("Can not extract edges for obejcts of type " + element.GetType().FullName);
            return new List<ICurve>();
        }

        /***************************************************/
    }

}

