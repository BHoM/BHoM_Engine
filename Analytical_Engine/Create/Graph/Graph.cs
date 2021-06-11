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
using BH.oM.Analytical.Fragments;
using BH.Engine.Analytical;
using BH.oM.Base;
using BH.oM.Diffing;
using BH.oM.Geometry;
using BH.Engine.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.Engine.Geometry;
using BH.oM.Dimensional;
using BH.Engine.Spatial;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Analytical
{
    public static partial class Create
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Create a graph from a collection of IBHoMObjects, property names and decimal places to determine unique graph entities.")]
        [Input("entities", "A collection of IBHoMOBjects to use as Graph entities. Entities should include DependencyFragments to determine the Graph Relations.")]
        [Input("comparisonConfig", "Settings to determine the uniqueness of entities.")]
        [Output("graph", "Graph.")]
        public static Graph<T> Graph<T>(List<T> entities, ComparisonConfig comparisonConfig = null)
            where T : IBHoMObject
        {
            return Graph(entities, new List<IRelation<T>>(), comparisonConfig);
        }

        /***************************************************/

        [Description("Create a graph from a collection of IRelations, property names and decimal places to determine unique graph entities.")]
        [Input("relations", "A collection of IRelations to use as Graph Relations. Relations should include sub Graphs containing the entities to be used in the Graph.")]
        [Input("comparisonConfig", "Settings to determine the uniqueness of entities.")]
        [Output("graph", "Graph.")]
        public static Graph<T> Graph<T>(List<IRelation<T>> relations, ComparisonConfig comparisonConfig = null)
            where T : IBHoMObject
        {
            return Graph(new List<T>(), relations, comparisonConfig);
        }

        /***************************************************/
        
        [Description("Create a graph from a collection of IBHoMObjects, a collection of IRelations, property names and decimal places to determine unique graph entities.")]
        [Input("entities", "Optional collection of IBHoMOBjects to use as Graph entities. Entities can include DependencyFragments to determine the Graph Relations.")]
        [Input("relations", "Optional collection of IRelations to use as Graph Relations. Relations can include sub Graphs containing the entities to be used in the Graph.")]
        [Input("comparisonConfig", "Settings to determine the uniqueness of entities.")]
        [Output("graph", "Graph.")]
        public static Graph<T> Graph<T>(List<T> entities = null, List<IRelation<T>> relations = null, ComparisonConfig comparisonConfig = null)
            where T : IBHoMObject
        {
            Graph<T> graph = new Graph<T>();

            List<T> clonedEntities = entities.DeepClone();
            List<IRelation<T>> clonedRelations = relations.DeepClone();

            //add objects from sub graphs if any
            clonedEntities.AddRange(clonedRelations.SelectMany(r => r.Subgraph.Entities.Values.DeepClone()).ToList());

            if (clonedEntities.Count == 0)
            {
                Reflection.Compute.RecordWarning("No IBHoMObjects found.");
                return graph;
            }

            Dictionary<Guid, T> matchedEntities = Query.UniqueEntitiesReplacementMap(clonedEntities, comparisonConfig);

            //convert dependency fragments attached to entities and add to relations
            clonedEntities.ForEach(ent => clonedRelations.AddRange(ent.ToRelation())); 

            //add to graph
            graph.Relations.AddRange(clonedRelations);

            //add unique objects
            foreach (KeyValuePair< Guid, T> kvp in matchedEntities)
            {
 
                if (!graph.Entities.ContainsKey(kvp.Value.BHoM_Guid))
                    graph.Entities.Add(kvp.Value.BHoM_Guid, kvp.Value);
                
            }
            graph = graph.UniqueEntities(matchedEntities);

            graph.UniqueEntityNames();

            if(graph.Relations.Count == 0)
                Reflection.Compute.RecordWarning("No Relations have been defined for this graph.");

            return graph;
        }

        /***************************************************/

        [Description("Create a graph from a collection of ICurves.")]
        [Input("connectingCurves", "A collection of ICurve representing the Graph Relations.")]
        [Input("prototypeEntity", "An INode to be used as the prototype of all entities in the Graph.")]
        [Input("entities", "Optional collection of IBHoMObjects to use as Graph entities.")]
        [Input("snappingTolerance", "Optional tolerance used when comparing connectingCurves end points and provided entities. Default is Tolerance.Distance (1e-6).")]
        [Input("relationDirection", "Optional RelationDirection used to determine the direction that relations can be traversed. Defaults to Forward indicating traversal is from source to target.")]
        [Output("graph", "Graph.")]
        public static Graph<T> Graph<T>(List<ICurve> connectingCurves, T prototypeEntity, List<T> entities = null, double snappingTolerance = Tolerance.Distance, RelationDirection relationDirection = RelationDirection.Forwards)
        where T : INode
        {
            if (entities == null)
                entities = new List<T>();

            List<T> entitiesCloned = entities.DeepClone();

            List<IRelation<T>> relations = new List<IRelation<T>>();
            foreach (ICurve curve in connectingCurves)
            {
                T start = FindOrCreateEntity(entitiesCloned, curve.IStartPoint(), snappingTolerance, prototypeEntity);
                T end = FindOrCreateEntity(entitiesCloned, curve.IEndPoint(), snappingTolerance, prototypeEntity);

                Relation<T> relation = new Relation<T>()
                {
                    Source = start.BHoM_Guid,
                    Target = end.BHoM_Guid,
                    Curve = curve
                };
                relations.AddRange(RelationsToAdd(relation, relationDirection));
            }
            Graph<T> graph = new Graph<T>();
            
            entitiesCloned.ForEach(n => graph.Entities.Add(n.BHoM_Guid, n));
            graph.Relations = relations;
            graph.UniqueEntityNames();

            return graph;
        }

        /***************************************************/

        [Description("Create a random graph from a randomly generated entities within a BoundingBox.")]
        [Input("entityCount", "Total number of entities.")]
        [Input("branching", "Total number of Relations between an entity and its closest neighbours.")]
        [Input("boundingBox", "BoundingBox defining the spatial limits of the Graph.")]
        [Input("prototypeEntity", "An INode to be used as the prototype of all entities in the Graph.")]
        [Input("tolerance", "Optional minimum distance permitted between randomly generated entities. Default is Tolerance.Distance (1e-6).")]
        [Input("relationDirection", "Optional RelationDirection used to determine the direction that relations can be traversed. Defaults to Forward indicating traversal is from source to target.")]
        [Output("graph", "Graph.")]
        public static Graph<T> Graph<T>(int entityCount, int branching, BoundingBox boundingBox, T prototypeEntity, double tolerance = Tolerance.Distance, RelationDirection relationDirection = RelationDirection.Forwards)
            where T : INode
        {
            Graph<T> graph = new Graph<T>();
            List<T> entities = new List<T>();

            for (int i = 0; i < entityCount; i++)
            {
                Point p = Geometry.Create.RandomPoint(m_Rnd, boundingBox);
                T entity = prototypeEntity.ClonePositionGuid(p);
                
                if (!ToCloseToAny(entities, entity, tolerance))
                    entities.Add(entity);

            }

            List<IRelation<T>> relations = new List<IRelation<T>>();
            foreach (T entity in entities)
            {
                foreach (T d in ClosestINodes(entities, entity, branching))
                {
                    Relation<T> relation = new Relation<T>()
                    {
                        Source = entity.BHoM_Guid,
                        Target = d.BHoM_Guid
                    };
                    relations.AddRange(RelationsToAdd(relation, relationDirection));
                }
            }

            entities.ForEach(n => graph.Entities.Add(n.BHoM_Guid, n));
            graph.Relations = relations;
            graph.UniqueEntityNames();
            
            return graph;
        }

        /***************************************************/

        [Description("Create a random graph within an orthogonal three-dimensional grid of Points.")]
        [Input("width", "Number of Points in the X direction.")]
        [Input("length", "Number of Points in the Y direction.")]
        [Input("height", "Number of Points in the Z direction.")]
        [Input("cellSize", "Distance between points in X, Y and Z directions.")]
        [Input("prototypeEntity", "An IElement0D to be used as the prototype of all entities in the Graph.")]
        [Input("relationDirection", "Optional RelationDirection used to determine the direction that relations can be traversed. Defaults to Forward indicating traversal is from source to target.")]
        [Output("graph", "Graph.")]
        public static Graph<T> Graph<T>(int width, int length, int height, double cellSize, T prototypeEntity, RelationDirection relationDirection = RelationDirection.Forwards)
            where T : INode
        {
            Graph<T> graph = new Graph<T>();
            List<List<List<T>>> entityGrid = new List<List<List<T>>>();
            for (int k = 0; k < height; k++)
            {
                List<List<T>> level = new List<List<T>>();
                for (int i = 0; i < width; i++)
                {
                    List<T> col = new List<T>();
                    for (int j = 0; j < length; j++)
                    {
                        Point p = Geometry.Create.Point(i * cellSize, j * cellSize, k * cellSize);

                        T entity = prototypeEntity.DeepClone();
                        entity.Position = p;
                        entity.BHoM_Guid = Guid.NewGuid();

                        graph.Entities.Add(entity.BHoM_Guid, entity);

                        col.Add(entity);
                    }
                    level.Add(col);
                }
                entityGrid.Add(level);
            }
            for (int k = 0; k < height; k++)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < length; j++)
                    {

                        List<T> connections = RandomNeighbours(entityGrid, i, j, k);
                        foreach (T c in connections)
                        {
                            Relation<T> relation = new Relation<T>()
                            {
                                Source = entityGrid[k][i][j].BHoM_Guid,
                                Target = c.BHoM_Guid
                            };
                            graph.Relations.AddRange(RelationsToAdd(relation, relationDirection));
                                
                        }
                    }
                }
            }

            graph.UniqueEntityNames();
            return graph;
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/
        private static T FindOrCreateEntity<T>(List<T> entities, Point point, double tolerance, T prototypeEntity)
            where T : INode
        {
            
            T entity = entities.ClosestINode(point);

            if (entity == null || entity.Position.Distance(point) > tolerance)
            {
                entity = prototypeEntity.ClonePositionGuid(point);
                entities.Add(entity);
            }
                
            return entity;
        }

        /***************************************************/

        private static T ClonePositionGuid<T>(this T node, Point position)
            where T : INode
        {
            node = node.DeepClone();
            node.Position = position;
            node.BHoM_Guid = Guid.NewGuid();
            return node;
        }

        /***************************************************/

        private static List<IRelation<T>> RelationsToAdd<T>(IRelation<T> relation, RelationDirection linkDirection)
            where T : IBHoMObject
        {
            List<IRelation<T>> relations = new List<IRelation<T>>();
            if (linkDirection == RelationDirection.Forwards)
                relations.Add(relation);

            if (linkDirection == RelationDirection.Backwards)
                relations.Add(relation.IReverse());

            if (linkDirection == RelationDirection.Both)
            {
                relations.Add(relation);
                IRelation<T> clone = relation.DeepClone();
                relations.Add(clone.IReverse());
            }
            return relations;
        }

        /***************************************************/

        private static bool ToCloseToAny<T>(List<T> entities, T entity, double tolerance)
            where T : INode
        {
            foreach (T n in entities)
            {
                double d = n.Position.Distance(entity.Position);
                if (d < tolerance)
                    return true;
            }
            return false;
        }

        /***************************************************/

        private static List<T> ClosestINodes<T>(List<T> entities, T entity, int branching)
            where T : INode
        {

            List<T> ordered = entities.OrderBy(n => n.Position.Distance(entity.Position)).ToList();

            return ordered.GetRange(1, branching);
        }

        /***************************************************/
        private static List<T> RandomNeighbours<T>(List<List<List<T>>> entities, int i, int j, int k)
            where T : INode
        {
            //from Von Neumann neighborhood randomly select 2 to all neighbours
            List<T> neighbours = new List<T>();
            int left = i - 1;
            int right = i + 1;
            int infront = j + 1;
            int behind = j - 1;
            int below = k - 1;
            int above = k + 1;
            if (left >= 0)
                neighbours.Add(entities[k][left][j]);

            if (right <= entities[0].Count - 1)
                neighbours.Add(entities[k][right][j]);

            if (behind >= 0)
                neighbours.Add(entities[k][i][behind]);

            if (infront <= entities[0][0].Count - 1)
                neighbours.Add(entities[k][i][infront]);

            if (below >= 0)
                neighbours.Add(entities[below][i][j]);

            if (above <= entities.Count - 1)
                neighbours.Add(entities[above][i][j]);

            if (neighbours.Count <= 2)
                return neighbours;

            int total = m_Rnd.Next(2, neighbours.Count);
            List<T> wanted = new List<T>();
            while (wanted.Count < total)
            {
                T next = neighbours[m_Rnd.Next(0, neighbours.Count)];
                if (wanted.Contains(next))
                    continue;
                wanted.Add(next);
            }
            return wanted;
        }

        /***************************************************/
        /****           Private Fields                  ****/
        /***************************************************/

        private static Random m_Rnd = new Random();
        
    }
}


