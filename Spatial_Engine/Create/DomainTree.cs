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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Spatial.Layouts;
using BH.Engine.Geometry;
using BH.oM.Data.Collections;
using BH.oM.Dimensional;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a spatial data tree from the data. Useful for spatial queries in large data sets.")]
        [Input("elements", "The elements to store in the data tree.")]
        [Input("treeDegree", "Degree of the tree. Determines the number of children each node of the tree can have.")]
        [Input("leafSize", "Determines the number of siblings a leaf node can have.")]
        [Input("sampleSize", "The number of items used to determine how to split the collection.")]
        [Output("domainTree", "A spatial data tree containing all provided elements in its leaves.")]
        public static DomainTree<T> DomainTree<T>(this IEnumerable<T> elements, int treeDegree = 16, int leafSize = 16, int sampleSize = 60) where T : IElement
        {
            return Data.Create.DomainTree(elements, x => x.IBounds().DomainBox(), treeDegree, leafSize, sampleSize);
        }

        /***************************************************/
    }
}




