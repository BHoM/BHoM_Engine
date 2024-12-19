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
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Analytical.Elements;
using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
 
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Creates adjacency ID from adjacency elements.")]
        [Input("edges", "Adjacency edges.")]
        [Input("elems", "Adjacency elements.")]
        [Output("adjacencyID", "The generated name of the adjacency.")]
        public static string AdjacencyID(this List<IElement1D> edges, List<IElement2D> elems)
        {
            string separator = "_";
            List<string> adjIDs = new List<string>();
            if (edges.Count != elems.Count)
            {
                Base.Compute.RecordWarning("Edge and element list lengths do not match. Each edge should have a corresponding element, please check your inputs.");
                return null;
            }
            else
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    IElement1D edge = edges[i];
                    IElement2D elem = elems[i];
                    string adjID = "Elem:" + elem.IPrimaryPropertyName() + " " + "Edge:" + edge.IPrimaryPropertyName();
                    adjIDs.Add(adjID);
                }
            }
            adjIDs.Sort();
            return string.Join(separator, adjIDs);    
        }


    }
}






