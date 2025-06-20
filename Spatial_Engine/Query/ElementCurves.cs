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

using BH.Engine.Geometry;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        [Description("Queries the defining curves of an IElement0D. Always returns empty collection due to zero-dimensionality of an IElement0D.")]
        [Input("element0D", "The IElement0D to extract the defining curves from.")]
        [Input("recursive", "Has no effect for IElement0D. Input here to unify inputs between all IElements.")]
        [Output("elementCurves", "The curves defining the base geometry of the IElement0D.")]
        public static List<ICurve> ElementCurves(this IElement0D element0D, bool recursive = true)
        {
            return new List<ICurve>();
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Queries the defining curves of an IElement1D.")]
        [Input("element1D", "The IElement1D of which to get the curve definition.")]
        [Input("recursive", "Has no effect for IElement1D. Input here to unify inputs between all IElements.")]
        [Output("elementCurves", "The curve defining the base geometry of the IElement1D.")]
        public static List<ICurve> ElementCurves(this IElement1D element1D, bool recursive = true)
        {
            return new List<ICurve> { element1D.IGeometry() };
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [PreviousInputNames("element2D", "panel,opening")]
        [Description("Queries the geometrically defining curves of the IElement2Ds surface.")]
        [Input("element2D", "The IElement2D of which to get the curve definition.")]
        [Input("recursive", "Ensures that the resulting curves are broken up into its smallest constituent parts.")]
        [Output("elementCurves", "The curves defining the base surface geometry of the IElement2D.")]
        public static List<ICurve> ElementCurves(this IElement2D element2D, bool recursive = true)
        {
            List<ICurve> result = new List<ICurve>();

            PolyCurve outline = element2D.OutlineCurve();
            foreach (ICurve curve in outline.Curves)
            {
                if (recursive)
                    result.AddRange(curve.ISubParts());
                else
                    result.Add(curve);
            }

            foreach (IElement2D e in element2D.IInternalElements2D())
            {
                result.AddRange(e.ElementCurves(recursive));
            }

            return result;
        }

        /******************************************/

        [PreviousInputNames("element2D", "panel")]
        [Description("Queries the geometrically defining external curves of the IElement2Ds surface.")]
        [Input("element2D", "The IElement2D of which to get the external curve definition.")]
        [Input("recursive", "Ensures that the resulting curves are broken up into its smallest constituent parts.")]
        [Output("elementCurves", "The curves defining the base surface geometry of the IElement2D.")]
        public static List<ICurve> ExternalElementCurves(this IElement2D element2D, bool recursive = true)
        {
            List<ICurve> result = new List<ICurve>();
            PolyCurve outline = element2D.OutlineCurve();
            foreach (ICurve curve in outline.Curves)
            {
                if (recursive)
                    result.AddRange(curve.ISubParts());
                else
                    result.Add(curve);
            }
            return result;
        }

        /******************************************/

        [PreviousInputNames("element2D", "panel")]
        [Description("Queries the geometrically defining internal curves, such as Openings, of the IElement2Ds surface.")]
        [Input("element2D", "The IElement2D of which to get the internal curve definition.")]
        [Input("recursive", "Ensures that the resulting curves are broken up into its smallest constituent parts.")]
        [Output("elementCurves", "The curves defining the base surface geometry of the IElement2D.")]
        public static List<ICurve> InternalElementCurves(this IElement2D element2D, bool recursive = true)
        {
            return element2D.IInternalElements2D().Where(x => x != null).SelectMany(x => x.ElementCurves(recursive)).ToList();
        }

        /******************************************/
        /****            IElement              ****/
        /******************************************/

        [Description("Queries the geometrically defining curves of the IElements geometries.")]
        [Input("elements", "The IElements of which to get the curve definitions.")]
        [Input("recursive", "Ensures that the resulting curves are broken up into its smallest constituent parts if it is an IElement2D.")]
        [Output("elementCurves", "The curves defining the base geometry of the IElements.")]
        public static List<ICurve> ElementCurves(this IEnumerable<IElement> elements, bool recursive = true)
        {
            List<ICurve> result = new List<ICurve>();
            foreach (IElement element in elements)
            {
                result.AddRange(element.IElementCurves(recursive));
            }
            return result;
        }


        /******************************************/
        /**** Public Methods - Interfaces      ****/
        /******************************************/

        [Description("Queries the geometrically defining curves of the IElements geometry.")]
        [Input("element", "The IElement of which to get the curve definition.")]
        [Input("recursive", "Ensures that the resulting curves are broken up into its smallest constituent parts if it is an IElement2D.")]
        [Output("elementCurves", "The curves defining the base geometry of the IElement.")]
        public static List<ICurve> IElementCurves(this IElement element, bool recursive = true)
        {
            return ElementCurves(element as dynamic, recursive);
        }
        
        /******************************************/
    }
}





