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

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static List<ICurve> ExternalEdges(this Extrusion surface)
        {
            ICurve curve = surface.Curve;
            Vector direction = surface.Direction;
            List<ICurve> edges = new List<ICurve>();

            if (!surface.Capped)
            {
                edges.Add(curve);
                ICurve other = curve.DeepClone();
                edges.Add(other.ITranslate(direction));
            }

            if (!curve.IIsClosed())
            {
                Point start = curve.IStartPoint();
                Point end = curve.IEndPoint();
                edges.Add(new Line { Start = start, End = start + direction });
                edges.Add(new Line { Start = end, End = end + direction });
            }

            return edges;
        }

        /***************************************************/

        public static List<ICurve> ExternalEdges(this Loft surface)
        {
            return surface.Curves; //TODO: Is that always correct?
        }

        /***************************************************/

        public static List<ICurve> ExternalEdges(this Pipe surface)
        {
            if (!surface.Capped)
            {
                ICurve curve = surface.Centreline;
                return new List<ICurve>()
                {
                    new Circle { Centre = curve.IStartPoint(), Normal = curve.IStartDir(), Radius = surface.Radius },
                    new Circle { Centre = curve.IEndPoint(), Normal = curve.IEndDir(), Radius = surface.Radius }
                };
            }
            else
                return new List<ICurve>();
        }

        /***************************************************/

        public static List<ICurve> ExternalEdges(this PlanarSurface surface)
        {
            return new List<ICurve> { surface.ExternalBoundary };
        }

        /***************************************************/

        public static List<ICurve> ExternalEdges(this PolySurface surface)
        {
            return surface.Surfaces.SelectMany(x => x.IExternalEdges()).ToList();
        }


        /***************************************************/
        /**** Public Methods - Mesh                     ****/
        /***************************************************/

        public static List<Line> ExternalEdges(this Mesh mesh)
        {
            if (mesh == null)
                return null;

            if (mesh.Faces.Count < 1)
                return null;

            List<Line> edges = mesh.Faces.SelectMany(f => f.Edges(mesh)).ToList();

            for (int i = edges.Count - 1; i > 0; i--)
            {
                for (int n = i - 1; n >= 0; n--)
                {
                    if ((edges[i].Start == edges[n].Start & edges[i].End == edges[n].End) | // edge[i] == edge[n]
                        (edges[i].Start == edges[n].End & edges[i].End == edges[n].Start))   // edge[i] == edge[n].Reverse()
                    {
                        edges.RemoveAt(i); // shared edge so remove both
                        edges.RemoveAt(n);
                        i--;
                        break;
                    }
                }
            }
            return edges;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<ICurve> IExternalEdges(this ISurface surface)
        {
            return ExternalEdges(surface as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static List<ICurve> ExternalEdges(this ISurface surface)
        {
            Base.Compute.RecordError($"ExternalEdges is not implemented for ISurface of type: {surface.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}

