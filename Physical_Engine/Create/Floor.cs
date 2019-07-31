/*
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

using System.ComponentModel;

using BH.oM.Physical.Constructions;
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Physical
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates floor base on given construction and edges")]
        [Input("construction", "Construction of the floor")]
        [Input("edges", "Edges of the floor")]
        [Output("Physical Floor")]
        public static Floor Floor(Construction construction, ICurve edges)
        {
            if (construction == null || edges == null)
                return null;

            PlanarSurface aPlanarSurface = Geometry.Create.PlanarSurface(edges);
            if (aPlanarSurface == null)
                return null;

            Floor aFloor = new Floor()
            {
                Construction = construction,
                Location = aPlanarSurface
            };

            return aFloor;
        }

        /***************************************************/

        [Description("Creates floor base on given construction, edges and internal edges")]
        [Input("construction", "Construction of the wall")]
        [Input("edges", "Edges of the floor")]
        [Input("internalEdges", "Internal edges of openings etc.")]
        [Output("Physical Floor")]
        public static Floor Floor(Construction construction, ICurve edges, IEnumerable<ICurve> internalEdges = null)
        {

            if (construction == null || edges == null)
                return null;

            List<ICurve> aInternalCurveList = null;
            if (internalEdges != null && internalEdges.Count() > 0)
                aInternalCurveList = internalEdges.ToList().ConvertAll(x => x as ICurve);

            PlanarSurface aPlanarSurface = Geometry.Create.PlanarSurface(edges, aInternalCurveList);
            if (aPlanarSurface == null)
                return null;

            Floor aFloor = new Floor()
            {
                Construction = construction,
                Location = aPlanarSurface
            };

            return aFloor;
        }

        /***************************************************/
    }
}
