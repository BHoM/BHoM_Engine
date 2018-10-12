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

        /***************************************************/

        public static List<Space> Spaces(this List<List<BuildingElement>> besAsSpace)
        {
            List<Space> spaces = new List<Space>();

            for (int x = 0; x < besAsSpace.Count; x++)
                spaces.Add(besAsSpace[x].Space(x.ToString(), x.ToString()));

            return spaces;
        }

        /***************************************************/

        public static Space Space(this List<BuildingElement> space)
        {
            Point spaceCentre = space.SpaceCentre();
            string xName = spaceCentre.X.ToString().Length > 3 ? spaceCentre.X.ToString().Substring(0, 3) : spaceCentre.X.ToString();
            string yName = spaceCentre.Y.ToString().Length > 3 ? spaceCentre.Y.ToString().Substring(0, 3) : spaceCentre.Y.ToString();
            string zName = spaceCentre.Z.ToString().Length > 3 ? spaceCentre.Z.ToString().Substring(0, 3) : spaceCentre.Z.ToString();
            string spaceName = xName + "-" + yName + "-" + zName;
            return Create.Space(spaceName, spaceName, space.SpaceCentre());
        }

        /***************************************************/

        public static Space Space(this List<BuildingElement> space, string spaceNumber, string spaceName)
        {
            return Create.Space(spaceName, spaceNumber, space.SpaceCentre());
        }

        /***************************************************/

        public static Space Space(this List<BuildingElement> space, int spaceNumber, string spaceName)
        {
            return Space(space, spaceNumber.ToString(), spaceName);
        }
    }
}
