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
using System.Linq;
using BH.oM.Geometry;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Gets the edge elements from an PlanarSurface defining the boundary of the element. Method required for all IElement2Ds. \n" +
             "For an PlanarSurface this will return a list of its Curves coresponding to the SubParts of the ExternalBoundary of the PlanarSurface.")]
        [Input("surface", "The PlanarSurface to get outline elements from.")]
        [Output("elements", "Outline elements of the PlanarSurface, i.e. the ExternalEdges sub parts of the PlanarSurface.")]
        public static List<IElement1D> OutlineElements1D(this PlanarSurface surface)
        {
            return surface.ExternalBoundary.ISubParts().ToList<IElement1D>();
        }

        /***************************************************/

    }
}


