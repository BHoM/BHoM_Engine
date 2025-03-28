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

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Analytical.Elements;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the entity closest to a Point from a collection of entities.")]
        [Input("entities", "The collection of entities to search.")]
        [Input("point", "The Point to search from.")]
        [Output("entity", "The IElement0D closest to the Point.")]
        public static IElement0D ClosestIElement0D<T>(this List<T> entities, Point point)
            where T : IElement0D
        {
            if (entities.Count == 0)
                return null;

            IElement0D closest = entities.Select(p => new { Node = p, Distance2 = p.IGeometry().Distance(point) })
                            .Aggregate((p1, p2) => p1.Distance2 < p2.Distance2 ? p1 : p2)
                            .Node;

            return closest;
            
        }
    }
}





