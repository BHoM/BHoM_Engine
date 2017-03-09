using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using BHoM.Generic;

namespace Graph_Engine
{
    

    /***************************************************************************/

    public class GraphNavigator<T>
    {
        public Graph<T> Graph { get; private set; }

        public GraphNavigator(Graph<T> graph)
        {
            Graph = graph;
        }

        public List<GraphNode<T>> GetPath(GraphNode<T> startNode, GraphNode<T> endNode, Func<T, T, double> costHeuristic)
        {
            // Create the inital close set
            HashSet<GraphNode<T>> closedSet = new HashSet<GraphNode<T>>();

            // Create initial set of boundary nodes and add start node to it
            SortedSet<PathItem<T>> openSet = new SortedSet<PathItem<T>>();
            openSet.Add(new PathItem<T>(startNode, 0, costHeuristic(startNode.Value, endNode.Value)));

            int iter = 0;
            while( openSet.Count > 0)
            {
                PathItem<T> currentItem = openSet.First();
                if (currentItem.Node == endNode || iter++ > 1000)
                    return ReconstructPath(currentItem);

                openSet.Remove(currentItem);
                closedSet.Add(currentItem.Node);

                foreach (GraphLink<T> link in currentItem.Node.Links)
                {
                    GraphNode<T> neighbour = link.EndNode;
                    if (closedSet.Contains(neighbour))
                        continue;

                    double tentativeCost = currentItem.StartCost + link.Weight;
                    PathItem<T> item = openSet.FirstOrDefault(x => x.Node == neighbour);
                    if (item == null)
                    {
                        PathItem<T> newItem = new PathItem<T>(neighbour, tentativeCost, costHeuristic(neighbour.Value, endNode.Value));
                        newItem.Previous = currentItem;
                        openSet.Add(newItem);
                    }     
                    else if (tentativeCost < item.StartCost)
                    {
                        openSet.Remove(item);
                        item.Previous = currentItem;
                        item.StartCost = tentativeCost;
                        openSet.Add(item);
                    }
                }
            }

            return null;
        }

        private List<GraphNode<T>> ReconstructPath(PathItem<T> endItem)
        {
            List<GraphNode<T>> path = new List<GraphNode<T>>();
            path.Add(endItem.Node);

            while (endItem.Previous != null)
            {
                endItem = endItem.Previous;
                path.Add(endItem.Node);
            }
            path.Reverse();

            return path;
        }

        private class PathItem<T> : IComparable<PathItem<T>>
        {
            public double StartCost { get; set; }    // Cost of going from start to that node
            public double EndCost { get; set; }     // estimated cost of going from this node to the end
            public GraphNode<T> Node { get; set; }
            public PathItem<T> Previous { get; set; }
            public bool IsValid { get { return Node != null; } }

            public double Score { get { return StartCost + EndCost; } } 

            public PathItem()
            {
                Node = null;
            }

            public PathItem(GraphNode<T> node, double startCost = 0, double endCost = 0)
            {
                Node = node;
                StartCost = startCost;
                EndCost = endCost;
                Previous = null;
            }

            int IComparable<PathItem<T>>.CompareTo(PathItem<T> other)
            {
                return Score.CompareTo(other.Score);
            }
        }
    }
}
