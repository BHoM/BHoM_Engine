using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Constraint6DOF Constraint6DOF(string name, bool[] fixity, double[] values)
        {
            Constraint6DOF constr = new Constraint6DOF(name);
            constr.TranslationX = (fixity[0]) ? DOFType.Fixed : (values[0] == 0) ? DOFType.Free : DOFType.Spring;
            constr.TranslationY = (fixity[1]) ? DOFType.Fixed : (values[1] == 0) ? DOFType.Free : DOFType.Spring;
            constr.TranslationZ = (fixity[2]) ? DOFType.Fixed : (values[2] == 0) ? DOFType.Free : DOFType.Spring;
            constr.RotationX = (fixity[3]) ? DOFType.Fixed : (values[3] == 0) ? DOFType.Free : DOFType.Spring;
            constr.RotationY = (fixity[4]) ? DOFType.Fixed : (values[4] == 0) ? DOFType.Free : DOFType.Spring;
            constr.RotationZ = (fixity[5]) ? DOFType.Fixed : (values[5] == 0) ? DOFType.Free : DOFType.Spring;

            constr.TranslationalStiffnessX = values[0];
            constr.TranslationalStiffnessY = values[1];
            constr.TranslationalStiffnessZ = values[2];
            constr.RotationalStiffnessX = values[3];
            constr.RotationalStiffnessY = values[4];
            constr.RotationalStiffnessZ = values[5];

            return constr;
        }

        /***************************************************/

        public static Constraint6DOF PinConstraint6DOF(string name = "Pin")
        {
            Constraint6DOF constr = new Constraint6DOF(name);
            constr.TranslationX = DOFType.Fixed;
            constr.TranslationY = DOFType.Fixed;
            constr.TranslationZ = DOFType.Fixed;
            return constr;
        }

        /***************************************************/

        public static Constraint6DOF FixConstraint6DOF(string name = "Fix")
        {
            Constraint6DOF constr = new Constraint6DOF(name);
            constr.TranslationX = DOFType.Fixed;
            constr.TranslationY = DOFType.Fixed;
            constr.TranslationZ = DOFType.Fixed;
            constr.RotationX = DOFType.Fixed;
            constr.RotationY = DOFType.Fixed;
            constr.RotationZ = DOFType.Fixed;
            return constr;
        }

        /***************************************************/

        public static Constraint6DOF FullReleaseConstraint6DOF(string name = "Release")
        {
            Constraint6DOF constr = new Constraint6DOF(name);
            constr.TranslationX = DOFType.Free;
            constr.TranslationY = DOFType.Free;
            constr.TranslationZ = DOFType.Free;
            constr.RotationX = DOFType.Free;
            constr.RotationY = DOFType.Free;
            constr.RotationZ = DOFType.Free;
            return constr;
        }

        /***************************************************/
    }
}
