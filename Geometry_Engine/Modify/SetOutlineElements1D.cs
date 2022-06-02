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

using System;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Geometry;
using BH.Engine.Base;


namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Sets the Outline Element1Ds of an PlanarSurface, i.e. the ExternalBoundary. Method required for all IElement2Ds.\n" +
                     "The provided edges all need to be ICurves and should form a closed loop. No checking for planarity is made by the method.\n" +
                     "The Method will return a new PlanarSurface with the provided edges as ExternalBoundary and InternalBoundaries set to those of the provided PlanarSurface.\n" +
                     "This means that the method could provide a PlanarSurface that have an ExternalBoundary that is not co-planar with the InternalBoundaries. This is required for the IElement workflow to work.")]
        [Input("surface", "The PlanarSurface to update the ExternalEdge of.")]
        [Input("edges", "A list of IElement1Ds which all should be of a type of ICurve.")]
        [Output("surface", "A new PlanarSurface with ExternalBoundary matching the provided edges and InternalBoundaries from the provided PlanarSurface.")]
        public static PlanarSurface SetOutlineElements1D(this PlanarSurface surface, IEnumerable<IElement1D> edges)
        {
            if (surface == null || edges == null || !edges.Any())
                return surface;

            ICurve externalEdge = Geometry.Compute.IJoin(edges.Cast<ICurve>().ToList()).First();

            return new PlanarSurface(externalEdge, surface.InternalBoundaries);
        }

        /***************************************************/

    }
}


