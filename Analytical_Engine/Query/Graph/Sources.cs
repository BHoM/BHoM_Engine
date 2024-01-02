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

using BH.oM.Analytical.Elements;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the collection of entity Guids that are never used as Relation targets.")]
        [Input("graph", "The Graph to search.")]
        [Output("sources", "The collection of entity Guids that are sources.")]
        public static List<Guid> Sources(this Graph graph)
        {
            if(graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the sources of a null graph.");
                return new List<Guid>();
            }

            //entity is a source if it never appears as target
            List<Guid> targets = graph.Relations.Select(x => x.Target).Distinct().ToList();
            List<Guid> sources = graph.Entities.Keys.ToList().Except(targets).ToList();
            return sources;
        }
    }
}




