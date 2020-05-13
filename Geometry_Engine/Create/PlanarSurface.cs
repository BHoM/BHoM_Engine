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
        [Input("externalBoundary", "The outer boundary curve of the surface. Needs to be closed and planar")]
        [Input("internalBoundaries", "Optional internal boundary curves descibing any openings inside the external. All internal edges need to be closed and co-planar with the external edge")]
        [Output("PlanarSurface", "Planar surface corresponding to the provided edge curves")]
        public static PlanarSurface PlanarSurface(ICurve externalBoundary, List<ICurve> internalBoundaries = null)
        {
            if (!externalBoundary.IIsPlanar())
            {
                Reflection.Compute.RecordError("External edge curve is not planar");
                return null;
            }

            if (!externalBoundary.IIsClosed())
            {
                Reflection.Compute.RecordError("External edge curve is not closed");
                return null;
            }

            internalBoundaries = internalBoundaries ?? new List<ICurve>();

            foreach (ICurve crv in internalBoundaries)
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

            if (internalBoundaries.Count > 0)
            {
                if (!Query.IsCoplanar(externalBoundary.IControlPoints().Concat(internalBoundaries.SelectMany(x => x.IControlPoints())).ToList()))
                {
                    Reflection.Compute.RecordError("The provided curves are not co-planar");
                    return null;
                }
            }

            bool nonExistantCheckWarning = false;

            try
            {
                if (externalBoundary.IIsSelfIntersecting())
                {
                    Reflection.Compute.RecordError("The provided externalBoundary is selfintersecting.");
                    return null;
                }
            } catch
            {
                nonExistantCheckWarning = true;
            }

            try
            {
                if (internalBoundaries.Any(x => x.IIsSelfIntersecting()))
                {
                    Reflection.Compute.RecordError("At least one of the internalBoundaries is selfintersecting.");
                    return null;
                }
            }
            catch
            {
                nonExistantCheckWarning = true;
            }

            try
            {
                if (internalBoundaries.Any(x => !externalBoundary.IIsContaining(x)))
                {
                    Reflection.Compute.RecordError("At least one of the internalBoundaries is not contained by the externalBoundary.");
                    return null;
                }
            } catch
            {
                nonExistantCheckWarning = true;
            }

            bool unionDone = false;
            try
            { 
                for (int i = 0; i < internalBoundaries.Count; i++)
                {
                    for (int j = i + 1; j < internalBoundaries.Count; j++)
                    {
                        if (internalBoundaries[i].ICurveIntersections(internalBoundaries[j]).Count != 0)
                        {
                            internalBoundaries = Compute.BooleanUnion(internalBoundaries).Cast<ICurve>().ToList();
                            Reflection.Compute.RecordWarning("InternalBounderies were overlapping, BooleanUnion has been used to get non overlapping regions.");
                            i = internalBoundaries.Count;
                            j = i;
                            unionDone = true;
                        }
                    }
                }
            }
            catch
            {
                nonExistantCheckWarning = true;
            }

            try
            {
                if (!unionDone)
                {
                    for (int i = 0; i < internalBoundaries.Count; i++)
                    {
                        for (int j = 0; j < internalBoundaries.Count; j++)
                        {
                            if (internalBoundaries[i].IIsContaining(internalBoundaries[j]))
                            {
                                internalBoundaries = Compute.BooleanUnion(internalBoundaries).Cast<ICurve>().ToList();
                                Reflection.Compute.RecordWarning("InternalBounderies were overlapping, BooleanUnion has been used to get non overlapping regions.");
                                i = internalBoundaries.Count;
                                j = i;
                            }
                        }
                    }
                }
            }
            catch
            {
                nonExistantCheckWarning = true;
            }

            if (nonExistantCheckWarning)
                Reflection.Compute.RecordWarning("Neeseccary checks to ensure vadility of the PlanarSurface is not implemented. The PlanarSurfaces curves relations are not garanteueed.");

            return new PlanarSurface(externalBoundary, internalBoundaries);
        }

        /***************************************************/

        [Description("Distributes the edge curve and creates a set of boundary planar surfaces")]
        [Input("boundaryCurves", "Boundary curves to be used. Non-planar and non-closed curves are ignored")]
        [Output("PlanarSurface", "List of planar surfaces created")]
        public static List<PlanarSurface> PlanarSurface(List<ICurve> boundaryCurves)
        {
            List<ICurve> checkedCurves = boundaryCurves.Where(x => x.IIsClosed() && x.IIsPlanar()).ToList();
            List<List<ICurve>> distributed = Compute.DistributeOutlines(checkedCurves);

            List<PlanarSurface> surfaces = new List<PlanarSurface>();

            for (int i = 0; i < distributed.Count; i++)
            {
                PlanarSurface srf = new PlanarSurface(
                    distributed[i][0],
                    distributed[i].Skip(1).ToList()
                );

                surfaces.Add(srf);
            }

            return surfaces;
        }
        

        /***************************************************/
    }
}

