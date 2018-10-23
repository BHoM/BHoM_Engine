using System;
using System.Collections.Generic;
using BH.oM.Environment.Elements;

using BH.oM.Base;

using System.Linq;

using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BuildingElement> BuildingElements(this List<IBHoMObject> bhomObjects)
        {
            List<BuildingElement> bes = new List<BuildingElement>();

            foreach(IBHoMObject obj in bhomObjects)
            {
                if (obj is BuildingElement)
                    bes.Add(obj as BuildingElement);
            }

            return bes;
        }

        public static List<BuildingElement> UniqueBuildingElements(this List<List<BuildingElement>> elements)
        {
            List<BuildingElement> rtn = new List<BuildingElement>();

            foreach(List<BuildingElement> lst in elements)
            {
                foreach(BuildingElement be in lst)
                {
                    BuildingElement beInList = rtn.Where(x => x.BHoM_Guid == be.BHoM_Guid).FirstOrDefault();
                    if (beInList == null)
                        rtn.Add(be);
                }
            }

            return rtn;
        }

        public static List<BuildingElement> ConnectedElements(this BuildingElement element, List<BuildingElement> elements)
        {
            List<BuildingElement> connectedElement = new List<BuildingElement>();

            List<Point> vertexPts = element.PanelCurve.IControlPoints();

            foreach(BuildingElement be in elements)
            {
                if (be.BHoM_Guid == element.BHoM_Guid) continue; //Don't check the same element...

                List<Point> vPts = be.PanelCurve.IControlPoints();

                //Check if at least one point matches in some manner
                bool ptMatches = false;
                foreach(Point pt in vPts)
                {
                    ptMatches = vertexPts.Contains(pt);
                    if (ptMatches) break; //Have found a match so no need to check the rest
                }

                if (ptMatches) connectedElement.Add(be);
            }
                
            return connectedElement;
        }
    }
}

