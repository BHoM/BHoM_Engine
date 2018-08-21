using BH.oM.Structure.Properties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Constraint4DOF Constraint4DOF(string name = "")
        {
            return new Constraint4DOF { Name = name };
        }

        /***************************************************/
    }
}
