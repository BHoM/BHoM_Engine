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
            return Graph(entities, null, diffConfig);
        }

        /***************************************************/
        [Description("Create a graph from a collection of IRelations and a diff config to determine the unique graph entities")]
        public static Graph Graph(List<IRelation> relations, DiffConfig diffConfig = null)
        {
            return Graph(null, relations, diffConfig);
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
                
            Diff diff = Diffing.Compute.DiffGenericObjects(entities, entities, diffConfig, false);

            SetMatchedObjects(diff);

            //add all provided relations to single list
            relations.AddRange(entities.ToRelation()); 
            //add to graph
            graph.Relations.AddRange(relations);

            foreach (Tuple< object,object> tuple in diff.UnchangedObjects)
            {
                if(tuple.Item2 is IBHoMObject)
                {
                    IBHoMObject bhomObject = (IBHoMObject)tuple.Item2;
                    if (!graph.Entities.ContainsKey(bhomObject.BHoM_Guid))
                        graph.Entities.Add(bhomObject.BHoM_Guid, bhomObject);
                }
            }
            graph.UniqueEntities(m_MatchedObjects);

            Modify.UniqueEntityNames(graph.Entities.Values.ToList());

            return graph;
        }

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

        public static Graph Graph(List<ICurve> connectingCurves,INode prototypeEntity, List<INode> entities = null, double snappingTolerance = 1.0, RelationDirection relationDirection = RelationDirection.Forwards)
        {

            if (entities == null)
                entities = new List<INode>();

            List<INode> entitiesCloned = entities.DeepClone();

            List<IRelation> relations = new List<IRelation>();
            foreach (ICurve curve in connectingCurves)
            {
                INode start = FindOrCreateINode(entitiesCloned, curve.IStartPoint(), snappingTolerance, prototypeEntity);
                INode end = FindOrCreateINode(entitiesCloned, curve.IEndPoint(), snappingTolerance, prototypeEntity);

                SpatialRelation relation = new SpatialRelation()
                {
                    Source = start.BHoM_Guid,
                    Target = end.BHoM_Guid,
                    Curve = curve
                };
                relations.AddRange(relationsToAdd(relation, relationDirection));
            }
            Graph graph = new Graph();
            
            entitiesCloned.ForEach(n => graph.Entities.Add(n.BHoM_Guid, n));
            graph.Relations = relations;
            Analytical.Modify.UniqueEntityNames(entitiesCloned.Cast<IBHoMObject>().ToList());

            return graph;
        }
        /***************************************************/
        public static Graph Graph(int nodeCount, int branching, BoundingBox boundingBox, INode prototypeEntity, double tolerance = 1.0, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            Graph graph = new Graph();
            Random rnd = new Random();
            List<INode> entities = new List<INode>();
            for (int i = 0; i < nodeCount; i++)
            {
                Point p = Geometry.Create.RandomPoint(rnd, boundingBox);
                INode n = prototypeEntity.DeepClone();
                n.BHoM_Guid = Guid.NewGuid();
                n.Position = p;

                if (!ToCloseToAny(entities, n, tolerance))
                    entities.Add(n);

            }
            List<IRelation> relations = new List<IRelation>();
            foreach (INode node in entities)
            {
                foreach (INode d in ClosestINodes(entities, node, branching))
                {
                    Relation relation = new Relation()
                    {
                        Source = node.BHoM_Guid,
                        Target = d.BHoM_Guid
                    };
                    relations.AddRange(relationsToAdd(relation, relationDirection));
                }
            }

            Analytical.Modify.UniqueEntityNames(entities.Cast<IBHoMObject>().ToList());
            entities.ForEach(n => graph.Entities.Add(n.BHoM_Guid, n));
            graph.Relations = relations;

            return graph;
        }

        /***************************************************/
        public static Graph Graph(int width, int length, int height, double cellsize, INode prototypeEntity, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            Graph graph = new Graph();
            List<List<List<INode>>> nodeGrid = new List<List<List<INode>>>();
            for (int k = 0; k < height; k++)
            {
                List<List<INode>> level = new List<List<INode>>();
                for (int i = 0; i < width; i++)
                {
                    List<INode> col = new List<INode>();
                    for (int j = 0; j < length; j++)
                    {
                        Point p = Geometry.Create.Point(i * cellsize, j * cellsize, k * cellsize);
                        INode n = prototypeEntity.DeepClone();
                        n.BHoM_Guid = Guid.NewGuid();
                        n.Position = p;
                        graph.Entities.Add(n.BHoM_Guid, n);

                        col.Add(n);
                    }
                    level.Add(col);
                }
                nodeGrid.Add(level);
            }
            for (int k = 0; k < height; k++)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < length; j++)
                    {

                        List<INode> connections = RandomNeighbours(nodeGrid, i, j, k);
                        foreach (INode c in connections)
                        {
                            Relation relation = new Relation()
                            {
                                Source = nodeGrid[k][i][j].BHoM_Guid,
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
        private static INode FindOrCreateINode(List<INode> entities, Point point, double tolerance,INode prototypeEntity)
        {
            INode node = entities.ClosestINode(point, tolerance);
            if (node == null || node.Position.Distance(point) > tolerance)
            {
                node = prototypeEntity.DeepClone();
                node.BHoM_Guid = Guid.NewGuid();
                node.Position = point;
                entities.Add(node);
            }
            return node;
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
        private static bool ToCloseToAny(List<INode> entities, INode node, double tolerance)
        {
            foreach (INode n in entities)
            {
                double d = n.Position.Distance(node.Position);
                if (d < tolerance)
                    return true;
            }
            return false;
        }
        /***************************************************/
        private static List<INode> ClosestINodes(List<INode> entities, INode node, int branching)
        {

            List<INode> ordered = entities.OrderBy(n => n.Position.Distance(node.Position)).ToList();

            return ordered.GetRange(1, branching);
        }
        /***************************************************/
        private static List<INode> RandomNeighbours(List<List<List<INode>>> entities, int i, int j, int k)
        {
            //from Von Neumann neighborhood randomly select 2 to all neighbours
            List<INode> neighbours = new List<INode>();
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
            List<INode> wanted = new List<INode>();
            while (wanted.Count < total)
            {
                INode next = neighbours[rnd.Next(0, neighbours.Count)];
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

