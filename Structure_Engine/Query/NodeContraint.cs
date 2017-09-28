using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool[] GetFixities(this NodeConstraint constraint)
        {
            return new bool[] { constraint.UX == DOFType.Fixed, constraint.UY == DOFType.Fixed, constraint.UZ == DOFType.Fixed,
                        constraint.RX == DOFType.Fixed, constraint.RY == DOFType.Fixed, constraint.RZ == DOFType.Fixed };
        }

        /***************************************************/

        public static double[] GetElasticValues(this NodeConstraint constraint)
        {
            return new double[] { constraint.KX, constraint.KY, constraint.KZ,
                        constraint.HX, constraint.HY, constraint.HZ };
        }

        /***************************************************/
    }
}
