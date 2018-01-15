using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Query 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool HasModifiers(Property2D property)
        {
            foreach (double modifier in property.Modifiers)
            {
                if (modifier != 1) return true;
            }
            return false;
        }

        /***************************************************/
    }
}
