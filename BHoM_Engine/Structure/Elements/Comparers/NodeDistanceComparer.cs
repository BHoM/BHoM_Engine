using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;
using BH.Engine.Geometry;

namespace BH.Engine.Structure.Elements
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

        public bool Equals(Node x, Node y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check if the GUIDs are the same
            if (x.BHoM_Guid == y.BHoM_Guid)
                return true;

            if ((int)Math.Round(x.Point.X * m_multiplier) != (int)Math.Round(y.Point.X * m_multiplier))
                return false;

            if ((int)Math.Round(x.Point.Y * m_multiplier) != (int)Math.Round(y.Point.Y * m_multiplier))
                return false;

            if ((int)Math.Round(x.Point.Z * m_multiplier) != (int)Math.Round(y.Point.Z * m_multiplier))
                return false;

            return true;
        }

        public int GetHashCode(Node obj)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;

            int x = ((int)Math.Round(obj.Point.X * m_multiplier)).GetHashCode();
            int y = ((int)Math.Round(obj.Point.X * m_multiplier)).GetHashCode();
            int z = ((int)Math.Round(obj.Point.X * m_multiplier)).GetHashCode();
            return x ^ y ^ z;

        }
    }
}
