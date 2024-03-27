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
using BH.oM.Dimensional;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Filter entities from a Graph.")]
        [Input("graph", "The Graph to filter the entities from.")]
        [Input("typeFilter", "The Type of the entities to filter.")]
        [Output("filter entities", "Entity Dictionary containing the filtered entities.")]
        public static Dictionary<Guid, IBHoMObject> FilterEntities(this Graph graph, Type typeFilter)
        {
            if (graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot filter the entities of a null graph.");
                return new Dictionary<Guid, IBHoMObject>();
            }

            Dictionary<Guid, IBHoMObject> entityDict = new Dictionary<Guid, IBHoMObject>();

            List<IBHoMObject> filtered = graph.Entities().Where(x => typeFilter.IsAssignableFrom(x.GetType())).ToList();
            filtered.ForEach(obj => entityDict.Add(obj.BHoM_Guid, obj));

            return entityDict;
        }

        /***************************************************/
    }
}



