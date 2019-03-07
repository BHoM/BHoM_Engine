﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a PlanarSurface based on boundary curves. Only processing done by this method is checking (co)planarity and that the curves are closed. Internal edges will be assumed to be inside the External")]
        [Input("externalEdge", "The outer boundary curve of the surface. Needs to be closed and planar")]
        [Input("internalEdges", "Optional internal edgescurves descibing any openings with the external edge. All internal edges need to be closed and co-planar with the external edge")]
        [Output("PlanarSurface", "Planar surface corresponding to the provided edge curves")]
        public static PlanarSurface PlanarSurface(ICurve externalEdge, List<ICurve> internalEdges = null)
        {
            if (!externalEdge.IIsPlanar())
            {
                Reflection.Compute.RecordError("External edge curve is not planar");
                return null;
            }

            if (!externalEdge.IIsClosed())
            {
                Reflection.Compute.RecordError("External edge curve is not closed");
                return null;
            }

            internalEdges = internalEdges ?? new List<ICurve>();

            foreach (ICurve crv in internalEdges)
            {
                if (!crv.IIsPlanar())
                {
                    Reflection.Compute.RecordError("At least one of the internal edge curves is not planar");
                    return null;
                }

                if (!crv.IIsClosed())
                {
                    Reflection.Compute.RecordError("At least one of the internal edge curves is not closed");
                    return null;
                }
            }

            if (internalEdges.Count > 0)
            {
                if (!Query.IsCoplanar(externalEdge.IControlPoints().Concat(internalEdges.SelectMany(x => x.IControlPoints())).ToList()))
                {
                    Reflection.Compute.RecordError("The provided curves are not co-planar");
                    return null;
                }
            }

            return new PlanarSurface { ExternalBoundary = externalEdge, InternalBoundaries = internalEdges };
        }

        /***************************************************/

        [Description("Distributes the edge curve and creates a set of boundary planar surfaces")]
        [Input("edges", "Boundary curves to be used. Non-planar and non-closed curves are ignored")]
        [Output("PlanarSurface", "List of planar surfaces created")]
        public static List<PlanarSurface> PlanarSurface(List<ICurve> edges)
        {
            List<ICurve> checkedCurves = edges.Where(x => x.IIsClosed() && x.IIsPlanar()).ToList();
            List<List<ICurve>> distributed = Compute.DistributeOutlines(checkedCurves);

            List<PlanarSurface> surfaces = new List<PlanarSurface>();

            for (int i = 0; i < distributed.Count; i++)
            {
                PlanarSurface srf = new PlanarSurface()
                {
                    ExternalBoundary = distributed[i][0],
                    InternalBoundaries = distributed[i].Skip(1).ToList()
                };

                surfaces.Add(srf);
            }

            return surfaces;
        }
        

        /***************************************************/
    }
}
