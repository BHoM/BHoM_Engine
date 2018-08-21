using BH.oM.Structure.Properties;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool HasModifiers(this IProperty2D property)
        {
            double[] modifiers = property.Modifiers();

            if (modifiers == null)
                return false;

            foreach (double modifier in modifiers)
            {
                if (modifier != 1) return true;
            }
            return false;
        }

        /***************************************************/

        public static double[] Modifiers(this IProperty2D property)
        {
            object modifersObj;

            if (property.CustomData.TryGetValue("Modifiers", out modifersObj))
            {
                return modifersObj as double[];
            }

            return null;
        }

        /***************************************************/

        public static bool HasModifiers(ISectionProperty property)
        {
            double[] modifiers = property.Modifiers();

            if (modifiers == null)
                return false;

            foreach (double modifier in modifiers)
            {
                if (modifier != 1) return true;
            }
            return false;
        }

        /***************************************************/

        public static double[] Modifiers(this ISectionProperty property)
        {
            object modifersObj;

            if (property.CustomData.TryGetValue("Modifiers", out modifersObj))
            {
                return modifersObj as double[];
            }

            return null;
        }

        /***************************************************/
    }
}
