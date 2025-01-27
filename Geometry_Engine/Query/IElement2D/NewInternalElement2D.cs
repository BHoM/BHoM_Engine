/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using System.ComponentModel;


namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a a new PlanarSurface that can be used as internal element for another PlanarSurface. The resulting PlanarSurface will have a null ExternalBoundary and empty list of InternalBoundaries. The resulting PlanarSurface is casted to IElement2D for generalization purposes.  \n" +
                     "Method required for any IElement2D that contians internal IElement2Ds.")]
        [Input("surface", "PlanarSurface just used to determine the appropriate type of IElement2D to create.")]
        [Output("opening", "The created Opening as a IElement2D.")]
        public static IElement2D NewInternalElement2D(this PlanarSurface surface)
        {
            return new PlanarSurface(null, new System.Collections.Generic.List<ICurve>());
        }

        /***************************************************/
    }
}






