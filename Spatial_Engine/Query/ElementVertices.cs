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
        /****            IElement0D            ****/
        /******************************************/

        [Description("Returns the point from the IElement0D. Mainly to accomodate the interface method.")]
        [Input("element0D", "The IElement0D to query for its vertices.")]
        [Output("vertices", "The location point of IElement0D.")]
        public static List<Point> ElementVertices(this IElement0D element0D)
        {
            return new List<Point>() { element0D.IGeometry() };
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Returns the discontinuity points from the defining ICurve of the IElement1D.")]
        [Input("element1D", "The IElement1D of which to get the vertecies from.")]
        [Output("vertices", "The IElement1Ds curves discontinuity points.")]
        public static List<Point> ElementVertices(this IElement1D element1D)
        {
            ICurve curve = element1D.IGeometry();
            List<Point> vertices;
            if (curve is Polyline)
                vertices = ((Polyline)curve).ControlPoints;
            else
                vertices = curve.IDiscontinuityPoints();

            if (curve.IIsClosed())
                vertices.RemoveAt(vertices.Count - 1);

            return vertices;
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [Description("Returns the discontinuity points from the defining ICurves of the IElement2D.")]
        [Input("element2D", "The IElement2D of which to get the vertecies from.")]
        [Output("vertices", "The IElement2Ds element curves discontinuity points.")]
        public static List<Point> ElementVertices(this IElement2D element2D)
        {
            List<Point> result = new List<Point>();
            result.AddRange(element2D.OutlineCurve().ElementVertices());
            foreach (IElement2D e in element2D.IInternalElements2D())
            {
                result.AddRange(e.ElementVertices());
            }

            return result;
        }

        /******************************************/

        [PreviousVersion("3.2", "BH.Engine.Common.Query.IElementVertices(System.Collections.Generic.IEnumerable<BH.oM.Dimensional.IElement>)")]
        [Description("Returns the discontinuity points from the defining ICurves of the IElements.")]
        [Input("elements", "The IElements of which to get the vertecies from.")]
        [Output("vertices", "The IElements element curves discontinuity points.")]
        public static List<Point> ElementVertices(this IEnumerable<IElement> elements)
        {
            List<Point> result = new List<Point>();
            foreach (IElement element in elements)
            {
                result.AddRange(element.IElementVertices());
            }
            return result;
        }


        /******************************************/
        /**** Public Methods - Interfaces      ****/
        /******************************************/

        [Description("Returns the discontinuity points from the defining ICurves of the IElement.")]
        [Input("element", "The IElement of which to get the vertecies from.")]
        [Output("vertices", "The IElements element curves discontinuity points.")]
        public static List<Point> IElementVertices(this IElement element)
        {
            return ElementVertices(element as dynamic);
        }

        /******************************************/

    }
}
