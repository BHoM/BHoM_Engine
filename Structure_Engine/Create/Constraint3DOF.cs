using BH.oM.Structure.Properties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Constraint3DOF Constraint3DOF(string name = "")
        {
            return new Constraint3DOF { Name = name };
        }

        /***************************************************/
    }
}
