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

            foreach (IBHoMObject obj in bhomObjects)
            {
                if (obj is BuildingElement)
                    bes.Add(obj as BuildingElement);
            }

            return bes;
        }

        public static List<BuildingElement> UniqueBuildingElements(this List<List<BuildingElement>> elements)
        {
            List<BuildingElement> rtn = new List<BuildingElement>();

            foreach (List<BuildingElement> lst in elements)
            {
                foreach (BuildingElement be in lst)
                {
                    BuildingElement beInList = rtn.Where(x => x.BHoM_Guid == be.BHoM_Guid).FirstOrDefault();
                    if (beInList == null)
                        rtn.Add(be);
                }
            }

            return rtn;
        }

        public static List<BuildingElement> ShadingElements(this List<BuildingElement> elements)
        {
            //Isolate all of the shading elements in the list - shading elements are ones connected only along one edge
            List<BuildingElement> shadingElements = new List<BuildingElement>();

            foreach (BuildingElement be in elements)
            {
                if (be.BuildingElementProperties != null)
                {
                    if (be.BuildingElementProperties.CustomData.ContainsKey("SAM_BuildingElementType"))
                    {
                        object aObject = be.BuildingElementProperties.CustomData["SAM_BuildingElementType"];

                        if (aObject != null && aObject.ToString().ToLower().Contains("shade"))
                            shadingElements.Add(be);
                            
                    }
                }
            }

            return shadingElements;
        }

        public static List<BuildingElement> ElementsByType(this List<BuildingElement> elements, BuildingElementType type)
        {
            return elements.Where(x => x.BuildingElementProperties.BuildingElementType == type).ToList();
        }

        public static List<BuildingElement> ElementsByWallType(this List<BuildingElement> elements)
        {
            return elements.ElementsByType(BuildingElementType.Wall);
        }

    }
}

