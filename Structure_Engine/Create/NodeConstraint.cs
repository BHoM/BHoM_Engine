using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        public static NodeConstraint NodeConstraint(string name, bool[] fixity, double[] values)
        {
            NodeConstraint constr = new NodeConstraint(name);
            constr.UX = (fixity[0]) ? DOFType.Fixed : (values[0] == 0) ? DOFType.Free : DOFType.Spring;
            constr.UY = (fixity[1]) ? DOFType.Fixed : (values[1] == 0) ? DOFType.Free : DOFType.Spring;
            constr.UZ = (fixity[2]) ? DOFType.Fixed : (values[2] == 0) ? DOFType.Free : DOFType.Spring;
            constr.RX = (fixity[3]) ? DOFType.Fixed : (values[3] == 0) ? DOFType.Free : DOFType.Spring;
            constr.RY = (fixity[4]) ? DOFType.Fixed : (values[4] == 0) ? DOFType.Free : DOFType.Spring;
            constr.RZ = (fixity[5]) ? DOFType.Fixed : (values[5] == 0) ? DOFType.Free : DOFType.Spring;

            constr.KX = values[0];
            constr.KY = values[1];
            constr.KZ = values[2];
            constr.HX = values[3];
            constr.HY = values[4];
            constr.HZ = values[5];

            return constr;
        }

        /***************************************************/

        public static NodeConstraint PinNodeConstraint(string name = "Pin")
        {
            NodeConstraint constr = new NodeConstraint(name);
            constr.UX = DOFType.Fixed;
            constr.UY = DOFType.Fixed;
            constr.UZ = DOFType.Fixed;
            return constr;
        }

        /***************************************************/

        public static NodeConstraint FixNodeConstraint(string name = "Fix")
        {
            NodeConstraint constr = new NodeConstraint(name);
            constr.UX = DOFType.Fixed;
            constr.UY = DOFType.Fixed;
            constr.UZ = DOFType.Fixed;
            constr.RX = DOFType.Fixed;
            constr.RY = DOFType.Fixed;
            constr.RZ = DOFType.Fixed;
            return constr;
        }

        /***************************************************/

        public static NodeConstraint FullReleaseNodeConstraint(string name = "Release")
        {
            NodeConstraint constr = new NodeConstraint(name);
            constr.UX = DOFType.Free;
            constr.UY = DOFType.Free;
            constr.UZ = DOFType.Free;
            constr.RX = DOFType.Free;
            constr.RY = DOFType.Free;
            constr.RZ = DOFType.Free;
            return constr;
        }

    }
}
