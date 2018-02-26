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
            return new BarRelease { StartRelease = FullReleaseConstraint6DOF(), EndRelease = FullReleaseConstraint6DOF(), Name = name };
        }

        /***************************************************/

        public static BarRelease BarReleasePinPin(string name = "PinPin")
        {
            return new BarRelease
            {
                StartRelease = Constraint6DOF(false, false, false, true,true,true, "Pin"),
                EndRelease = Constraint6DOF(false, false, false, true, true, true, "Pin"),
                Name = name
            };
        }

        /***************************************************/

        public static BarRelease BarReleasePinSlip(string name = "PinSlip")
        {
            return new BarRelease
            {
                StartRelease = Constraint6DOF(false, false, false, true, true, true, "Pin"),
                EndRelease = Constraint6DOF(true, false, false, true, true, true, "Slip"),
                Name = name
            };
        }

        /***************************************************/
    }
}
