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
    }
}
