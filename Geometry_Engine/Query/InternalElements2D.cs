/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using System.Linq;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Gets inner IElement2Ds from a PlanarSurface. Method required for all IElement2Ds. \n" +
         "For a PlanarSurface this method will return a list of new PlanarSurfaces with ExternalBoundary corresponding to the InternalBoundaries of the PlanarSurface being queried.")]
        [Input("surface", "The PlanarSurface to get internal IElement2Ds from.")]
        [Output("elements", "The list of the internal IElement2Ds of the PlanarSurface, i.e. a list of PlanarSurfaces with ExternalBoundary matching the InternalBoundaries of the PlanarSurface provided.")]
        public static List<IElement2D> InternalElements2D(this PlanarSurface surface)
        {
            if (surface == null || surface.InternalBoundaries == null)
                return new List<IElement2D>();

            return surface.InternalBoundaries.Select(x => new PlanarSurface(x, new List<ICurve>())).ToList<IElement2D>();
        }

        /***************************************************/
    }
}





