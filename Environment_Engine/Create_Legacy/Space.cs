using BH.oM.Environmental.Elements_Legacy;
using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Space Space(Panel panels)
        {
            return new Space { Panels = panels };
        }

        /***************************************************/
    }
}
