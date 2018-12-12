using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Properties.Surface;
using BH.oM.Structure.Properties.Section;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ISurfaceProperty ApplyModifiers(this ISurfaceProperty prop, double f11 =1, double f12=1, double f22=1, double m11=1, double m12=1, double m22=1, double v13=1, double v23=1, double mass=1, double weight=1)
        {
            ISurfaceProperty clone = prop.GetShallowClone() as ISurfaceProperty;

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
