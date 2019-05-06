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

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<ICurve> Edges(this Surface surface)
        {
            if (surface.Extents != null)
                return surface.Extents.IExternalEdges();
            else
                return new List<ICurve>();
        }

        /***************************************************/

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

        public static List<ICurve> ExternalEdgeCurves(this Panel panel)
        {
            return panel.ExternalEdges.Select(x => x.Curve).ToList();
        }

        /***************************************************/

        public static List<ICurve> AllEdgeCurves(this Panel panel)
        {
            List<ICurve> result = panel.ExternalEdgeCurves();
            result.AddRange(panel.InternalEdgeCurves());
            return result;
        }

        /***************************************************/

        public static List<ICurve> EdgeCurves(this Opening opening)
        {
            return opening.Edges.Select(e => e.Curve).ToList();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IEnumerable<ICurve> IEdges(this IAreaElement element)
        {
            if (element is Panel)
                return (AllEdgeCurves(element as Panel));
            else
                return Edges(element as dynamic);
        }

        /***************************************************/
    }

}
