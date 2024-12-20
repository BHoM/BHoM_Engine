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

using BH.oM.Geometry;
using System;
using System.Linq;
using System.ComponentModel;
using BH.oM.Data.Collections;
using System.Collections.Generic;
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a spatial data tree from the data. Useful for spatial queries in large data sets.")]
        [Input("geometries", "The geometries to store in the data tree.")]
        [Input("treeDegree", "Degree of the tree. Determinse the number of children each node of the tree can have.")]
        [Input("leafSize", "Determinse the number of siblings a leaf node can have.")]
        [Input("sampleSize", "The number of items used to determine how to split the collection.")]
        [Output("domainTree", "A spatial data tree containing all the provvided geometries in its leaves.")]
        public static DomainTree<T> DomainTree<T>(this IEnumerable<T> geometries, int treeDegree = 16, int leafSize = 16, int sampleSize = 60) where T : IGeometry
        {
            return Data.Create.DomainTree(geometries, x => x.IBounds().DomainBox(), treeDegree, leafSize, sampleSize);
        }

        /***************************************************/
    }
}




