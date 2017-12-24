using System;
using System.Collections.Generic;
using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public class BarEndNodesDistanceComparer : IEqualityComparer<Bar>
    {
        private NodeDistanceComparer m_nodeComparer;

        public BarEndNodesDistanceComparer()
        {
            m_nodeComparer = new NodeDistanceComparer();
        }

        public BarEndNodesDistanceComparer(int decimals)
        {
            m_nodeComparer = new NodeDistanceComparer(decimals);
        }

        public bool Equals(Bar bar1, Bar bar2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(bar1, bar2)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(bar1, null) || Object.ReferenceEquals(bar2, null))
                return false;

            //Check if the GUIDs are the same
            if (bar1.BHoM_Guid == bar2.BHoM_Guid)
                return true;

            if (m_nodeComparer.Equals(bar1.StartNode, bar2.StartNode))
            {
                return m_nodeComparer.Equals(bar1.EndNode, bar2.EndNode);
            }
            else if (m_nodeComparer.Equals(bar1.StartNode, bar2.EndNode))
            {
                return m_nodeComparer.Equals(bar1.EndNode, bar2.StartNode);
            }

            return false;
        }

        public int GetHashCode(Bar bar)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(bar, null)) return 0;

            return bar.StartNode.GetHashCode() ^ bar.EndNode.GetHashCode();
        }
    }
}
