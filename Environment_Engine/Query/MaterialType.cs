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

        public static MaterialType MaterialType(this GasMaterial gasMaterial)
        {
            return BH.oM.Environmental.Elements.MaterialType.Gas;
        }

        /***************************************************/

        public static MaterialType MaterialType(this TransparentMaterial transparentMaterial)
        {
            return BH.oM.Environmental.Elements.MaterialType.Transparent;
        }

        /***************************************************/

        public static MaterialType MaterialType(this OpaqueMaterial opaqueMaterial)
        {
            return BH.oM.Environmental.Elements.MaterialType.Opaque;
        }

        /***************************************************/

        public static MaterialType IMaterialType(this IMaterial material)
        {
            return MaterialType(material as dynamic);
        }

        /***************************************************/
    }
}
