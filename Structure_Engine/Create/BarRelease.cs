using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BarRelease BarRelease(Constraint6DOF startConstraint, Constraint6DOF endConstraint, string name = "")
        {
            return new BarRelease { StartRelease = startConstraint, EndRelease = endConstraint, Name = name };
        }

        /***************************************************/

        public static BarRelease BarReleaseFixFix( string name = "FixFix")
        {
            return new BarRelease { StartRelease = FixConstraint6DOF(), EndRelease = FixConstraint6DOF(), Name = name };
        }

        /***************************************************/

        public static BarRelease BarReleasePinPin(string name = "PinPin")
        {

            Constraint6DOF endRelease = PinConstraint6DOF();
            endRelease.RotationX = DOFType.Fixed;

            return new BarRelease
            {
                StartRelease = PinConstraint6DOF(),
                EndRelease = endRelease,
                Name = name
            };
        }

        /***************************************************/

        public static BarRelease BarReleasePinSlip(string name = "PinSlip")
        {
            return new BarRelease
            {
                StartRelease = PinConstraint6DOF(),
                EndRelease = Constraint6DOF(false, true, true, false, false, false, "Slip"),
                Name = name
            };
        }

        /***************************************************/
    }
}
