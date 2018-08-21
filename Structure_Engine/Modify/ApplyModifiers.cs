using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Properties;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IProperty2D ApplyModifiers(this IProperty2D prop, double f11 =1, double f12=1, double f22=1, double m11=1, double m12=1, double m22=1, double v13=1, double v23=1, double mass=1, double weight=1)
        {
            IProperty2D clone = prop.GetShallowClone() as IProperty2D;

            double[] modifiers = new double[] { f11, f12, f22, m11, m12, m22, v13, v23, mass, weight };

            clone.CustomData["Modifiers"] = modifiers;

            return clone;
        }

        /***************************************************/

        public static ISectionProperty ApplyModifiers(this ISectionProperty prop, double area = 1, double iy = 1, double iz = 1, double j = 1, double asy = 1, double asz = 1)
        {
            ISectionProperty clone = prop.GetShallowClone() as ISectionProperty;

            double[] modifiers = new double[] { area, iy, iz, j, asy, asz };

            clone.CustomData["Modifiers"] = modifiers;

            return clone;
        }

        /***************************************************/
    }
}
