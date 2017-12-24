using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsConstrained(this Node node)
        {
            return node.Constraint != null;
        }

        /***************************************************/

    }
}
