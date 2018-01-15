using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;

namespace BH.oM.Structural
{
    public static class XNode
    {
        public static List<Node> MergeWithin(List<Node> nodes, double tolerance)
        {
            int removed = 0;
            List<Node> result = new List<Node>();
            nodes.Sort(delegate (Node n1, Node n2)
            {
                return n1.Point.DistanceTo(BH.oM.Geometry.Point.Origin).CompareTo(n2.Point.DistanceTo(BH.oM.Geometry.Point.Origin));
            });
            result.AddRange(nodes);

            for (int i = 0; i < nodes.Count; i++)
            {
                double distance = nodes[i].Point.DistanceTo(BH.oM.Geometry.Point.Origin);
                int j = i + 1;
                while (j < nodes.Count && Math.Abs(nodes[j].Point.DistanceTo(BH.oM.Geometry.Point.Origin) - distance) < tolerance)
                {
                    if (nodes[i].Point.DistanceTo(nodes[j].Point) < tolerance)
                    {
                        nodes[j] = nodes[j].Merge(nodes[i]);
                        result.RemoveAt(i - removed++);
                        break;
                    }
                    j++;
                }
            }
            return result;
        }
    }
}
