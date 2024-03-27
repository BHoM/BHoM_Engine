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

using BH.Engine.Geometry;
using BH.oM.Analytical.Graph;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
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
        public static List<IRelation> Relation(this Graph graph, IBHoMObject source, IBHoMObject target, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            if (graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the relations of a null graph.");
                return new List<IRelation>();
            }

            if (source == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the relations of a graph when the source is null.");
                return new List<IRelation>();
            }

            if (target == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the relations of a graph when the target is null.");
                return new List<IRelation>();
            }

            List<IRelation> relations = new List<IRelation>();
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
        public static List<Guid> Relation(this Graph graph, Guid source, Guid target, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            if (graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the relations of a null graph.");
                return new List<Guid>();
            }

            List<IRelation> relations = graph.Relation(graph.Entities[source], graph.Entities[target], relationDirection);
            
            List<Guid> guids = relations.Select(rel => rel.BHoM_Guid).ToList();
            
            return guids;
        }
    }
}



