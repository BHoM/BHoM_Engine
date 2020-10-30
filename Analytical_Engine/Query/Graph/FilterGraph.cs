/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Analytical.Fragments;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Filter a Graph.")]
        [Input("graph", "The Graph to filter.")]
        [Input("ignoreClusters", "The clusters to ignore. Entities in these clusters and the relations defined by them will be removed")]
        [Output("filtered graph", "The filtered Graph.")]

        public static Graph FilterGraph(this Graph graph, List<string> ignoreClusters = null)
        {
            Graph filteredGraph = graph.DeepClone();

            if (ignoreClusters.Count() == 0)
                return filteredGraph;

            foreach (IBHoMObject entity in graph.Entities.Values.ToList())
            {
                EntityViewFragment viewFragment = entity.FindFragment<EntityViewFragment>();
                if(viewFragment!=null)
                {
                    if (ignoreClusters.Contains(viewFragment.ClusterName))
                    {
                        filteredGraph.RemoveEntity(entity.BHoM_Guid);
                    }
                        
                }
            }

            return filteredGraph;
        }
    }
}
