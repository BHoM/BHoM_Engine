using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;

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
        public static double MassPerMetre(this ISectionProperty section)
        {
            return section.Area * section.Material.Density;
        }

        /***************************************************/

        public static double MassPerMetre(this ConcreteSection section)
        {
            //TODO: Handle reinforcement
            return section.Area * section.Material.Density;
        }

        /***************************************************/

        public static double MassPerMetre(this CompositeSection section)
        {
            //TODO: Handle embedment etc..
            return section.ConcreteSection.MassPerMetre() + section.SteelSection.MassPerMetre();
        }

        /***************************************************/

        public static double MassPerMetre(this CableSection section)
        {
            //TODO: Add property for kg/m as part of the cable section?
            return section.Area * section.Material.Density;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IMassPerMetre(this ISectionProperty section)
        {
            return MassPerMetre(section as dynamic);
        }

        /***************************************************/
    }
}
