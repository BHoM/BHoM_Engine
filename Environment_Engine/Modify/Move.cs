using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Interface;
using BH.oM.Environmental.Elements;
using BH.oM.Structural.Elements;
using BH.oM.Geometry;

using BH.Engine.Geometry;


namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BuildingElement Move(this BuildingElement BuildingElement, Storey Storey)
        {
            Vector aVector = Geometry.Create.Vector();
            if(BuildingElement.Storey != null)
                aVector = Geometry.Create.Vector(0, 0, Storey.Elevation - BuildingElement.Storey.Elevation);

            BuildingElement.Move(aVector);
            BuildingElement.Storey = Storey;

            return BuildingElement;
        }

        public static BuildingElement Move(this BuildingElement BuildingElement, Vector Vector)
        {
            Move(BuildingElement.BuildingElementGeometry, Vector);

            return BuildingElement;
        }

        public static IBuildingElementGeometry Move(this IBuildingElementGeometry BuildingElementGeometry, Vector Vector)
        {
            BuildingElementGeometry.Curve.ITranslate(Vector);
            return BuildingElementGeometry;
        }
    }
}
