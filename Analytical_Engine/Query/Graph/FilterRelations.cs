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

using BH.oM.Analytical.Graph;
using BH.oM.Base;
using BH.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Filter relations from a Graph.")]
        [Input("graph", "The Graph to filter the relations from.")]
        [Input("typeFilter", "The Type of the relation to filter.")]
        [Output("filtered relations", "Collection of IRelations filtered from the Graph.")]
        public static List<IRelation> FilterRelations(this Graph graph, Type typeFilter)
        {
            if (graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot filter the relations of a null graph.");
                return new List<IRelation>();
            }

            return graph.Relations.Where(x => typeFilter.IsAssignableFrom(x.GetType())).ToList(); 
        }
    }
}



