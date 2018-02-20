using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Elements;
using BH.oM.Environmental.Interface;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
#
        public static MaterialType MaterialType(this GasMaterial gasMaterial)
        {
            return Curve(gasMaterial as dynamic);
        }

        public static MaterialType MaterialType(this TransparentMaterial transparentMaterial)
        {
            return Curve(transparentMaterial as dynamic);
        }

        public static MaterialType MaterialType(this OpaqueMaterial opaqueMaterial)
        {
            return Curve(opaqueMaterial as dynamic);
        }


        public static MaterialType IMaterialType(this IMaterial material)
        {
            return Curve(material as dynamic);
        }

        /***************************************************/
    }
}
