using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double GetMass(this Bar bar)
        {
            return bar.Length() * bar.SectionProperty.IGetMassPerMetre();
        }

        /***************************************************/
        public static double GetMassPerMetre(this ISectionProperty section)
        {
            return section.Area * section.Material.Density;
        }

        /***************************************************/

        public static double GetMassPerMetre(this ConcreteSection section)
        {
            //TODO: Handle reinforcement
            return section.Area * section.Material.Density;
        }

        /***************************************************/

        public static double GetMassPerMetre(this CompositeSection section)
        {
            //TODO: Handle embedment etc..
            return section.ConcreteSection.GetMassPerMetre() + section.SteelSection.GetMassPerMetre();
        }

        /***************************************************/

        public static double GetMassPerMetre(this CableSection section)
        {
            //TODO: Add property for kg/m as part of the cable section?
            return section.Area * section.Material.Density;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IGetMassPerMetre(this ISectionProperty section)
        {
            return GetMassPerMetre(section as dynamic);
        }

    }
}
