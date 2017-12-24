using System;
using System.Collections.Generic;
using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public class NodeDistanceComparer : IEqualityComparer<Node>
    {
        private double m_multiplier;

        public NodeDistanceComparer()
        {
            //TODO: Grab tolerance from global tolerance settings
            m_multiplier = 1000;
        }

        public NodeDistanceComparer(int decimals)
        {
            m_multiplier = Math.Pow(10, decimals);
        }

        public bool Equals(Node node1, Node node2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(node1, node2)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(node1, null) || Object.ReferenceEquals(node2, null))
                return false;

            //Check if the GUIDs are the same
            if (node1.BHoM_Guid == node2.BHoM_Guid)
                return true;

            if ((int)Math.Round(node1.Point.X * m_multiplier) != (int)Math.Round(node2.Point.X * m_multiplier))
                return false;

            if ((int)Math.Round(node1.Point.Y * m_multiplier) != (int)Math.Round(node2.Point.Y * m_multiplier))
                return false;

            if ((int)Math.Round(node1.Point.Z * m_multiplier) != (int)Math.Round(node2.Point.Z * m_multiplier))
                return false;

            return true;
        }

        public int GetHashCode(Node node)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(node, null)) return 0;

            int x = ((int)Math.Round(node.Point.X * m_multiplier)).GetHashCode();
            int y = ((int)Math.Round(node.Point.Y * m_multiplier)).GetHashCode();
            int z = ((int)Math.Round(node.Point.Z * m_multiplier)).GetHashCode();
            return x ^ y ^ z;

        }
    }
}
