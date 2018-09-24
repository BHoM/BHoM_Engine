using System.Linq;
using System.Collections.Generic;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Architecture.Elements;

using BH.oM.Base;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Space> Spaces(this List<IBHoMObject> bhomObjects)
        {
            List<Space> spaces = new List<Space>();

            foreach(IBHoMObject obj in bhomObjects)
            {
                if (obj is Space)
                    spaces.Add(obj as Space);
            }

            return spaces;
        }

    }
}
