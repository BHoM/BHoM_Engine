using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Analytical.Fragments;
using BH.Engine.Analytical;
using BH.oM.Base;
using BH.oM.Diffing;
using BH.Engine.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using System.ComponentModel;
using BH.Engine.Geometry;
using BH.Engine.GraphFlow;
using BH.oM.Dimensional;
using BH.Engine.Spatial;

namespace BH.Engine.Analytical
{
    public static partial class Create
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Create a graph from a collection of IBHoMObjects with Dependency fragments and a diff config to determine the unique graph entities")]
        public static Graph Graph(List<IBHoMObject> entities, DiffConfig diffConfig = null)
        {
            return Graph(entities, new List<IRelation>(), diffConfig);
        }

        /***************************************************/
        [Description("Create a graph from a collection of IRelations and a diff config to determine the unique graph entities")]
        public static Graph Graph(List<IRelation> relations, DiffConfig diffConfig = null)
        {
            return Graph(new List<IBHoMObject>(), relations, diffConfig);
        }

        /***************************************************/
        [Description("Create a graph from a collection of IBHoMObjects with Dependency fragments, a collection of IRelations and a diff config to determine the unique graph entities")]
        public static Graph Graph(List<IBHoMObject> entities = null, List<IRelation> relations = null, DiffConfig diffConfig = null)
        {
            Graph graph = new Graph();

            //add objects from sub graphs if any
            entities.AddRange(relations.SelectMany(r => r.Subgraph.Entities.Values).ToList());

            if (entities.Count == 0)
            {
                Reflection.Compute.RecordWarning("No IBHoMObjects found");
                return graph;
            }

            m_MatchedObjects = Query.DiffEntities(entities, diffConfig);
            //Diff diff = Diffing.Compute.DiffGenericObjects(entities, entities, diffConfig, false);

            //SetMatchedObjects(diff);

            //add all provided relations to single list
            relations.AddRange(entities.ToRelation()); 
            //add to graph
            graph.Relations.AddRange(relations);

            //add unique objects
            foreach (KeyValuePair< Guid, IBHoMObject> kvp in m_MatchedObjects)
            {
 
                if (!graph.Entities.ContainsKey(kvp.Value.BHoM_Guid))
                    graph.Entities.Add(kvp.Value.BHoM_Guid, kvp.Value);
                
            }
            graph.UniqueEntities(m_MatchedObjects);

            Modify.UniqueEntityNames(graph.Entities.Values.ToList());

            return graph;
        }

        /***************************************************/

        public static Graph Graph<T>(List<ICurve> connectingCurves, IElement0D prototypeEntity, List<IElement0D> entities = null, double snappingTolerance = 1.0, RelationDirection relationDirection = RelationDirection.Forwards)
        where T : IElement0D
        {

            if (entities == null)
                entities = new List<IElement0D>();

            List<IElement0D> entitiesCloned = entities.DeepClone();

            List<IRelation> relations = new List<IRelation>();
            foreach (ICurve curve in connectingCurves)
            {
                IElement0D start = FindOrCreateEntity(entitiesCloned, curve.IStartPoint(), snappingTolerance, prototypeEntity);
                IElement0D end = FindOrCreateEntity(entitiesCloned, curve.IEndPoint(), snappingTolerance, prototypeEntity);

                SpatialRelation relation = new SpatialRelation()
                {
                    Source = ((IBHoMObject)start).BHoM_Guid,
                    Target = ((IBHoMObject)end).BHoM_Guid,
                    Curve = curve
                };
                relations.AddRange(relationsToAdd(relation, relationDirection));
            }
            Graph graph = new Graph();
            
            entitiesCloned.ForEach(n => graph.Entities.Add(((IBHoMObject)n).BHoM_Guid, ((IBHoMObject)n)));
            graph.Relations = relations;
            Analytical.Modify.UniqueEntityNames(entitiesCloned.Cast<IBHoMObject>().ToList());

            return graph;
        }
        /***************************************************/
        public static Graph Graph(int entityCount, int branching, BoundingBox boundingBox, IElement0D prototypeEntity, double tolerance = 1.0, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            Graph graph = new Graph();
            Random rnd = new Random();
            List<IElement0D> entities = new List<IElement0D>();
            for (int i = 0; i < entityCount; i++)
            {
                Point p = Geometry.Create.RandomPoint(rnd, boundingBox);
                IElement0D entity = prototypeEntity.ClonePositionGuid(p);
                
                if (!ToCloseToAny(entities, entity, tolerance))
                    entities.Add(entity);

            }
            List<IRelation> relations = new List<IRelation>();
            foreach (IElement0D entity in entities)
            {
                foreach (IElement0D d in ClosestIElement0Ds(entities, entity, branching))
                {
                    SpatialRelation relation = new SpatialRelation()
                    {
                        Source = ((IBHoMObject)entity).BHoM_Guid,
                        Target = ((IBHoMObject)d).BHoM_Guid
                    };
                    relations.AddRange(relationsToAdd(relation, relationDirection));
                }
            }

            Analytical.Modify.UniqueEntityNames(entities.Cast<IBHoMObject>().ToList());
            entities.ForEach(n => graph.Entities.Add(((IBHoMObject)n).BHoM_Guid, ((IBHoMObject)n)));
            graph.Relations = relations;

            return graph;
        }

