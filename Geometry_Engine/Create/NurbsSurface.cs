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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [NotImplemented]
        [Description("Creates a random NurbsSurface for testing purposes. This method is not yet implemented.")]
        [Input("rnd", "Random number generator to use for creating the surface.")]
        [Input("box", "Optional bounding box to constrain the random surface within. If null, a default bounding box will be used.")]
        [Input("minNbCPs", "Minimum number of control points to generate in each direction. Must be at least 4.", typeof(int))]
        [Input("maxNbCPs", "Maximum number of control points to generate in each direction. Must be greater than minNbCPs.", typeof(int))]
        [Output("surface", "A randomly generated NurbsSurface. Currently throws NotImplementedException.")]
        public static NurbsSurface RandomNurbsSurface(Random rnd, BoundingBox box = null, int minNbCPs = 4, int maxNbCPs = 20)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
    }
}
