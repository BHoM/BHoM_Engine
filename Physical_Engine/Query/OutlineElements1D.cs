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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Dimensional;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the external outline elements of an ISurface.")]
        [Input("surface", "The ISurface to get the external outline elements from.")]
        [Output("curves", "The curves defining the external boundery of the ISurface.")]
        public static List<IElement1D> OutlineElements1D(this oM.Physical.Elements.ISurface surface)
        {
            if (surface == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the outline 1D elements of a null surface.");
                return new List<IElement1D>();
            }

            PlanarSurface pSurface = surface.Location as PlanarSurface;
            if (pSurface == null)
            {
                Engine.Base.Compute.RecordError("Not implemented for non-PlanarSurfaces");
                return null;
            }

            return pSurface.ExternalBoundary.ISubParts().ToList<IElement1D>();
        }

        /***************************************************/

        [Description("Gets the external outline elements of an IOpening.")]
        [Input("opening", "The IOpening to get the external outline elements from.")]
        [Output("curves", "The curves defining the external boundery of the IOpening.")]
        public static List<IElement1D> OutlineElements1D(this IOpening opening)
        {
            if (opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the outline 1D elements of a null opening.");
                return new List<IElement1D>();
            }

            PlanarSurface pSurface = opening.Location as PlanarSurface;
            if (pSurface == null)
            {
                Engine.Base.Compute.RecordError("Not implemented for non-PlanarSurfaces");
                return null;
            }

            return pSurface.ExternalBoundary.ISubParts().ToList<IElement1D>();
        }

        /***************************************************/

        [Description("Gets the external outline elements of a PadFoundation.")]
        [Input("padFoundation", "The PadFoundation to get the external outline elements from.")]
        [Output("curves", "The curves defining the external boundery of the PadFoundation.")]
        public static List<IElement1D> OutlineElements1D(this PadFoundation padFoundation)
        {
            if (padFoundation.IsNull())
                return null;

            PlanarSurface surface = padFoundation.Location;

            return surface.ExternalBoundary.ISubParts().ToList<IElement1D>();
        }

        /***************************************************/


    }
}




