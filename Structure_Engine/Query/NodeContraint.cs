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

        public static bool[] GetFixities(this Constraint6DOF constraint)
        {
            return new bool[] { constraint.TranslationX == DOFType.Fixed, constraint.TranslationY == DOFType.Fixed, constraint.TranslationZ == DOFType.Fixed,
                        constraint.RotationX == DOFType.Fixed, constraint.RotationY == DOFType.Fixed, constraint.RotationZ == DOFType.Fixed };
        }

        /***************************************************/

        public static double[] GetElasticValues(this Constraint6DOF constraint)
        {
            return new double[] { constraint.TranslationalStiffnessX, constraint.TranslationalStiffnessY, constraint.TranslationalStiffnessZ,
                        constraint.RotationalStiffnessX, constraint.RotationalStiffnessY, constraint.RotationalStiffnessZ };
        }

        /***************************************************/
    }
}
