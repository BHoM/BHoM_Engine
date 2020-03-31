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
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Queries the geometricly defining curve of the IElement1D.")]
        [Input("element1D", "The IElement1D of which to get the curve definintion.")]
        [Input("recursive", "Has no effect for IElement1D. Input here to unify inputs between all IElements.")]
        [Output("elementCurves", "The curve defining the base geometry of the IElement1D.")]
        public static List<ICurve> ElementCurves(this IElement1D element1D, bool recursive = true)
        {
            return new List<ICurve> { element1D.IGeometry() };
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [Description("Queries the geometricly defining curves of the IElement2Ds surface.")]
        [Input("element2D", "The IElement2D of which to get the curve definintion.")]
        [Input("recursive", "Ensures that the resulting curves are broken up into its smallest constituent parts.")]
        [Output("elementCurves", "The curves defining the base surface geometry of the IElement2D.")]
        public static List<ICurve> ElementCurves(this IElement2D element2D, bool recursive)
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
        /****            IElement              ****/
        /******************************************/

        [Description("Queries the geometricly defining curves of the IElements geometries.")]
        [Input("elements", "The IElements of which to get the curve definintions.")]
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

        [Description("Queries the geometricly defining curves of the IElements geometry.")]
        [Input("element", "The IElement of which to get the curve definintion.")]
        [Input("recursive", "Ensures that the resulting curves are broken up into its smallest constituent parts if it is an IElement2D.")]
        [Output("elementCurves", "The curves defining the base geometry of the IElement.")]
        public static List<ICurve> IElementCurves(this IElement element, bool recursive = true)
        {
            return ElementCurves(element as dynamic, recursive);
        }
        
        /******************************************/
    }
}
