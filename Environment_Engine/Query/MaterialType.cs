using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Environment.Interface;
using BH.oM.Environment.Materials;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static MaterialType MaterialType(this GasMaterial gasMaterial)
        {
            return BH.oM.Environment.Elements.MaterialType.Gas;
        }

        /***************************************************/

        public static MaterialType MaterialType(this TransparentMaterial transparentMaterial)
        {
            return BH.oM.Environment.Elements.MaterialType.Transparent;
        }

        /***************************************************/

        public static MaterialType MaterialType(this OpaqueMaterial opaqueMaterial)
        {
            return BH.oM.Environment.Elements.MaterialType.Opaque;
        }

        /***************************************************/

        public static MaterialType IMaterialType(this IMaterial material)
        {
            return MaterialType(material as dynamic);
        }

        /***************************************************/
    }
}
