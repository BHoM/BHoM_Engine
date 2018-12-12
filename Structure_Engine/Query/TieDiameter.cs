using BH.oM.Structure.Properties.Section;
using BH.oM.Structure.Properties.Section.Reinforcement;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double TieDiameter(this ConcreteSection property)
        {
            foreach (Reinforcement reo in property.Reinforcement)
            {
                if (reo is TieReinforcement)
                {
                    return reo.Diameter;
                }
            }
            return 0;
        }

        /***************************************************/
    }
}
