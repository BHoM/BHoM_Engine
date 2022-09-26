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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Base;


namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/


        [Description("Sets the InternalElement2Ds of an PlanarSurface, i.e. a set of PlanarSurfaces with ExternalBoundary matching the new InternalBoundaries to be applied to the PlanarSurface. Method required for all IElement2Ds.\n" +
                     "The provided openings all need to be PlanarSurfaces.\n" +
                     "The Method will return a new PlanarSurface with the provided openings (PlanarSurfaces) as InternalBoundaries and ExternalBoundary set to that of the provided PlanarSurface.\n" +
                     "This means that the method could provide a PlanarSurface that have an ExternalEdge that is not co-planar with the InternalBoundaries. This is required for the IElement workflow to work.")]
        [Input("surface", "The PlanarSurface to update.")]
        [Input("openings", "The internal IElement2Ds to set. For an PlanarSurface this should be a list of PlanarSurfaces with ExternalBoundary matching what is to be set as InternalBoundaries for the provided PlanarSurface.")]
        [Output("surface", "A new PlanarSurface with ExternalBoundary from the provided PlanarSurface and InternalBoundaries matching the provided openings.")]
        public static PlanarSurface SetInternalElements2D(this PlanarSurface surface, List<IElement2D> openings)
        {
            if (surface == null || openings == null)
                return surface;

            List<ICurve> openingCurves = openings.Cast<PlanarSurface>().Select(x => x.ExternalBoundary).ToList();
            return new PlanarSurface(surface.ExternalBoundary, openingCurves);
        }

        /***************************************************/
    }
}



