/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Modifies a Graph by removing all entities that are not used by relations.")]
        [Input("graph", "The Graph to modify.")]
        [Output("graph", "The modified Graph without isolated entities.")]
        [PreviousVersion("4.2", "BH.Engine.Analytical.Modify.RemoveIsolatedEntities(BH.oM.Analytical.Elements.Graph)")]
        public static Graph<T> RemoveIsolatedEntities<T>(this Graph<T> graph)
            where T: IBHoMObject
        {
            if (graph == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot remove isolated entities from a null graph.");
                return null;
            }

            foreach (Guid n in graph.IsolatedEntities())
                graph.Entities.Remove(n);

            return graph;
        }

        /***************************************************/
    }
}

