using BH.oM.Structure.Properties;
using BH.oM.Reflection.Attributes;
using System;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double MassPerArea(this ConstantThickness constantThickness)
        {
            return constantThickness.Thickness * constantThickness.Material.Density;
        }

        /***************************************************/

        [NotImplemented]
        public static double MassPerArea(this Ribbed ribbedProperty)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static double MassPerArea(this Waffle ribbedProperty)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static double MassPerArea(this LoadingPanelProperty loadingPanelProperty)
        {
            return 0;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IMassPerArea(this IProperty2D property)
        {
            return MassPerArea(property as dynamic);
        }

        /***************************************************/
    }
}
