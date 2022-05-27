/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Queries the indices of this face's vertices.")]
        [Input("face", "The face to query.")]
        [Output("array", "Array containing the indices of the face's vertecies.")]
        public static int[] Vertices(this Face face)
        {
            if (face.IsQuad())
            {
                return new int[]
                {
                    face.A, face.B, face.C, face.D
                };
            }
            else
            {
                return new int[]
                {
                    face.A, face.B, face.C
                };
            }
        }

        /***************************************************/

        [Description("Returns all the unique of the polyline if it is closed. This means that the last control point will be omitted for a closed Polyline. Undefined if the curve is open.")]
        [Input("pLine", "The Polyline to extract vertices from.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("vertices", "Vertices of the Polyline.")]
        public static List<Point> Vertices(this Polyline pLine, double tolerance = Tolerance.Distance)
        {
            if (!pLine.IsClosed(tolerance))
            {
                Base.Compute.RecordError("Input curve is not closed. Verticies not defined.");
                return new List<Point>();
            }

            return pLine.ControlPoints.GetRange(0, pLine.ControlPoints.Count - 1);
        }

        /***************************************************/

        [Description("Returns the Vertices of the Polygon.")]
        [Input("pGon", "The Polygon to extract the Vertices from.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("vertices", "Vertices of the Polyline.")]
        public static List<Point> Vertices(this Polygon pGon, double tolerance = Tolerance.Distance)
        {
            return pGon.Vertices.ToList();
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Returns the Vertices of the IPolyline if it is closed. Undefined for open IPolylines.")]
        [Input("pline", "The IPolyline to extract the Vertices from.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("vertices", "Vertices of the IPolyline.")]
        public static List<Point> IVertices(this IPolyline pline, double tolerance = Tolerance.Distance)
        {
            return Vertices(pline as dynamic, tolerance);
        }

        /***************************************************/
    }
}



