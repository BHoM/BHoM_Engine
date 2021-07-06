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

using BH.Engine.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
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

        [Description("Returns the collection of IRelations that have source and target Guids matching those provided.")]
        [Input("graph", "The Graph that owns the IRelation.")]
        [Input("source", "The IBHoMObject source to search for.")]
        [Input("target", "The IBHoMObject target to search for.")]
        [Input("relationDirection", "Optional RelationDirection used to determine the direction that relations can be traversed. Defaults to Forwards indicating traversal is from source to target.")]
        [Output("relations", "The collection of matching IRelations.")]
        [PreviousVersion("4.2", "BH.Engine.Analytical.Query.Relation(BH.oM.Analytical.Elements.Graph,BH.oM.Base.IBHoMObject,BH.oM.Base.IBHoMObject,BH.oM.Analytical.Elements.RelationDirection>")]
        public static List<IRelation<T>> Relation<T>(this Graph<T> graph, T source, T target, RelationDirection relationDirection = RelationDirection.Forwards)
            where T : IBHoMObject
        {
            if (graph == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the relations of a null graph.");
                return new List<IRelation<T>>();
            }

            if (source == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the relations of a graph when the source is null.");
                return new List<IRelation<T>>();
            }

            if (target == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the relations of a graph when the target is null.");
                return new List<IRelation<T>>();
            }

            List<IRelation<T>> relations = new List<IRelation<T>>();
            Guid sourceGuid = source.BHoM_Guid;
            Guid targetGuid = target.BHoM_Guid;
            switch (relationDirection)
            {
                case RelationDirection.Forwards:
                    relations.AddRange(graph.Relations.FindAll(x => x.Source.Equals(sourceGuid) && x.Target.Equals(targetGuid)));
                    break;
                case RelationDirection.Backwards:
                    relations.AddRange(graph.Relations.FindAll(x => x.Source.Equals(targetGuid) && x.Target.Equals(sourceGuid)));
                    break;
                case RelationDirection.Both:
                    relations.AddRange(graph.Relations.FindAll(x => x.Source.Equals(sourceGuid) && x.Target.Equals(targetGuid)));
                    relations.AddRange(graph.Relations.FindAll(x => x.Source.Equals(targetGuid) && x.Target.Equals(sourceGuid)));
                    break;
            }
            return relations;
        }

        /***************************************************/

        [Description("Returns the collection of IRelation Guids that have source and target Guids matching those provided.")]
        [Input("graph", "The Graph that owns the IRelation.")]
        [Input("source", "The Guid of the source to search for.")]
        [Input("target", "The Guid of the target to search for.")]
        [Input("relationDirection", "Optional RelationDirection used to determine the direction that relations can be traversed. Defaults to Forwards indicating traversal is from source to target.")]
        [Output("relations", "The collection of matching IRelation Guids.")]
        [PreviousVersion("4.2", "BH.Engine.Analytical.Query.Relation(BH.oM.Analytical.Elements.Graph,System.Guid,System.Guid,BH.oM.Analytical.Elements.RelationDirection>")]
        public static List<Guid> Relation<T>(this Graph<T> graph, Guid source, Guid target, RelationDirection relationDirection = RelationDirection.Forwards)
            where T : IBHoMObject
        {
            if (graph == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the relations of a null graph.");
                return new List<Guid>();
            }

            List<IRelation<T>> relations = graph.Relation(graph.Entities[source], graph.Entities[target], relationDirection);
            
            List<Guid> guids = relations.Select(rel => rel.BHoM_Guid).ToList();
            
            return guids;
        }
    }
}
