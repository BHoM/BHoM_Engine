using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Mass(this Bar bar)
        {
            return bar.Length() * bar.SectionProperty.IMassPerMetre();
        }

        /***************************************************/
    }
}
