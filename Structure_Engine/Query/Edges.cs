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

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Base.Attributes;
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
        [Input("surface", "The structural surface to extract edges from.")]
        [Output("edges", "The list of curves representing the edges of the underlying surface.")]
        public static List<ICurve> Edges(this Surface surface)
        {
            if (surface.IsNull())
                return null;
            if (surface.Extents != null)
                return surface.Extents.IExternalEdges();
            else
                return new List<ICurve>();
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Extracts all the edge curves from an AreaElement.")]
        [Input("element", "The element to extract opening edges from.")]
        [Output("edges", "The list of curves representing all internal and external edges of an element.")]
        public static IEnumerable<ICurve> IEdges(this IAreaElement element)
        {
            return element.IIsNull() ? null : Edges(element as dynamic);
        }

        /***************************************************/

        [Description("Extracts all the edge curves from an AreaElement.")]
        [Input("element", "The element to extract opening edges from.")]
        [Output("edges", "The list of curves representing all internal and external edges of an element.")]
        private static IEnumerable<ICurve> Edges(this Panel element)
        {
            return Engine.Spatial.Query.ElementCurves(element, true);
        }

        private static IEnumerable<ICurve> Edges(this IAreaElement element)
        {
            Base.Compute.RecordWarning("Can not extract edges for objects of type " + element.GetType().FullName);
            return new List<ICurve>();
        }

        /***************************************************/
    }

}




