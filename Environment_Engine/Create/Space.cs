using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using BH.oM.Architecture.Elements;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Space Space(string name, string number, IEnumerable<BuildingElement> buildingElements)
        {
            return new Space
            {
                Name = name,
                Number = number,
                BuildingElements = buildingElements.ToList()
            };
        }

        /***************************************************/

        public static Space Space(string name, string number, IEnumerable<BuildingElementPanel> buildingElementsPanel)
        {
            List<BuildingElement> aBuildingElementList = new List<BuildingElement>();
            foreach (BuildingElementPanel aBuildingElementPanel in buildingElementsPanel)
                aBuildingElementList.Add(BuildingElement(null, aBuildingElementPanel));
                    
            return new Space
            {
                Name = name,
                Number = number,
                BuildingElements = aBuildingElementList
            };
        }

        /***************************************************/

        public static Space Space(string name, string number, Point location, Level level)
        {
            return new Space
            {
                Name = name,
                Number = number,
                Location = location,
                Level = level
            };
        }

        /***************************************************/
    }
}
