using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine_Explore.BHoM.Structural.Elements;
using Engine_Explore.BHoM.Structural.Properties;
using Interop.gsa_8_7;
using System.Collections;

namespace Engine_Explore.Engine.Convert
{
    public static class GsaElement
    {
        public static string CustomKey { get; } = "GSA_id";

        public static Node Read(GsaNode gsaNode)
        {
            Node bhomNode = new Node
            {
                Name = gsaNode.Name,
                Point = new BHoM.Geometry.Point(gsaNode.Coor[0], gsaNode.Coor[1], gsaNode.Coor[2]),
                Constraint = ReadNodeConstraint("", gsaNode.Restraint, gsaNode.Stiffness)
            };

            bhomNode.CustomData[CustomKey] = gsaNode.Ref;
            return bhomNode;
        }

        /***************************************************/

        public static NodeConstraint ReadNodeConstraint(string name, int gsaRestraint, double[] gsaStiffness)
        {
            BitArray a = new BitArray(new int[] { gsaRestraint });
            bool[] fixities = { a[0], a[1], a[2], a[3], a[4], a[5] };

            return new NodeConstraint(name, fixities, gsaStiffness);
        }
    }
}
