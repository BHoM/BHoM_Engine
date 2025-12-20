/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Gets the internal edges of an Extrusion surface.")]
        [Input("surface", "The Extrusion to get internal edges from.")]
        [Output("edges", "The internal edges of the Extrusion.")]
        public static List<ICurve> InternalEdges(this Extrusion surface)
        {
            ICurve curve = surface.Curve;
            Vector direction = surface.Direction;

            List<ICurve> edges = new List<ICurve>();
            if (surface.Capped)
            {
                edges.Add(curve);
                edges.Add(surface.Curve.ITranslate(surface.Direction));
            }

            if (curve.IIsClosed())
            {
                Point start = curve.IStartPoint();
                edges.Add(new Line { Start = start, End = start + direction });
            }

            if (curve.IIsClosed())
            {
                Point end = curve.IEndPoint();
                edges.Add(new Line { Start = end, End = end + direction });
            }

            return edges;
        }

        /***************************************************/

        [Description("Gets the internal edges of a Pipe surface.")]
        [Input("surface", "The Pipe to get internal edges from.")]
        [Output("edges", "The internal edges of the Pipe.")]
        public static List<ICurve> InternalEdges(this Pipe surface)
        {
            if (surface.Capped)
            {
                ICurve curve = surface.Centreline;
                return new List<ICurve>()
                {
                    new Circle { Centre = curve.IStartPoint(), Normal = curve.IStartDir(), Radius = surface.Radius },
                    new Circle { Centre = curve.IEndPoint(), Normal = curve.IEndDir(), Radius = surface.Radius }
                };
            }
            else
            {
                return new List<ICurve>();
            }
        }

        /***************************************************/

        [Description("Gets the internal edges of a PlanarSurface.")]
        [Input("surface", "The PlanarSurface to get internal edges from.")]
        [Output("edges", "The internal edges of the PlanarSurface.")]
        public static List<ICurve> InternalEdges(this PlanarSurface surface)
        {
            return surface.InternalBoundaries;
        }

        /***************************************************/

        [Description("Gets the internal edges of a PolySurface.")]
        [Input("surface", "The PolySurface to get internal edges from.")]
        [Output("edges", "The internal edges of the PolySurface.")]
        public static List<ICurve> InternalEdges(this PolySurface surface)
        {
            return surface.Surfaces.SelectMany(x => x.IInternalEdges()).ToList();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the internal edges of any ISurface.")]
        [Input("surface", "The ISurface to get internal edges from.")]
        [Output("edges", "The internal edges of the surface.")]
        public static List<ICurve> IInternalEdges(this ISurface surface)
        {
            return InternalEdges(surface as dynamic);
        }

        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static List<ICurve> InternalEdges(this object surface)
        {
            Base.Compute.RecordError($"InternalEdges is not implemented for objects of type: {surface.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}