        /***************************************************/
        public static Graph Graph<T>(int width, int length, int height, double cellsize, T prototypeEntity, RelationDirection relationDirection = RelationDirection.Forwards)
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
                        Point p = Geometry.Create.Point(i * cellsize, j * cellsize, k * cellsize);

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
                            SpatialRelation relation = new SpatialRelation()
                            {
                                Source = entityGrid[k][i][j].BHoM_Guid,
                                Target = c.BHoM_Guid
                            };
                            graph.Relations.AddRange(relationsToAdd(relation, relationDirection));
                                
                        }
                    }
                }
            }

            Analytical.Modify.UniqueEntityNames(graph.Entities.Values.ToList());
            return graph;
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/
        private static IElement0D FindOrCreateEntity(List<IElement0D> entities, Point point, double tolerance, IElement0D prototypeEntity)
        {
            IElement0D entity = entities.ClosestIElement0D(point, tolerance);

            if (entity == null || entity.IGeometry().Distance(point) > tolerance)
            {
                entity = prototypeEntity.ClonePositionGuid(point);
                entities.Add(entity);
            }
                
            return entity;
        }
        private static IElement0D ClonePositionGuid(this IElement0D element0D, Point position)
        {
            element0D = element0D.DeepClone();
            element0D = element0D.ISetGeometry(position);
            ((IBHoMObject)element0D).BHoM_Guid = Guid.NewGuid();
            return element0D;
        }
        /***************************************************/
        private static List<IRelation> relationsToAdd(IRelation relation, RelationDirection linkDirection)
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
        //private static void AutoLayout(this Graph graph)
        //{
        //    Dictionary<string, int> depths = graph.DepthDictionary();
        //    depths.Remove("root");
        //    List<int> distinctDepths = depths.Values.Distinct().ToList();
        //    distinctDepths.Sort();
        //    distinctDepths.Reverse();
        //    double x = 0;
        //    foreach (int d in distinctDepths)
        //    {
        //        //all the entities at this level
        //        IEnumerable<string> level = depths.Where(kvp => kvp.Value == d).Select(kvp => kvp.Key);
        //        double y = 0;
        //        foreach(string name in level)
        //        {
        //            INode n = graph.Entities.Find(p => p.Name == name);
        //            if (n.Location == new oM.Geometry.Point())
        //            {
        //                n.Location = Geometry.Create.Point(x, y, 0);
        //                y--;
        //            }
        //        }
        //        x++;
        //    }
        //}
        /***************************************************/

        private static void SetMatchedObjects(Diff diff)
        {
            m_MatchedObjects = new Dictionary<Guid, IBHoMObject>();
            foreach (Tuple<object, object> tuple in diff.UnchangedObjects)
            {
                if (tuple.Item1 is IBHoMObject && tuple.Item2 is IBHoMObject)
                {
                    IBHoMObject original = (IBHoMObject)tuple.Item1;
                    IBHoMObject matched = (IBHoMObject)tuple.Item2;
                    if (!m_MatchedObjects.ContainsKey(original.BHoM_Guid))
                        m_MatchedObjects.Add(original.BHoM_Guid, matched);
                }

            }

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

            int total = rnd.Next(2, neighbours.Count);
            List<IBHoMObject> wanted = new List<IBHoMObject>();
            while (wanted.Count < total)
            {
                IBHoMObject next = neighbours[rnd.Next(0, neighbours.Count)];
                if (wanted.Contains(next))
                    continue;
                wanted.Add(next);
            }
            return wanted;
        }
        /***************************************************/
        /****           Private Fields                  ****/
        /***************************************************/
        private static Random rnd = new Random();
        private static Dictionary<Guid, IBHoMObject> m_MatchedObjects = new Dictionary<Guid, IBHoMObject>();
    }
}

