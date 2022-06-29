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

using BH.oM.Geometry;
using System;
using System.ComponentModel;

using BH.oM.Base.Attributes;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a list of PlanarSurfaces where each surface Normal is within the tolerance angle of the given vector.")]
        [Input("surfaces", "A list of PlanarSurfaces to filter by a given Normal vector.")]
        [Input("vector", "The Vector to filter by.")]
        [Input("tolerance", "Angle tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Angle.")]
        [Output("matchingSurfaces", "A list of PlanarSurfaces where each surface Normal is within the tolerance angle of the given vector.")]
        public static List<PlanarSurface> FilterByNormal(this List<PlanarSurface> surfaces, Vector vector, double tolerance = Tolerance.Angle)
        {
            List<PlanarSurface> matchingSurfaces = new List<PlanarSurface>();
            foreach (PlanarSurface srf in surfaces)
            {
                if (Math.Abs(Query.Angle(vector, srf.Normal())) < tolerance)
                    matchingSurfaces.Add(srf);
            }
            return matchingSurfaces;
        }
    }
}
