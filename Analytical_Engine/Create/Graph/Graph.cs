/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;

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
        public static Graph Graph(List<IBHoMObject> entities, ComparisonConfig comparisonConfig = null)
        {
            return Graph(entities, new List<IRelation>(), comparisonConfig);
        }

        /***************************************************/

        [Description("Create a graph from a collection of IRelations, property names and decimal places to determine unique graph entities.")]
        [Input("relations", "A collection of IRelations to use as Graph Relations. Relations should include sub Graphs containing the entities to be used in the Graph.")]
        [Input("comparisonConfig", "Settings to determine the uniqueness of entities.")]
        [Output("graph", "Graph.")]
        public static Graph Graph(List<IRelation> relations, ComparisonConfig comparisonConfig = null)
        {
            return Graph(new List<IBHoMObject>(), relations, comparisonConfig);
        }

        /***************************************************/

        [Description("Create a graph from a collection of IBHoMObjects, a collection of IRelations, property names and decimal places to determine unique graph entities.")]
        [Input("entities", "Optional collection of IBHoMOBjects to use as Graph entities. Entities can include DependencyFragments to determine the Graph Relations.")]
        [Input("relations", "Optional collection of IRelations to use as Graph Relations. Relations can include sub Graphs containing the entities to be used in the Graph.")]
        [Input("comparisonConfig", "Settings to determine the uniqueness of entities.")]
        [Output("graph", "Graph.")]
        public static Graph Graph(List<IBHoMObject> entities = null, List<IRelation> relations = null, ComparisonConfig comparisonConfig = null)
        {
            Graph graph = new Graph();

            List<IBHoMObject> clonedEntities = entities.DeepClone();
            List<IRelation> clonedRelations = relations.DeepClone();

            //add objects from sub graphs if any
            clonedEntities.AddRange(clonedRelations.SelectMany(r => r.Subgraph.Entities.Values.DeepClone()).ToList());

            if (clonedEntities.Count == 0)
            {
                Base.Compute.RecordWarning("No IBHoMObjects found.");
                return graph;
            }

            m_MatchedObjects = Query.UniqueEntitiesReplacementMap(clonedEntities, comparisonConfig);

            //convert dependency fragments attached to entities and add to relations
            clonedEntities.ForEach(ent => clonedRelations.AddRange(ent.ToRelation()));

            //add to graph
            graph.Relations.AddRange(clonedRelations);

            //add unique objects
            foreach (KeyValuePair<Guid, IBHoMObject> kvp in m_MatchedObjects)
            {
                if (!graph.Entities.ContainsKey(kvp.Value.BHoM_Guid))
                    graph.Entities.Add(kvp.Value.BHoM_Guid, kvp.Value);

            }
            graph = graph.UniqueEntities(m_MatchedObjects);

            graph.UniqueEntityNames();

            if (graph.Relations.Count == 0)
                Base.Compute.RecordWarning("No Relations have been defined for this graph.");

            return graph;
        }

        /***************************************************/

        [Description("Create a graph from a collection of ICurves.")]
        [Input("connectingCurves", "A collection of ICurve representing the Graph Relations.")]
        [Input("prototypeEntity", "An IElement0D to be used as the prototype of all entities in the Graph.")]
        [Input("entities", "Optional collection of IBHoMObjects to use as Graph entities.")]
        [Input("snappingTolerance", "Optional tolerance used when comparing connectingCurves end points and provided entities. Default is Tolerance.Distance (1e-6).")]
        [Input("relationDirection", "Optional RelationDirection used to determine the direction that relations can be traversed. Defaults to Forward indicating traversal is from source to target.")]
        [Output("graph", "Graph.")]
        public static Graph Graph<T>(List<ICurve> connectingCurves, T prototypeEntity, List<IElement0D> entities = null, double snappingTolerance = Tolerance.Distance, RelationDirection relationDirection = RelationDirection.Forwards)
        where T : IElement0D
        {
            if (entities == null)
                entities = new List<IElement0D>();

            if (!entities.All(e => e is IBHoMObject))
            {
                Base.Compute.RecordError("All entities must be of type IBHoMObject and IGeometry.");
                return null;
            }

            List<IElement0D> entitiesCloned = entities.DeepClone();

            List<IRelation> relations = new List<IRelation>();
            foreach (ICurve curve in connectingCurves)
            {
                IElement0D start = FindOrCreateEntity(entitiesCloned, curve.IStartPoint(), snappingTolerance, prototypeEntity);
                IElement0D end = FindOrCreateEntity(entitiesCloned, curve.IEndPoint(), snappingTolerance, prototypeEntity);

                Relation relation = new Relation()
                {
                    Source = ((IBHoMObject)start).BHoM_Guid,
                    Target = ((IBHoMObject)end).BHoM_Guid,
                    Curve = curve
                };
                relations.AddRange(RelationsToAdd(relation, relationDirection));
            }

            Graph graph = new Graph();

            entitiesCloned.ForEach(n =>
            {
                if (!graph.Entities.ContainsKey(((IBHoMObject)n).BHoM_Guid))
                {
                    graph.Entities.Add(((IBHoMObject)n).BHoM_Guid, ((IBHoMObject)n));
                }

            });

            graph.Relations = relations;
            graph.UniqueEntityNames();

            return graph;
        }

        /***************************************************/

        [Description("Create a random graph from a randomly generated entities within a BoundingBox.")]
        [Input("entityCount", "Total number of entities.")]
        [Input("branching", "Total number of Relations between an entity and its closest neighbours.")]
        [Input("boundingBox", "BoundingBox defining the spatial limits of the Graph.")]
        [Input("prototypeEntity", "An IElement0D to be used as the prototype of all entities in the Graph.")]
        [Input("tolerance", "Optional minimum distance permitted between randomly generated entities. Default is Tolerance.Distance (1e-6).")]
        [Input("relationDirection", "Optional RelationDirection used to determine the direction that relations can be traversed. Defaults to Forward indicating traversal is from source to target.")]
        [Output("graph", "Graph.")]
        public static Graph Graph(int entityCount, int branching, BoundingBox boundingBox, IElement0D prototypeEntity, double tolerance = Tolerance.Distance, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            Graph graph = new Graph();
            List<IElement0D> entities = new List<IElement0D>();

            for (int i = 0; i < entityCount; i++)
            {
                Point p = Geometry.Create.RandomPoint(m_Rnd, boundingBox);
                IElement0D entity = prototypeEntity.ClonePositionGuid(p);

                if (!ToCloseToAny(entities, entity, tolerance))
                    entities.Add(entity);

            }

            List<IRelation> relations = new List<IRelation>();
            foreach (IElement0D entity in entities)
            {
                foreach (IElement0D d in ClosestIElement0Ds(entities, entity, branching))
                {
                    Relation relation = new Relation()
                    {
                        Source = ((IBHoMObject)entity).BHoM_Guid,
                        Target = ((IBHoMObject)d).BHoM_Guid
                    };
                    relations.AddRange(RelationsToAdd(relation, relationDirection));
                }
            }

            graph.UniqueEntityNames();
            entities.ForEach(n => graph.Entities.Add(((IBHoMObject)n).BHoM_Guid, ((IBHoMObject)n)));
            graph.Relations = relations;

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
        public static Graph Graph<T>(int width, int length, int height, double cellSize, T prototypeEntity, RelationDirection relationDirection = RelationDirection.Forwards)
            where T : IElement0D
        {
            Graph graph = new Graph();
            List<List<List<IBHoMObject>>> entityGrid = new List<List<List<IBHoMObject>>>();
            for (int k = 0; k < height; k++)
            {
                List<List<IBHoMObject>> level = new List<List<IBHoMObject>>();
                for (int i = 0; i < width; i++)
                {
                    List<IBHoMObject> col = new List<IBHoMObject>();
                    for (int j = 0; j < length; j++)
                    {
                        Point p = Geometry.Create.Point(i * cellSize, j * cellSize, k * cellSize);

                        IElement0D entity = prototypeEntity.DeepClone();
                        entity = entity.ISetGeometry(p);
                        ((IBHoMObject)entity).BHoM_Guid = Guid.NewGuid();

                        graph.Entities.Add(((IBHoMObject)entity).BHoM_Guid, ((IBHoMObject)entity));

                        col.Add((IBHoMObject)entity);
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

                        List<IBHoMObject> connections = RandomNeighbours(entityGrid, i, j, k);
                        foreach (IBHoMObject c in connections)
                        {
                            Relation relation = new Relation()
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
        private static IElement0D FindOrCreateEntity(List<IElement0D> entities, Point point, double tolerance, IElement0D prototypeEntity)
        {
            IElement0D entity = entities.ClosestIElement0D(point);

            if (entity == null || entity.IGeometry().Distance(point) > tolerance)
            {
                entity = prototypeEntity.ClonePositionGuid(point);
                entities.Add(entity);
            }

            return entity;
        }

        /***************************************************/

        private static IElement0D ClonePositionGuid(this IElement0D element0D, Point position)
        {
            element0D = element0D.DeepClone();
            element0D = element0D.ISetGeometry(position);
            ((IBHoMObject)element0D).BHoM_Guid = Guid.NewGuid();
            return element0D;
        }

        /***************************************************/

        private static List<IRelation> RelationsToAdd(IRelation relation, RelationDirection linkDirection)
        {
            List<IRelation> relations = new List<IRelation>();
            if (linkDirection == RelationDirection.Forwards)
                relations.Add(relation);

            if (linkDirection == RelationDirection.Backwards)
                relations.Add(relation.IReverse());

            if (linkDirection == RelationDirection.Both)
            {
                relations.Add(relation);
                IRelation clone = relation.DeepClone();
                relations.Add(clone.IReverse());
            }
            return relations;
        }

        /***************************************************/

        private static bool ToCloseToAny(List<IElement0D> entities, IElement0D entity, double tolerance)
        {
            foreach (IElement0D n in entities)
            {
                double d = n.IGeometry().Distance(entity.IGeometry());
                if (d < tolerance)
                    return true;
            }
            return false;
        }

        /***************************************************/

        private static List<IElement0D> ClosestIElement0Ds(List<IElement0D> entities, IElement0D element0D, int branching)
        {

            List<IElement0D> ordered = entities.OrderBy(n => n.IGeometry().Distance(element0D.IGeometry())).ToList();

            return ordered.GetRange(1, branching);
        }

        /***************************************************/
        private static List<IBHoMObject> RandomNeighbours(List<List<List<IBHoMObject>>> entities, int i, int j, int k)
        {
            //from Von Neumann neighborhood randomly select 2 to all neighbours
            List<IBHoMObject> neighbours = new List<IBHoMObject>();
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
            List<IBHoMObject> wanted = new List<IBHoMObject>();
            while (wanted.Count < total)
            {
                IBHoMObject next = neighbours[m_Rnd.Next(0, neighbours.Count)];
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
        private static Dictionary<Guid, IBHoMObject> m_MatchedObjects = new Dictionary<Guid, IBHoMObject>();
    }
}




