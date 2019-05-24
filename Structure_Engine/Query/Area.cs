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

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Area(this Panel panel)
        {
            List<PolyCurve> externalEdges = BH.Engine.Geometry.Compute.IJoin(panel.ExternalEdgeCurves());
            List<PolyCurve> internalEdges = BH.Engine.Geometry.Compute.IJoin(panel.InternalEdgeCurves());

            return externalEdges.Select(x => x.Area()).Sum() - internalEdges.Select(x => x.Area()).Sum();
        }

        /***************************************************/

        public static double Area(this Opening opening)
        {
            List<PolyCurve> edges = BH.Engine.Geometry.Compute.IJoin(opening.EdgeCurves());

            return edges.Select(x => x.Area()).Sum();
        }

        /***************************************************/

        public static double Area(this FEMesh mesh)
        {
            return mesh.Geometry().Area();
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IArea(this IAreaElement element)
        {
            return Area(element as dynamic);
        }

        /***************************************************/
    }

}
