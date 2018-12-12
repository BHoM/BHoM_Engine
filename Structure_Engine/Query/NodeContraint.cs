using BH.oM.Structure.Properties.Constraint;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool[] Fixities(this Constraint6DOF constraint)
        {
            return new bool[] { constraint.TranslationX == DOFType.Fixed, constraint.TranslationY == DOFType.Fixed, constraint.TranslationZ == DOFType.Fixed,
                        constraint.RotationX == DOFType.Fixed, constraint.RotationY == DOFType.Fixed, constraint.RotationZ == DOFType.Fixed };
        }

        /***************************************************/

        public static double[] ElasticValues(this Constraint6DOF constraint)
        {
            return new double[] { constraint.TranslationalStiffnessX, constraint.TranslationalStiffnessY, constraint.TranslationalStiffnessZ,
                        constraint.RotationalStiffnessX, constraint.RotationalStiffnessY, constraint.RotationalStiffnessZ };
        }

        /***************************************************/
    }
}
